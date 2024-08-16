using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Attributes;

/// <summary>
/// Select multiple values form either a Enum based selection factory, or a free text input field where editors can add custom texts as they please
/// <para>If this attribute is used on a IList&lt;string&gt; without an Enum Type, then it will work as a input field where editors write any text they want.</para>
/// Each string value is stored witha comma as the delimiter, if editors writes a comma, it is stored as '&#44;' the html entity which you can either display "raw html" or replace it back to a comma
/// </summary>
/// <remarks>
/// There's no current max limit on how many items you or editors can add to the dropdown, you can write so in the description field for editors if needed
/// <para>What this means: IF you want editors so select maximum 3 cars, say so in the Description of the Property</para>
/// </remarks>
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
    /// <summary>
    /// Option to set your own selection factory type that will be invoked instead of the built-in one
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

    public bool ShowExpiredItems { get; set; } = true;

    public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
    {
        var additionalValues = context?.DisplayMetadata?.AdditionalValues;

        if (additionalValues.IsNot()) return;

        try
        {
            foreach (var data in additionalValues)
            {
                // NOTE: Latest Optimizely suddenly invokes this multiple times, so checking the value and propertyName as propertyName exists when we want to override it
                if (data.Value is ExtendedMetadata metadata && metadata.PropertyName.Is())
                {
                    metadata.SelectionFactoryType = typeof(MultiDropdownSelectionFactory);

                    metadata.ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/MultiDropdownSelection/" + nameof(MultiDropdownSelectionController.Script);
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
