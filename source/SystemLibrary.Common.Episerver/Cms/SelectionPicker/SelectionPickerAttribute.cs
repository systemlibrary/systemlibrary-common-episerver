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

    public object Hide { get; set; }

    public object Show { get; set; }

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
                    //extendedMetadata.UIHint = "SystemLibrary.Common.Episerver.SelectionPicker.ColorPickerEditor";
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
