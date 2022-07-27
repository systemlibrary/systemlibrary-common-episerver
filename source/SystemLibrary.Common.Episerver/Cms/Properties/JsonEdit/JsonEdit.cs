namespace SystemLibrary.Common.Episerver.Cms;

/// <summary>
/// Property type to display 'message' inside the CMS's property view
/// </summary>
/// <example>
/// <code>
/// public class StartPage
/// {
///     [Display(Name = "Json Editor for T",
///         Description = "Opens up a new window with a Json Editor for content stored in this property")]
///     public virtual JsonEdit&lt;T&gt; Info {get;set;}
/// }
/// </code>
/// </example>
public class JsonEdit<T> : IJsonEdit where T : class
{
}

public interface IJsonEdit
{
}
