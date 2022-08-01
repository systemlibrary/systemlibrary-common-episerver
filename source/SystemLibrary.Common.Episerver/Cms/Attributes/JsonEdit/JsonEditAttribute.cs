using System;

using EPiServer.Shell.ObjectEditing;

using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class JsonEditAttribute : Attribute, IDisplayMetadataProvider
{
    public const string UiHint = "SystemLibrary.Common.Episerver.UiHint.JsonEdit";

    public Type Type { get; set; }

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
