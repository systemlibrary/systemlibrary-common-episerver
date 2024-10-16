using System.Text;

using EPiServer.Core;

using React;

using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Extensions for Generic Types where T is a class
/// </summary>
public static partial class TExtensions
{
    internal const string SysLibStorageLevel = "___" + nameof(SysLibStorageLevel);
    internal const string SysLibStorageHiddenInputs = "___" + nameof(SysLibStorageHiddenInputs);
    internal const string SysLibStorageSsrId = "___" + nameof(SysLibStorageSsrId);

    internal static bool ShowComponentEditLink = AppSettings.Current?.Edit?.ShowComponentEditLink == true;

    /// <summary>
    /// Return 'Model' as a serer side rendered component or ready to be hydrated, or both.
    /// <para>Simply call ReactDOM.Hydrate or the React 18 version of hydration.</para>
    /// <para>Throws exception if invalid combinations in arguments.</para>
    /// Should not throw if it executes, if a React rendering error occurs it is logged and printed in the DOM.
    /// <para>- The div rendered has a class 'sle-ssr-error' which can be used to hide it in non-dev environments</para>
    /// <para>- The div only contains the message of the erorr, not stacktrace</para>
    /// <para>Always skipped property names: CurrentPage, CurrentBlock</para>
    /// </summary>
    /// <remarks>
    /// If 'Model' is of type ContentData from Optimizely CMS, then it skips the default Optimizely CMS properties, such as:
    /// <para>Name, Property, Item, IsReadOnly, IsModified, ContentTypeID, ViewData, ContentLink, 
    /// ParentLink, ArchiveLink, Category and more... To bypass? Create your own viewmodel</para>
    /// </remarks>
    public static StringBuilder ReactServerSideRender<T>(this T model, object additionalProps = null, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null, bool renderClientOnly = false, bool renderServerOnly = false, bool printNullValues = true) where T : class
    {
        if (model == null)
        {
            Log.Error("Model is null in ReactServerSideRender for cssClass, id and componentFullname: " + cssClass + ", " + id + ", " + componentFullName);

            var path = HttpContextInstance.Current?.Request.Url();

            if (path?.EndsWithAny(StringComparison.Ordinal, "Block", "Block/", "Component/", "Component") == true)
            {
                return new StringBuilder("");
            }
            return new StringBuilder("<div class='" + Globals.CssClassName.SsrError + "' style=\"color:darkred;background-color:white;width:100%;max-width:1920px;border-top:1px solid red; border-bottom:1px solid red;\">Component: " + (componentFullName ?? id) + "<br/>Exception: Model passed to server side rendering is null, cannot continue... Do you have duplicate controllers? Have you used Controller instead of Component?<br/>Tip: Hide this error through css class '" + Globals.CssClassName.SsrError + "'</div>");
        }
        var modelType = Validate(model, additionalProps, tagName, cssClass, renderClientOnly, renderServerOnly);

        var renderServerSide = !renderClientOnly || renderServerOnly;
        var renderClientSide = renderClientOnly || !renderServerOnly;

        var level = IncrementLevel(renderClientSide);

        if (level > 512)
        {
            level = DecrementLevel(renderClientSide);
            return new StringBuilder("");
        }

        if (Globals.IsUnitTesting && renderClientSide) level = 1;

        var props = ModelToProps(modelType, model, additionalProps, camelCaseProps, printNullValues);

        var jsonProps = PropsToJsonProps(props, camelCaseProps);

        var ssrId = GetSSRID(renderClientSide, id, model, props, jsonProps);

        componentFullName = GetComponentFullName(modelType, model, componentFullName);

        var root = GetRootElementStart(componentFullName, id, cssClass, tagName, ssrId);

        try
        {
            var serverSideComponent = CreateServerSideComponent(props, jsonProps, componentFullName, tagName, cssClass, renderServerSide, renderClientSide);

            RenderServerSideComponent(root, serverSideComponent, ssrId, tagName, renderClientSide);
        }
        catch (Exception ex)
        {
            var message = "Component: " + componentFullName + ", tagName: " + tagName + ", hasProps: " + (props.Count > 0) + "\nException: ";
            Log.Error(message + ex.ToString());

            if (ex.Message.Contains("not find a comp") || ex.Message.Contains("likely forgot to export"))
            {
                root.Append("<div class='" + Globals.CssClassName.SsrError + "' style=\"color:darkred;background-color:white;width:100%;max-width:1920px;border-top:1px solid red; border-bottom:1px solid red;\">Component: " + componentFullName + "<br/>Exception: Not found at server-side rendering. The component must be available from window object in your server-side script, so make sure you've exported it. Usually like this: window." + componentFullName + "=your import;. Or did you forget to add it to App_Start\\ReactConfig.cs? Or a typo?<br/>Note: restart APP to reload script changes<br/>Tip: Hide this error by css class '" + Globals.CssClassName.SsrError + "'</div>");
            }
            else
            {
                root.Append("<div class='" + Globals.CssClassName.SsrError + "' style=\"color:darkred;background-color:white;width:100%;max-width:1920px;border-top:1px solid red; border-bottom:1px solid red;\">Component: " + componentFullName + "<br/>Exception: " + ex.Message + "<br/>Note: restart APP to reload script changes<br/>Tip: Hide this error by css class '" + Globals.CssClassName.SsrError + "'</div>");
            }
        }

        AppendRootElementEnd(root, tagName);

        var ssrIdStore = GetSsrIdStore(renderClientSide);

        level = DecrementLevel(renderClientSide);

        if (Globals.IsUnitTesting && renderClientSide) level = 0;

        if (level <= 0 && renderServerSide)
        {
            try
            {
                ReactEnvironment.GetCurrentOrThrow.ReturnEngineToPool();
            }
            catch (Exception ex)
            {
                if (!Globals.IsUnitTesting)
                    Log.Error("React returning engine to pool failed, continue silently... " + ex.Message);
            }
        }

        if (ShowComponentEditLink && tagName != null && level <= 1)
        {
            if (model is IContent content)
            {
                var link = ComponentEditLink.Create(content);
                if (link != null)
                {
                    root.Append(link);
                }
            }
        }

        AppendHiddenInput(level, ssrId, componentFullName, jsonProps, ssrIdStore, root);

        return root;
    }
}
