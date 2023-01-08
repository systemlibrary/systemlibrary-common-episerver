﻿using SystemLibrary.Common.Episerver.Cms.Properties;

namespace SystemLibrary.Common.Episerver;

public class PropertiesConfiguration
{
    public MessageConfiguration Message { get; set; }

    public PropertiesConfiguration()
    {
        Message = new MessageConfiguration();
    }
}
