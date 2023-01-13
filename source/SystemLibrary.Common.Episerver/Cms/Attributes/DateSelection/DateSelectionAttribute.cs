using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

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
                    //dijit dijitReset dijitInline dijitLeft dijitTextBox dijitComboBox dijitDateTextBox dijitValidationTextBox
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
