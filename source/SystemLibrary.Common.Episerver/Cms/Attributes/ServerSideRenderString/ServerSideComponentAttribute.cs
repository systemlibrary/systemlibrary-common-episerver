
namespace SystemLibrary.Common.Episerver.Attributes;

/// <summary>
/// Marks a property to be rendered as HTML with a hidden input storing its JSON props for client-side hydration.
/// <para>Usage: When serializing the Model to JSON, but a property on the Model should be rendered as HTMl server side, avoiding separate client-side prop passing.</para>
/// </summary>
/// <example>
/// <code>
/// [Display(Name = "Title")]
/// [ServerSideComponent]
/// public virtual ContentArea Title { get ;set; }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class ServerSideComponentAttribute : Attribute { }
