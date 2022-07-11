using System;
using System.Collections.Generic;

using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

namespace SystemLibrary.Common.Episerver.Cms.EditorDescriptors;

[EditorDescriptorRegistration(TargetType = typeof(CategoryList))]
public class HideCategoryListDescriptor : EditorDescriptor
{
    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        if (Initialize.ServiceCollectionExtensions.Options?.HidePropertyCategoryList == true)
        {
            base.ModifyMetadata(metadata, attributes);
            if (metadata.PropertyName == "icategorizable_category")
            {
                metadata.ShowForEdit = false;
            }
        }
    }
}

