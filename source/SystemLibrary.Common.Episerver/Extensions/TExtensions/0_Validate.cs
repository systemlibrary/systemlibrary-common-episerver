using EPiServer;

using SystemLibrary.Common.Net.Extensions;


namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void Validate(object propsModel, object additionalProps = null, string tagName = "div", bool renderClientOnly = false, bool renderServerOnly = false)
    {
        var type = propsModel.GetOriginalType();

        if (!type.IsClass)
            throw new Exception("'viewModel/model' passed must be a class with C# properties, where they will be passed as props into your react component");

        if (type.IsListOrArray())
            throw new Exception("'viewModel/model' passed cannot be an array or a list. It must be a 'viewModel' or 'model' with properties. Surround the array/list with a class is one way to fix this.");

        if (tagName?.StartsWith("<") == true)
            throw new Exception("'tagName' should not include < > characters");

        if (tagName.IsNot() && renderClientOnly)
            throw new Exception("'tagName' must be included when using 'renderClientOnly', as it is used to keep track of the initial properties. Please set it, for instance: 'div'");

        if (tagName != null && tagName != "div" && tagName != "article" && tagName != "section" && tagName != "header" && tagName != "footer")
            throw new Exception("'tagName' must be either null, div, section, header, article or footer");

        if (additionalProps != null && !additionalProps.GetOriginalType().IsClass)
            throw new Exception("'additionalProps' passed must be a class with C# properties, where they will be passed as props into your react component. If an 'additional property' matches a property in the model, then the 'additional property' is overwriting it");

        if (renderServerOnly && renderClientOnly)
            throw new Exception("You cannot render 'client only' and 'server side only', it doesnt make any sense. Choose either client or server side or let both be false, to render on both sides: " + type.Name);
    }
}