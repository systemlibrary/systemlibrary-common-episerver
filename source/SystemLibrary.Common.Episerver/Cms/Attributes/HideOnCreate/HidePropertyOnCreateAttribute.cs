namespace SystemLibrary.Common.Episerver.Attributes;

// CREDS TO: https://www.david-tec.com/2017/07/hiding-required-properties-on-the-create-new-page-in-episerver/
/// <summary>
/// Hide property on create attribute
/// <para>This hides properties in the on-create dialog, even if the property is required</para>
/// </summary>
/// <example>
/// This hides the required title property on creation
/// <code>
/// [HidePropertyOnCreate]
/// [Display(Name = "Title")]
/// [Required]
/// public virtual string Title {get;set;}
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class HidePropertyOnCreateAttribute : Attribute { }
