﻿using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Microsoft.Identity.Client;

using React;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static ConcurrentDictionary<string, bool> GetSsrIdStore(bool renderClientSide)
    {
        var storage = HttpContextInstance.Current?.Items;

        if (storage == null || !renderClientSide) return null;

        var dictionary = storage[SysLibStorageSsrId] as ConcurrentDictionary<string, bool>;

        if (dictionary == null)
        {
            dictionary = new ConcurrentDictionary<string,bool>();
            // NOTE: in rare async scenarios, between null and adding the keys (creating the hashset), another thread might also create a HashSet
            // but that second hashSet during creation, the first thread (hopefully) manages to set it, so it is not null after creation
            // We do not want to use a lock here for time being
            if (storage[SysLibStorageSsrId] == null)
            {
                storage[SysLibStorageSsrId] = dictionary;
            }
            else
                dictionary = storage[SysLibStorageSsrId] as ConcurrentDictionary<string, bool>;
        }

        return dictionary;
    }
}