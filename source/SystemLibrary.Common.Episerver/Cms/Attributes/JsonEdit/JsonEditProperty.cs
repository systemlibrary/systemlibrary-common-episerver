using System;

using EPiServer.Core;
using EPiServer.PlugIn;

using SystemLibrary.Common.Net;

namespace SystemLibrary.Common.Episerver.Cms.Attributes;

[PropertyDefinitionTypePlugIn(
    DisplayName = "[SystemLibrary] Json Edit",
    Description = "This property is a json editor for T",
    GUID = "5A4FA4DE-CACC-46F0-B83C-92570D6DE45F")]
public class JsonEditProperty : PropertyLongString
{
    public override Type PropertyValueType
    {
        get
        {
            return SystemType.StringType;
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
