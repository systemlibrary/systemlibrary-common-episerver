using System;
using System.Collections.Generic;

using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.HideCategoryList;

[EditorDescriptorRegistration(TargetType = typeof(CategoryList))]
public class HideCategoryListDescriptor : EditorDescriptor
{
    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        if (ServiceCollectionExtensions.Options?.HidePropertyCategoryList == true)
        {
            base.ModifyMetadata(metadata, attributes);
            if (metadata.PropertyName == "icategorizable_category")
            {
                metadata.ShowForEdit = false;
            }
        }
    }
}

