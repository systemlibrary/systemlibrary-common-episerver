//using System;

//using EPiServer;

//using Microsoft.AspNetCore.Mvc;

//namespace SystemLibrary.Common.Episerver.ActionResults;

//public abstract class BaseReactResult : ContentResult
//{
//    protected static string GetReactComponentFullName(object model, string componentFullName)
//    {
//        Type type = GetType(model);

//        if (componentFullName.Is()) return componentFullName;

//        var componentName = GetReactComponentName(type);

//        var componentFolder = GetGlobalThisVariablePath();

//        return componentFolder + "." + componentName;
//    }

//    static Type GetType(object model)
//    {
//        var type = model.GetOriginalType();

//        if (!type.IsClass)
//            throw new Exception("'viewModel/model' passed must be a class with C# properties, where they will be passed as props into your react component");

//        return type;
//    }

//    static string GetReactComponentName(Type type)
//    {
//        var name = type.Name;

//        if (name.EndsWith("ViewModel"))
//            return name.Substring(0, name.Length - "ViewModel".Length);

//        if (name.EndsWith("Model"))
//            return name.Substring(0, name.Length - "Model".Length);

//        return name;
//    }

//    static string GetGlobalThisVariablePath()
//    {
//        return "reactComponents";
//    }
//}

