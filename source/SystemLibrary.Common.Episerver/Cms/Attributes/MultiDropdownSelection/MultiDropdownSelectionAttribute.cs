using System;
using System.Collections.Generic;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Episerver.Cms.Abstract;
using SystemLibrary.Common.Net;
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
            // NOTE: Latest Optimizely suddenly invokes this multiple times, so checking the value and propertyName as propertyName is null in irrelevant occasions
            if (data.Value is ExtendedMetadata metadata && metadata.PropertyName.Is())
            {
                var propertyType = metadata.ModelType;

                var propertyListType = BaseMultiSelectionFactory.GetGenericType(propertyType);

                if (propertyListType == null)
                    throw new Exception("Property " + metadata.PropertyName + ": Must be of type IList<string> or IList<Enum> (Enum is your own custom type 'public enum Colors ...'");

                if (propertyListType != SystemType.StringType && !propertyListType.IsEnum)
                    throw new Exception("Property " + metadata.PropertyName + ": Must be of type IList with either String or Enum");

                if (EnumType != null && !EnumType.IsEnum)
                    throw new Exception("Property " + metadata.PropertyName + ": EnumType is filled in the attribute, but the type is not an Enum");
                
                var multiDropdownStoreOptions = new List<ISelectItem>();

                BaseMultiSelectionFactory.PopulateSelectionItems(multiDropdownStoreOptions, this, propertyListType, metadata);

                var multiDropdownSelectionSaveString = propertyListType == SystemType.StringType;

                var multiDropdownSelectionDoFilter = propertyListType.IsEnum && (SelectionFactoryType != null || (EnumType != null && EnumType != propertyListType));

                metadata.EditorConfiguration.Add(nameof(multiDropdownSelectionDoFilter), multiDropdownSelectionDoFilter);

                metadata.EditorConfiguration.Add(nameof(multiDropdownSelectionSaveString), multiDropdownSelectionSaveString);

                metadata.EditorConfiguration.Add("multiDropdownStoreOptions", multiDropdownStoreOptions);

                metadata.EditorConfiguration.Add("multiDropdownShowExpiredItems", ShowExpiredItems);

                metadata.SelectionFactoryType = SelectionFactoryType ?? typeof(MultiDropdownSelectionFactory);

                metadata.ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/MultiDropdownSelection/" + nameof(MultiDropdownSelectionController.Script);
                break;
            }
        }
    }
}
