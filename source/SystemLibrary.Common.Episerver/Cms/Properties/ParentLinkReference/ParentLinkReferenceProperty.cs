using System;

using EPiServer.Core;
using EPiServer.PlugIn;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

[PropertyDefinitionTypePlugIn(
    DisplayName = "[SystemLibrary] Parent Link Reference",
    Description = "This property is a simple link to the parent content",
    GUID = "CBE9E1B9-89AC-41A2-9A8F-47B21F5849D4")]
public class ParentLinkReferenceProperty : PropertyLongString
{
    public override Type PropertyValueType
    {
        get
        {
            return typeof(ParentLinkReference);
        }
    }

    public override object Value
    {
        get
        {
            return null;
        }

        set
        {
        }
    }

    public override object SaveData(PropertyDataCollection properties)
    {
        return null;
    }
}
