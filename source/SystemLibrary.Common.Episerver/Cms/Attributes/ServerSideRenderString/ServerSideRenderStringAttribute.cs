
namespace SystemLibrary.Common.Episerver.Attributes;

/// <summary>
/// Force this content area to be rendered as String when a "parent" ContentArea is rendered as "Props"
/// </summary>
/// <example>
/// <code>
/// [Display(Name = "Title")]
/// [ServerSideRenderString]
/// public virtual ContentArea Title {get;set;}
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ServerSideRenderStringAttribute : Attribute { }
