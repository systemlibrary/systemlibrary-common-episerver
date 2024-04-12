using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Web;

using EPiServer;
using EPiServer.Core;
using EPiServer.Shell;
using EPiServer.SpecializedProperties;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Primitives;
using Microsoft.Identity.Client.Extensions.Msal;

using React;

using SystemLibrary.Common.Episerver.Cms.Properties;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;
using SystemLibrary.Common.Web;

using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SystemLibrary.Common.Episerver.Extensions;

public static partial class TExtensions
{
    internal const string SysLibStorageLevel = "___" + nameof(SysLibStorageLevel);
    internal const string SysLibStorageHiddenInputs = "___" + nameof(SysLibStorageHiddenInputs);
    internal const string SysLibStorageSsrId = "___" + nameof(SysLibStorageSsrId);

    /// <summary>
    /// Return 'Model' as a serer side rendered component or ready to be hydrated, or both.
    /// 
    /// Simply call ReactDOM.Hydrate or the React 18 version of hydration.
    /// 
    /// Throws exception if invalid combinations in arguments.
    /// 
    /// Should not throw if it executes, if a React rendering error occurs it is logged and printed in the DOM.
    /// - The div rendered has a class 'ssr-errored' which can be used to hide it in non-dev environments
    /// - The div only contains the message of the erorr, not stacktrace
    /// 
    /// NOTE: If 'Model' is of type ContentData from Optimizely CMS, then it skips the default Optimizely CMS properties, such as:
    /// Name, Property, Item, IsReadOnly, IsModified, ContentTypeID, ViewData, ContentLink, 
    /// ParentLink, ArchiveLink, Category and more... To bypass? Create your own viewmodel
    /// 
    /// NOTE: Always skipped property names: CurrentPage, CurrentBlock
    /// </summary>
    public static StringBuilder ReactServerSideRender<T>(this T model, object additionalProps = null, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null, bool renderClientOnly = false, bool renderServerOnly = false, bool printNullValues = true) where T : class
    {
        Validate(model, additionalProps, tagName, renderClientOnly, renderServerOnly);

        var renderServerSide = !renderClientOnly || renderServerOnly;
        var renderClientSide = renderClientOnly || !renderServerOnly;

        var level = IncrementLevel(renderClientSide);

        if (Globals.IsUnitTesting) level = 1;

        var props = ModelToProps(model, additionalProps, camelCaseProps, printNullValues);

        string jsonProps = PropsToJsonProps(props, camelCaseProps);

        var ssrId = GetSSRID(id, model, props, jsonProps);

        componentFullName = GetComponentFullName(model, componentFullName);

        var root = GetRootElementStart(componentFullName, id, cssClass, tagName, renderClientSide, ssrId);

        try
        {
            var serverSideComponent = CreateServerSideComponent(props, jsonProps, componentFullName, tagName, cssClass, renderServerSide, renderClientSide);

            RenderServerSideComponent(root, serverSideComponent, ssrId, tagName, renderClientSide);
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            root.Append("<div class='ssr-errored'>Exception: " + ex.Message + "</div>");
        }
        finally
        {
            try
            {
                ReactEnvironment.GetCurrentOrThrow.ReturnEngineToPool();
            }
            catch (Exception ex)
            {
                if (!Globals.IsUnitTesting)
                    Log.Error("React returning engine too pool failed, continue silently..." + ex.Message);
            }
        }

        AppendRootElementEnd(root, tagName);

        var ssrIdStore = GetSsrIdStore(renderClientSide);

        level = DecrementLevel(renderClientSide);

        if (Globals.IsUnitTesting) level = 0;

        AppendHiddenInput(level, ssrId, componentFullName, jsonProps, ssrIdStore, root);

        return root;
    }
}
