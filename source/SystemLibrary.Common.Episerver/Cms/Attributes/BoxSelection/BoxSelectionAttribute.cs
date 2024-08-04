using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Attributes;

/// <summary>
/// Show a selection of items in boxes, where one can select one or more boxes
/// <para>1. Create a simple color picker with 3-4-5 colors that Editors can choose from?</para>
/// 2. Choose between a few choices instead of radiobuttons/dropdownlist/checkboxes?
/// </summary>
/// <example>
/// Let's assume we got one enum with colors:
/// <code>
/// enum ColorSelection {
///     None,
///     [EnumValue("#fff")]
///     White,
///     [EnumValue("#bbb")]
///     Grey,
///     [EnumValue("#000")]
///     Black,
/// }
/// </code>
/// 
/// Select one color:
/// <code class="language-csharp hljs">
/// [Display(Name = "Background Color")]
/// [BoxSelection()]
/// public virtual ColorSelection Color { get; set; }
/// </code>
/// 
/// Select one color, but hide some:
/// <code class="language-csharp hljs">
/// [Display(Name = "Background Color")]
/// [BoxSelection(Hide = ColorSelection.Grey)]
/// public virtual ColorSelection Color { get; set; }
/// </code> 
/// 
/// Select one color, but only among the ones we show:
/// <code class="language-csharp hljs">
/// [Display(Name = "Background Color")]
/// [BoxSelection(Show = new object[] { ColorSelection.Grey, ColorSelection.Black} )]
/// public virtual ColorSelection Color { get; set; }
/// </code> 
/// 
/// Select many colors, but only among the ones we show:
/// <code class="language-csharp hljs">
/// [Display(Name = "Background Color")]
/// [BoxSelection(Show = new object[] { ColorSelection.Grey, ColorSelection.Black} )]
/// public virtual IList&lt;string&gt; Colors { get; set; }
/// 
/// //Note: Property is now a list of strings, so to get the Enum simply call Colors[0].ToEnum&lt;ColorSelection&gt;()
/// </code> 
/// 
/// Select many colors, but only among the ones we show and prevent unselecting all boxes:
/// <code class="language-csharp hljs">
/// [Display(Name = "Background Color")]
/// [BoxSelection(Show = new object[] { ColorSelection.Grey, ColorSelection.Black }, AllowUnselection = false )]
/// public virtual IList&lt;string&gt; Colors { get; set; }
/// 
/// //Note: Property is now a list of strings, so to get the Enum simply call Colors[0].ToEnum&lt;ColorSelection&gt;()
/// </code> 
/// 
/// Example with built-in FontAwesome Icons:
/// <code>
/// enum ProductSelection 
/// {
///   [EnumValue(FontAwesomeSolid.hourglass)]
///   Product1,
///   [EnumValue(FontAwesomeSolid.house)]
///   Product2
/// }
/// 
/// [BoxSelection()]
/// public virtual ProductSelection Product { get; set; }
/// </code>
/// 
/// Example with custom images:
/// <code>
/// enum ImageSelection 
/// {
///  [EnumValue("")]
///  Unset,
/// 
///  [EnumValue("/static/images/blog-image.png")]
///  [EnumText("")]
///  Image1,
/// 
///  [EnumValue("/static/images/article-image.png")]
///  [EnumText("Optional: Text above image")]
///  Image2
/// }
/// 
/// [BoxSelection(EnumType = typeof(ImageSelection))]
/// public virtual string ImagePath { get; set; }
/// //Optional: public virtual ImageSelection ImageSelected { get; set; }
/// //If you are interested in the 'EnumValue' (url path) then chose 'string'
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class BoxSelectionAttribute : Attribute, IDisplayMetadataProvider
{
    /// <summary>
    /// Option to set your own selection factory instead of the built-in one
    /// </summary>
    public virtual Type SelectionFactoryType { get; set; }

    /// <summary>
    /// Specifically set the Enum Type to be the Selection
    /// <para>Not needed to be set if the EnumType is the property type, as in 'public virtual SomeEnum ...'</para>
    /// </summary>
    public Type EnumType { get; set; }

    /// <summary>
    /// Hide a range of items
    /// <para>By default all items are shown, unless explicit setting Hide or Show</para>
    /// <para>Hide can either be one value, or a new object[] { ... }</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum Colors {
    ///     None,
    ///     White,
    ///     Grey,
    ///     Black,
    /// }
    /// 
    /// 
    /// Show = Color.White //would then show only one item to be select, white
    /// 
    /// Show = new object[] { Color.White, Color.Grey, Color.Black} //would show 3 items, would not show 'none'
    /// 
    /// Hide = Color.Grey //would hide only grey, show all other options
    /// 
    /// Hide = new object[] { Color.White, Color.Black } //would show only None and Grey
    /// </code>
    /// </example>
    public object Hide { get; set; }

    /// <summary>
    /// Show a range of items
    /// <para>By default all items are shown, unless explicit setting Hide or Show</para>
    /// <para>Show can either be one value, or a new object[] { ... }</para>
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// enum Colors {
    ///     None,
    ///     White,
    ///     Grey,
    ///     Black,
    /// }
    /// 
    /// Show = Color.White //would then show only one item to be select, white
    /// 
    /// Show = new object[] { Color.White, Color.Grey, Color.Black} //would show 3 items, would not show 'none'
    /// 
    /// Hide = Color.Grey //would hide only grey, show all other options
    /// 
    /// Hide = new object[] { Color.White, Color.Black } //would show only None and Grey
    /// </code>
    /// </example>
    public object Show { get; set; }

    /// <summary>
    /// Allow unselection of the currently selected value, leaving all options to be deselected 
    /// <para>- if all items are deselected, and Content is Published, the default value is then stored in the property</para>
    /// <para>- An enum has a default integer of 0 usually, a string is null</para>
    /// </summary>
    public bool AllowUnselection { get; set; } = true;

    /// <summary>
    /// Show expired (removed/deleted) options from the Factory in the user interface for Editors
    /// <para>Items no longer existing, but are still selected in the DB on some properties, will then appear as 'Expired: ...'</para>
    /// Do note that if property is an Enum, and an Enum is never null, so the value stored is the INT, which then will still be sent to the 'View/Frontend' even though the Enum does not contain that number anymore
    /// </summary>
    /// <example>
    /// public enum Products { A, B, C }
    /// 
    /// A ProductBlock.cs has a property 
    /// <code>public virtual Products ProductSelected {get; set;} = Products.B;</code>
    /// // Product B is selected/stored in DB
    /// 
    /// Then we delete B as an option from the Enum, it's obsolete/not for sale anymore
    /// 
    /// Products now contain only A and C
    /// 
    /// With this option turned on, B is displayed as 'Expired: 1'
    /// - if property type is 'virtual string' it would show 'Expired: B' 
    /// </example>
    public bool ShowExpiredItems { get; set; } = true;

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var additionalMetadata = context?.DisplayMetadata?.AdditionalValues;

        if (additionalMetadata.IsNot()) return;

        try
        {
            foreach (var data in additionalMetadata)
            {
                if (data.Value is ExtendedMetadata extendedMetadata && extendedMetadata.PropertyName.Is())
                {
                    if (SelectionFactoryType == null)
                        extendedMetadata.SelectionFactoryType = typeof(BoxSelectionFactory);
                    else
                        extendedMetadata.SelectionFactoryType = SelectionFactoryType;

                    extendedMetadata.ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/BoxSelection/" + nameof(BoxSelectionController.Script);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
