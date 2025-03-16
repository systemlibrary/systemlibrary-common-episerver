using EPiServer;

using SystemLibrary.Common.Framework.Extensions;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static Type Validate(object propsModel, object additionalProps, string tagName, string cssClass, bool renderClientOnly, bool renderServerOnly)
    {
        var type = propsModel.GetOriginalType();

        if (!type.IsClass)
            throw new Exception("'viewModel/model' passed must be a class with C# properties, where they will be passed as props into your react component");

        if (tagName == null && renderServerOnly == false)
            throw new Exception("TagName can only be null if the component is only server side rendered, renderServerOnly is set to false");

        if (renderServerOnly && tagName == null && cssClass != null)
            throw new Exception("ServerSideOnly is set, without a tag, you cannot add a cssClass. Remove the param cssClass and rather add it to your react code");

        if (type.IsListOrArray())
            throw new Exception("'viewModel/model' passed cannot be an array or a list. It must be a 'viewModel' or 'model' with properties. Surround the array/list with a class is one way to fix this.");

        if (tagName?.StartsWith("<") == true)
            throw new Exception("'tagName' should not include < > characters");

        if (tagName == null && renderClientOnly)
            throw new Exception("'tagName' must be included when using 'renderClientOnly', as it is used to keep track of the initial properties. Please set it, for instance: 'div'");

        if (tagName != null && tagName != "div" && tagName != "article" && tagName != "section" && tagName != "header" && tagName != "footer")
            throw new Exception("'tagName' must be either null, div, section, header, article or footer");

        if (additionalProps != null && !additionalProps.GetOriginalType().IsClass)
            throw new Exception("'additionalProps' passed must be a class with C# properties, where they will be passed as props into your react component. If an 'additional property' matches a property in the model, then the 'additional property' is overwriting it");

        if (renderServerOnly && renderClientOnly)
            throw new Exception("You cannot render 'client only' and 'server side only', it doesnt make any sense. Choose either client or server side or let both be false, to render on both sides: " + type.Name);

        return type;
    }
}