namespace SystemLibrary.Common.Episerver.Cms;

/// <summary>
/// Property type to display 'message' inside the CMS's property view
/// </summary>
/// <example>
/// <code>
/// public class StartPage
/// {
///     [Display(Name = "Note",
///         Description = "This message appears in the property view's mode, by default its shown in its whole, but can enable toggle functionality, which by default hides the information, but shows the title")]
///     public virtual Message Info {get;set;}
/// }
/// </code>
/// </example>
public class Message
{
}
