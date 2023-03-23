//using React.AspNet;

//namespace SystemLibrary.Common.Episerver.ActionResults;

//// Calls the inital render method with all properites being set, but events are still not setup in frontend, so a call to React....something is needed
//public class ReactSsrResult<T> : BaseReactResult where T : class
//{
//    public ReactSsrResult(T viewModel, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null)
//    {
//        componentFullName = GetReactComponentFullName(viewModel, componentFullName);

//        ContentType = "text/html";

//        var htmlHelper = HtmlHelperFactory.Build<T>();

//        Content = htmlHelper.React<T>(componentFullName, viewModel, tagName, id, false, true, cssClass)?.ToString();
//    }
//}
