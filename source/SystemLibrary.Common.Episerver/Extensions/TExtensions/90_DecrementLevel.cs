using System.Collections.Concurrent;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static int DecrementLevel(bool renderClientSide)
    {
        var storage = HttpContextInstance.Current?.Items;

        if (storage == null || !renderClientSide) return -9;

        var dictionary = storage[SysLibStorageLevel] as ConcurrentDictionary<string, int>;

        dictionary.AddOrUpdate(SysLibComponentStorageKey, 0, (key, oldValue) => oldValue - 1);

        return dictionary[SysLibComponentStorageKey];
    }
}