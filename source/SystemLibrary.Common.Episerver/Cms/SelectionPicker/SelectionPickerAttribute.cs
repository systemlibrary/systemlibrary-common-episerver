using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class SelectionPickerAttribute : Attribute, IDisplayMetadataProvider
{
    public virtual Type SelectionFactoryType { get; set; }

    public Type Type { get; set; }

    /// <summary>
    /// Hide a range of items
    /// </summary>
    /// <example>
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
    /// </example>
    public object Hide { get; set; }

    /// <summary>
    /// Show a range of items
    /// </summary>
    /// <example>
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
    /// </example>
    public object Show { get; set; }

    /// <summary>
    /// Allow a current selected value to be unselected if clicked on, as a result the property's 'default' value is stored into the property if no items are selected
    /// </summary>
    public bool AllowUnselection { get; set; } = true;

    /// <summary>
    /// Show expired items in the list is default on
    ///
    /// Control wether or not values that are no longer selectable, but is stored in a property, shows as 'Expired: ...' or not
    /// 
    /// Note: If the property is of type Enum, an Enum is never null, and the value stored is the int of the Enum, so it will always return, even if the 'Enum Value' has 'Expired'
    /// 
    /// For instance: if you have an enum Products { 1, 2, 3 }
    /// Then a ProductBlock has selected Products.2 
    /// Then later on you remove the Products.2 from code, the Products.2 is still returned to the View of the ProductBlock as an Int, as it is still stored in the propertys value
    /// 
    /// In the above scenario, with ShowExpiredItems set to true, it would display 'Expired: 2' for the Products.2 value for the ProductBlock
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
                    extendedMetadata.SelectionFactoryType = typeof(SelectionPickerFactory);
                    extendedMetadata.ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/SelectionPicker/" + nameof(SelectionPickerController.Script);
                    //extendedMetadata.UIHint = "SystemLibrary.Common.Episerver.SelectionPicker.SelectionPickerEditor";
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Warning(ex);
        }
    }
}
