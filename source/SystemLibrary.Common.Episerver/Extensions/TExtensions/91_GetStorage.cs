using System.Collections.Concurrent;

using SystemLibrary.Common.Framework;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static ConcurrentDictionary<string, bool> GetSsrIdStore(bool renderClientSide)
    {
        var storage = HttpContextInstance.Current.Items;

        if (storage == null || !renderClientSide) return null;

        var dictionary = storage[SysLibStorageSsrId] as ConcurrentDictionary<string, bool>;

        if (dictionary == null)
        {
            // NOTE: Might need to lock:
            //var newDict = new ConcurrentDictionary...
            //dictionary = Interlocked.CompareExchange(ref storage[SysLibStorageSsrId], newDict, null) as ConcurrentDictionary<string, bool>;
            //if (dictionary == null)
            //{
            //    dictionary = newDict; // The first thread will create the dictionary
            //}

            dictionary = new ConcurrentDictionary<string, bool>();
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