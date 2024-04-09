using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using EPiServer.Logging;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Microsoft.Identity.Client;

using React;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    const string SysLibComponentStorageKey = "1";

    static int IncrementLevel(bool renderClientSide)
    {
        var storage = HttpContextInstance.Current?.Items;

        if (storage == null || !renderClientSide) return -999;

        var dictionary = storage[SysLibStorageLevel] as ConcurrentDictionary<string, int>;

        if (dictionary == null)
        {
            dictionary = new ConcurrentDictionary<string, int>();
            // NOTE: in rare async scenarios, between null and adding the keys (creating the hashset), another thread might also create a HashSet
            // but that second hashSet during creation, the first thread (hopefully) manages to set it, so it is not null after creation
            // We do not want to use a lock here for time being
            if (storage[SysLibStorageLevel] == null)
            {
                storage[SysLibStorageLevel] = dictionary;
            }
            else
                dictionary = storage[SysLibStorageLevel] as ConcurrentDictionary<string, int>;
        }

        dictionary.AddOrUpdate(SysLibComponentStorageKey, 1, (key, oldValue) => oldValue + 1);

        return dictionary[SysLibComponentStorageKey];
    }
}