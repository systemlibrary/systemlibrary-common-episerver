using System;

using EPiServer.Core;
using EPiServer.PlugIn;

namespace SystemLibrary.Common.Episerver.Properties;

[PropertyDefinitionTypePlugIn(DisplayName = "[SystemLibrary] Message",
    Description = "This property display an information message that shows only in property view",
    GUID = "F9632D08-653D-4C29-BD0B-E121E91D22D0")]
public class MessageProperty : PropertyLongString
{
    public override Type PropertyValueType
    {
        get
        {
            return typeof(Message);
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
