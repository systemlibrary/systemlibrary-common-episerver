using System.Collections.Concurrent;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    const string SysLibComponentStorageKey = "1";

    static int IncrementLevel(bool renderClientSide)
    {
        if (!renderClientSide) return -9;

        var storage = HttpContextInstance.Current?.Items;

        if (storage == null) return -9;

        var dictionary = storage[SysLibStorageLevel] as ConcurrentDictionary<string, int>;

        if (dictionary == null)
        {
            dictionary = new ConcurrentDictionary<string, int>();
            // NOTE: in rare async scenarios, between null check above and the setting of dictionary below, there might be two threads creating the dictionary
            // The first that finishes the creation is then the one being used
            // Still a small chance to error, both threads check for null at same time, both becomes true, both sets...
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