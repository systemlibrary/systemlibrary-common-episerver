namespace SystemLibrary.Common.Episerver.Cms.Properties;

/// <summary>
/// Property type to display 'parent link' inside the CMS
/// </summary>
/// <example>
/// <code>
/// public class StartPage
/// {
///     [Display(Name = "Parent link",
///         Description = "A url to the parent where this content lives",
///         GroupName = SystemTabNames.Settings, //Adding it to "Settings" tab in Epi
///         Order = 10001)]
///     public virtual ParentLinkReference Ref {get;set;}
/// }
/// </code>
/// </example>
public class ParentLinkReference
{
}
