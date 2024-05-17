using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Attributes;

/// <summary>
/// Add Json Edit attribute to a virtual string property to activate the simple Json Editor
/// </summary>
/// <example>
/// <code>
/// public class StartPage : PageData 
/// {
///     [JsonEdit(Title = "Car Selection", Type = typeof(JsonCar),  SortByPropertyName2 = nameof(JsonCar.Age))]
///     public virtual string Cars { get; set; }
/// }
/// 
/// public class Car
/// {
///     [Required(ErrorMessage = "This shows as 'orange-red-ish' below the input field and marks the field required with a *")]
///     [Display(Name = "Override 'Name' as the display name", Description = "This shows up as the placeholder of the input field")]
///     public string Name { get; set; }
///     
///     [Display(Name = "Cars Age")]
///     public int Age { get; set; } 
///     
///     [Display(Name = "Is it sold?")]
///     public bool IsSold { get; set;}
///     
///     [Required]
///     public DateTime Created { get; set; }
/// }
/// </code>
/// 
/// Index.cshtml
/// <code>
/// var cars = @Model.CurrentPage.Cars.Json&lt;List&lt;Car&gt;&gt;();
/// 
/// &lt;Car count: @cars.Count&gt;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class JsonEditAttribute : Attribute, IDisplayMetadataProvider
{
    /// <summary>
    /// The c# class that is stored as a json string, and all its properties is editable through the Json Editor
    /// </summary>
    public Type Type { get; set; }

    /// <summary>
    /// Optional: Give the Json Editor Window a simple title so its more editor friendly
    /// 
    /// If not filled out - the 'Type.Name' will be used as the title of the Json Editor Window
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Optional: Set to name of a property that the Editor can sort by
    /// 
    /// If empty: the sort button 1 is hidden inside the 'Json Editor Window'
    /// </summary>
    /// <example>
    /// Sorting by 'Age' the C# property name, and the friendly name "First Name":
    /// <code>
    /// public class Car
    /// {
    ///     [Display(Name = "First Name")]
    ///     public string FirstName { get; set; }
    ///     public int Age { get; set; }
    /// }
    /// 
    /// public class StartPage : PageData
    /// {
    ///     [JsonEdit(Type = typeof(Car), SortByPropertyName1 = "First Name", SortByPropertyName2 = nameof(Car.Age))]
    ///     public virtual string Cars { get; set ; }
    /// }
    /// </code>
    /// </example>
    public string SortByPropertyName1 { get; set; }

    /// <summary>
    /// Optional: Set to name of a property that the Editor can sort by
    /// 
    /// If empty: the sort button 2 is hidden inside the 'Json Editor Window'
    /// </summary>
    /// <example>
    /// Sorting by 'Age' the C# property name, and the friendly name "First Name":
    /// <code>
    /// public class Car
    /// {
    ///     [Display(Name = "First Name")]
    ///     public string FirstName { get; set; }
    ///     public int Age { get; set; }
    /// }
    /// 
    /// public class StartPage : PageData
    /// {
    ///     [JsonEdit(Type = typeof(Car), SortByPropertyName1 = "First Name", SortByPropertyName2 = nameof(Car.Age))]
    ///     public virtual string Cars { get; set ; }
    /// }
    /// </code>
    /// </example>
    public string SortByPropertyName2 { get; set; }

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
                    if (Type == null || !Type.IsClass)
                        throw new Exception(nameof(JsonEditAttribute) + " must have a Type set in its declaration of the attribute for property: " + extendedMetadata.PropertyName);

                    extendedMetadata.SelectionFactoryType = typeof(JsonEditFactory);
                    extendedMetadata.ClientEditingClass = Globals.AreaCms + "/JsonEdit/" + nameof(JsonEditController.Script);
                    extendedMetadata.UIHint = Globals.AreaCms + "/JsonEdit";
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
