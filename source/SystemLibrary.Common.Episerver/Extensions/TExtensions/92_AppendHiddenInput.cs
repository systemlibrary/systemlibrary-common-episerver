using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using Microsoft.AspNetCore.Connections.Features;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using Microsoft.Identity.Client;

using React;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void AppendHiddenInput(int level, string ssrId, string componentFullName, string jsonProps, ConcurrentDictionary<string, bool> ssrIdStore, StringBuilder root)
    {
        if (ssrId == null) return;

        if (level == -9) return;

        var storage = HttpContextInstance.Current?.Items;

        if(storage == null && level == 0)
        {
            root.Append($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />");
            
            if (!Globals.IsUnitTesting && !EnvironmentConfig.IsProd)
            {
                Log.Debug(componentFullName + " serversiderendering append input to root as storage or level do not exist: " + level);
            }

            return;
        }

        var dictionary = storage[SysLibStorageHiddenInputs] as ConcurrentDictionary<string, StringBuilder>;

        if (dictionary == null)
        {
            dictionary = new ConcurrentDictionary<string, StringBuilder>();
            // NOTE: in rare async scenarios, between null and adding the keys (creating the hashset), another thread might also create a HashSet
            // but that second hashSet during creation, the first thread (hopefully) manages to set it, so it is not null after creation
            // We do not want to use a lock here for time being
            if (storage[SysLibStorageHiddenInputs] == null)
            {
                storage[SysLibStorageHiddenInputs] = dictionary;
            }
            else
                dictionary = storage[SysLibStorageHiddenInputs] as ConcurrentDictionary<string, StringBuilder>;
        }

        if (level == 0)
        {
            // All hidden inputs within "first parent component", is now appended to the bottom in the DOM
            if(dictionary.ContainsKey(SysLibComponentStorageKey))
                root.Append(dictionary[SysLibComponentStorageKey] as StringBuilder);

            root.Append($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />");

            if (!Globals.IsUnitTesting && !EnvironmentConfig.IsProd)
            {
                Log.Debug(componentFullName + " serversiderendering appended all inputs to root");
            }

            return;
        }

        // If ssr id input has already been printed, just return
        if (ssrIdStore.ContainsKey(ssrId))
        {
            if (!Globals.IsUnitTesting && !EnvironmentConfig.IsProd)
            {
                Log.Debug(componentFullName + " serversiderendering ssrid already printed");
            }
            return;
        }

        // Printing this input with the ssr id once
        ssrIdStore.TryAdd(ssrId, true);

        if (!Globals.IsUnitTesting && !EnvironmentConfig.IsProd)
        {
            Log.Debug(componentFullName + " serversiderendering input with ssrid " + ssrId + " is appended");
        }

        // Adding or updating the string builder containing all inputs of type hidden
        dictionary.AddOrUpdate(
            SysLibComponentStorageKey,
            new StringBuilder($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />"),
            (key, oldValue) => oldValue.Append($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />")
        );
    }
}