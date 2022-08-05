using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

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
/// var cars = @Model.CurrentPage.Cars.ToJson&lt;Car&gt;();
/// 
/// &lt;Car count: @cars.Count&gt;
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class JsonEditAttribute : Attribute, IDisplayMetadataProvider
{
    public const string UiHint = "SystemLibrary.Common.Episerver.UiHint.JsonEdit";

    public Type Type { get; set; }

    public string Title { get; set; }

    public string SortByPropertyName1 { get; set; }
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
                    extendedMetadata.ClientEditingClass = "/SystemLibrary/Common/Episerver/UiHint/JsonEdit/" + nameof(JsonEditController.Script);
                    extendedMetadata.UIHint = UiHint;
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
