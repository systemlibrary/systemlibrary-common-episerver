//using EPiServer;
//using EPiServer.Core;
//using EPiServer.ServiceLocation;
//using EPiServer.Web;
//using EPiServer.Web.Mvc;
//using EPiServer.Web.Mvc.Html;

//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace SystemLibrary.Common.Episerver.Initialize;

//internal partial class CustomContentAreaRenderer : ContentAreaRenderer
//{
//    static IContentAreaLoader ContentAreaLoader;
//    static IContentAreaItemAttributeAssembler AttributeAssembler;
//    static IContentRenderer ContentRenderer;

//    static CustomContentAreaRenderer()
//    {
//        ContentAreaLoader = ServiceLocator.Current.GetInstance<IContentAreaLoader>();
//        AttributeAssembler = ServiceLocator.Current.GetInstance<IContentAreaItemAttributeAssembler>();
//        ContentRenderer = ServiceLocator.Current.GetInstance<IContentRenderer>();
//    }

//    protected override void RenderContentAreaItem(IHtmlHelper htmlHelper, ContentAreaItem contentAreaItem, string templateTag, string htmlTag, string cssClass)
//    {
//        var isInEditMode = IsInEditMode();

//        var shouldAddParentWrapper = ShouldRenderWrappingElement(htmlHelper);

//        IContent content = ContentAreaLoader.Get(contentAreaItem);

//        if (content == null) return;

//        using (new ContentAreaContext(htmlHelper.ViewContext.HttpContext, content))
//        {
//            var templateModel = this.ResolveContentTemplate(htmlHelper, content, templateTag != null ? [templateTag] : new string[] { });

//            // Fallback without the template tag, if it was specified...
//            if (templateModel == null && templateTag != null)
//                templateModel = ResolveContentTemplate(htmlHelper, content, new string[] { });

//            if (templateModel == null)
//                Log.Error("Could not find a matching content template for content of type: " + content.GetOriginalType().Name + ". Tried with template tag: " + templateTag + ". There's no controller for this Type/Tag");

//            TagBuilder tagBuilder = null;

//            if (shouldAddParentWrapper)
//            {
//                if (htmlTag.IsNot())
//                    tagBuilder = new TagBuilder("div");
//                else
//                    tagBuilder = new TagBuilder(htmlTag);

//                AddNonEmptyCssClass(tagBuilder, cssClass);

//                var assembler = ServiceLocator.Current.GetInstance<IContentAreaItemAttributeAssembler>();

//                tagBuilder.MergeAttributes(assembler.GetAttributes(contentAreaItem, isInEditMode, templateModel != null));

//                BeforeRenderContentAreaItemStartTag(tagBuilder, contentAreaItem);
//            }

//            if (!isInEditMode)
//            {
//                var user = htmlHelper.ViewContext.HttpContext.User;
//                var identity = user?.Identity;
//                if (identity != null && identity.IsAuthenticated)
//                {
//                    var link = ComponentEditLink.Create(user, content);
//                    if (link != null)
//                        htmlHelper.ViewContext.Writer.Write(link);
//                }
//            }

//            htmlHelper.ViewContext.Writer.Write(tagBuilder?.RenderStartTag());

//            var insideContentArea = isInEditMode;

//            try
//            {
//                htmlHelper.RenderContentData(content, insideContentArea, templateModel, ContentRenderer);
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error rendering " + templateModel?.Name + " with message: " + (ex.InnerException?.Message ?? ex.Message));
//            }

//            htmlHelper.ViewContext.Writer.Write(tagBuilder?.RenderEndTag());
//        }
//    }

//    protected override bool ShouldRenderWrappingElement(IHtmlHelper htmlHelper)
//    {
//        return (bool?)htmlHelper.ViewContext?.ViewData["hascontainer"] != false || IsInEditMode();
//    }
//}

////partial class CustomContentAreaRenderer : ContentAreaRenderer
////{
////    static Dictionary<string, object> GetRenderSettings(ContentAreaItem contentAreaItem, string templateTag, string htmlTag, string cssClass)
////    {
////        var settings = new Dictionary<string, object>()
////        {
////            ["childrencustomtagname"] = htmlTag,
////            ["childrencssclass"] = cssClass,
////            ["tag"] = templateTag
////        };

////        return contentAreaItem.RenderSettings.Concat(
////                from r in settings
////                where !contentAreaItem.RenderSettings.ContainsKey(r.Key)
////                select r
////                )
////                .ToDictionary(r => r.Key, r => r.Value);
////    }
////}