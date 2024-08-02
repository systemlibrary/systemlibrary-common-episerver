namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    const string ViewModel = "ViewModel";
    const string ReactComponents = "reactComponents.";
    const string Model = "Model";
    static string GetComponentFullName(Type modelType, object model, string componentFullName)
    {
        if (componentFullName != null) return componentFullName;

        var name = modelType.Name;

        if (name[0] == '<')
            name = name.Replace("<>", "").Replace("`", "").Replace(" ", "");

        if (name[0] != 'V' && name != ViewModel && name.EndsWith(ViewModel, StringComparison.Ordinal))
            return ReactComponents + name.Substring(0, name.Length - ViewModel.Length);

        if (name[0] != 'M' && name != Model && name.EndsWith(Model, StringComparison.Ordinal))
            return ReactComponents + name.Substring(0, name.Length - Model.Length);

        return ReactComponents + name;
    }
}