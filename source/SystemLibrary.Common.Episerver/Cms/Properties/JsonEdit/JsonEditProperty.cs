using System;

using EPiServer.Core;
using EPiServer.PlugIn;

namespace SystemLibrary.Common.Episerver.Cms
{
    [PropertyDefinitionTypePlugIn(
        DisplayName = "[SystemLibrary] Json Edit",
        Description = "This property is a json editor for T",
        GUID = "5C0EA1CA-1BDD-47F6-A51B-0DC864383D45")]
    public class JsonEditProperty: PropertyLongString
    {
        public override Type PropertyValueType
        {
            get
            {
                return typeof(IJsonEdit);
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
}
