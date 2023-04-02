using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using EPiServer.Core;
using EPiServer.Shell.ObjectEditing;
using EPiServer.Shell.ObjectEditing.EditorDescriptors;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Cms.Descriptors;

public abstract class BaseCustomPropertyDescriptor : EditorDescriptor
{
    public override void ModifyMetadata(ExtendedMetadata metadata, IEnumerable<Attribute> attributes)
    {
        if (AppSettings.Current.Edit.AllPropertiesShowPropertyDescriptions)
        {
            var displayAttribute = metadata.GetAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                if (displayAttribute?.Description.Is() == true)
                {
                    if (metadata.IsRequired && metadata.InitialValue == null)
                        return;

                    // metadata.DisplayName = "<span>" + metadata.DisplayName + "<div class='sysLibPropDesc'>" + displayAttribute.Description + "</div></span>";
                }
            }
        }
    }
}

[EditorDescriptorRegistration(TargetType = typeof(string))]
public class CustomStringPropertyDescriptor : BaseCustomPropertyDescriptor
{
}

[EditorDescriptorRegistration(TargetType = typeof(int))]
public class CustomIntPropertyDescriptor : BaseCustomPropertyDescriptor
{
}

[EditorDescriptorRegistration(TargetType = typeof(bool))]
public class CustomBoolPropertyDescriptor : BaseCustomPropertyDescriptor
{
}

[EditorDescriptorRegistration(TargetType = typeof(XhtmlString))]
public class CustomXhtmlStringPropertyDescriptor : BaseCustomPropertyDescriptor
{
}

[EditorDescriptorRegistration(TargetType = typeof(DateTime))]
public class CustomDateTimePropertyDescriptor : BaseCustomPropertyDescriptor
{
}

[EditorDescriptorRegistration(TargetType = typeof(Enum))]
public class CustomEnumPropertyDescriptor : BaseCustomPropertyDescriptor
{
}


//Required properties suddenly shows the "metadata" text...
public class DefaultValidator : Validator<IContentData>
{
    public override void OnValidate(IContentData content)
    {
    }
}