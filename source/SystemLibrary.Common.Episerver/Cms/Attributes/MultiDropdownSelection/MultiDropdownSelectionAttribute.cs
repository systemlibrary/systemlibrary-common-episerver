﻿using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

/// <summary>
/// Select multiple values form either a Enum based selection factory, or a free text input field where editors can add custom texts as they please
/// 
/// Note: 
/// If this attribute is used on a IList&lt;string&gt; without an Enum Type, then it will work as a input field where editors write any text they want.
/// - Each string value is stored witha comma as the delimiter, if editors writes a comma, it is stored as '&#44;' the html entity which you can either display "raw html" or replace it back to a comma
/// 
/// There's no current max limit on how many items you might add, you can write so in the description field for editors if needed
/// </summary>
/// <example>
/// <code>
/// public class StartPage : PageData 
/// {
///     [MultiDropdownSelection]
///     public virtual IList&lt;string&gt; Cars { get; set; }   
///     //Editors can now write any cars to the list, free text input field
///     
///     [MultiDropdownSelection(EnumType = typeof(CarSelection))]
///     public virtual IList&lt;string&gt; Cars { get; set; }   
///     //Editors chooses among all cars in the 'CarSelection' enum, a car can only be selected once
///     
///     [MultiDropdownSelection(EnumType = typeof(CarSelection), Hide = MultiSelect.BlackCar)]
///     public virtual IList&lt;CarSelection&gt; Cars { get; set; }   
///     //Editors chooses among all cars stored as a list of the Enum itself, except black car as it is hidden (not selectable), a car can only be selected once
/// }
/// 
/// enum CarSelection
/// {
///     RedCar,
///     YellowCar,
///     BlackCar,
///     WhiteCar,
///     [EnumText("Blue car")] //Shown in the selection instaed of 'BlueCar'
///     BlueCar,
///     [EnumText("Green car")] //Shown in the selection instead of'GreenCar'
///     GreenCar
/// }
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class MultiDropdownSelectionAttribute : Attribute, IDisplayMetadataProvider
{
    public virtual Type SelectionFactoryType { get; set; }

    public Type EnumType { get; set; }

    public object Hide { get; set; }

    public object Show { get; set; }

    public bool ShowExpiredItems { get; set; } = true;

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var additionalValues = context?.DisplayMetadata?.AdditionalValues;

        if (additionalValues.IsNot()) return;

        foreach (var data in additionalValues)
        {
            // NOTE: Optimizely keeps fucking up, they always break things, someone should get fired...
            // This does not do anything, the Factory is running multiple times, even though attribute exists only once
            // Must check on "PropertyName" to avoid calling factory twice...
            // and setting ClientEditingClass here, do nada
            if (data.Value is ExtendedMetadata extendedMetadata && extendedMetadata.PropertyName.Is())
            {
                extendedMetadata.SelectionFactoryType = typeof(MultiDropdownSelectionFactory);
                extendedMetadata.ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/MultiDropdownSelection/" + nameof(MultiDropdownSelectionController.Script);
                break;
            }
        }
    }
}
