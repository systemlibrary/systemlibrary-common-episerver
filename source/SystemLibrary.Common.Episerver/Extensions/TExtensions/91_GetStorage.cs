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
    static HashSet<string> GetHashSet( bool renderClientSide)
    {
        if (!renderClientSide) return null;

        var storage = HttpContextInstance.Current?.Items;

        HashSet<string> keys = null;
        if (storage != null)
        {
            keys = storage[SysLibComponentKeys] as HashSet<string>;

            if (keys == null)
            {
                keys = new HashSet<string>();

                // NOTE: in rare async scenarios, between null and adding the keys (creating the hashset), another thread might also create a HashSet
                // but that second hashSet during creation, the first thread (hopefully) manages to set it, so it is not null after creation
                // We do not want to use a lock here for time being
                if (storage[SysLibComponentKeys] == null)
                {
                    storage[SysLibComponentKeys] = keys;
                }
                else
                    keys = storage[SysLibComponentKeys] as HashSet<string>;
            }
        }

        return keys;
    }
}