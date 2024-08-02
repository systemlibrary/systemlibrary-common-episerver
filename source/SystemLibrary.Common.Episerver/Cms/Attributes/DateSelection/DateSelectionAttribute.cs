using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Attributes;

/// <summary>
/// Add Date Selection attribute to a public virtual DateTime property to activate a 'date picker', it will hide the 'time selection'
/// </summary>
/// <example>
/// <code>
/// public class StartPage : PageData 
/// {
///     [DateSelection()]
///     public virtual DateTime Expires { get; set; }
///
///     [DateSelection()]
///     public virtual DateTime? Expires { get; set; }
/// }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class DateSelectionAttribute : Attribute, IDisplayMetadataProvider
{
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
                    extendedMetadata.ClientEditingClass = "dijit/form/DateTextBox";
                    extendedMetadata.EditorConfiguration.Add("className", "dijitReset dijitInputInner systemlibrary-common-episerver-date-property-input");
                    extendedMetadata.EditorConfiguration.Add("class", "systemlibrary-common-episerver-date-property");
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
