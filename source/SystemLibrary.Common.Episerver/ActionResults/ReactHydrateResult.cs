//using System;
//using System.Dynamic;
//using System.Text;

//using EPiServer;

//using SystemLibrary.Common.Net.Extensions;

//namespace SystemLibrary.Common.Episerver.ActionResults;

//// Calls the initial render method, properties are not ready, but all properties are set on the html element as a json string which is now ready to be parsed and Hydrated by the react function React.Hydrate(element);
//public class ReactHydrateResult : BaseReactResult
//{
//    public ReactHydrateResult(object model, object additionalProps = null, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null)
//    {
//        componentFullName = GetReactComponentFullName(model, componentFullName);

//        ContentType = "text/html";

//        Content = GetReactHydrateComponentContent(model, additionalProps, tagName, camelCaseProps, cssClass, id, componentFullName);
//    }

//    static string GetReactHydrateComponentContent(object model, object additionalProps, string tagName = "div", bool camelCaseProps = false, string cssClass = null, string id = null, string componentFullName = null, bool renderClientOnly = false, bool renderServerOnly = true)
//    {
//        if (tagName.StartsWith("<"))
//            throw new Exception("'tagName' should not include < > letters");

//        if (additionalProps != null && !additionalProps.GetOriginalType().IsClass)
//            throw new Exception("'additionalProps' passed must be a class with C# properties, where they will be passed as props into your react component");

//        var props = GetModelAsExpandoObjectProps(model, additionalProps, camelCaseProps);

//        var content = new StringBuilder("<" + tagName);
//        if (id.Is())
//            content.Append(" id=\"{id}\"");

//        // Use Sytem.Text to serialize into "hydrate=" object, through simply SystemLibrary.Json()

//        content.Append("hydrate=\"" + props.Json() + "\"");

//        throw new Exception("Not yet implemented");

//        return content.ToString();
//    }

//    static ExpandoObject GetModelAsExpandoObjectProps(object model, object additionalProps, bool camelCaseProps)
//    {
//        // Loop through both model and additionalProps, into a new expandoobject... 
//        return null;
//    }
//}