using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

/// <summary>
/// Show a selection of items in boxes, where one can select one or more boxes
/// 
/// 1. Create a simple color picker with 3-4-5 colors that Editors can choose from?
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
    public virtual Type SelectionFactoryType { get; set; }

    public Type EnumType { get; set; }

    /// <summary>
    /// Hide a range of items
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
    /// Allow a current selected value to be unselected if clicked on, as a result the property's 'default' value is stored into the property if no items are selected
    /// </summary>
    public bool AllowUnselection { get; set; } = true;

    /// <summary>
    /// Control wether or not values that are no longer selectable, but tehir value is still stored in a property, will show up as 'Expired: ...'
    /// 
    /// Note: If property type is Enum, an Enum is never null, so value stored is the int of the Enum, which will always be sent to the View even though you've removed the Enum Key
    /// 
    /// Example:
    /// public enum Products { A, B, C }
    /// 
    /// A product block has stored "Products.B"
    /// 
    /// You then delete B from the enum, ending up with only A and C
    /// 
    /// This turned on will then display 'Expired: 1' as 1 is the int of B
    /// - if property is a string, it would then show the Value of B, which in our scenario would be 'Expired: B'
    /// </summary>
    public bool ShowExpiredItems { get; set; } = true;

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var additionalMetadata = context?.DisplayMetadata?.AdditionalValues;

        if (additionalMetadata.IsNot()) return;

        try
        {
            foreach (var data in additionalMetadata)
            {
                if (data.Value is ExtendedMetadata extendedMetadata)
                {
                    extendedMetadata.SelectionFactoryType = typeof(BoxSelectionFactory);
                    extendedMetadata.ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/BoxSelection/" + nameof(BoxSelectionController.Script);
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
