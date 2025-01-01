using System.Collections.Concurrent;
using System.Text;
using System.Web;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static void AppendHiddenInput(int level, string ssrId, string componentFullName, string jsonProps, ConcurrentDictionary<string, bool> ssrIdStore, StringBuilder root)
    {
        if (ssrId == null) return;

        if (level == -9) return;

        var storage = HttpContextInstance.Current?.Items;

        if (storage == null)
        {
            root.Append($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />");

            return;
        }

        // hidden input for the ssrId already printed
        if (ssrIdStore.ContainsKey(ssrId))
        {
            Debug.Log("hidden input already printed for ssr-id " + ssrId);
            return;
        }

        ssrIdStore.TryAdd(ssrId, true);

        // NOTE:
        // In heavy aync scenarios: first thread sees storage as null, a second thread comes before first thread has set the storage
        // leaving both threads with null and wanting to create a new storage (dictionary)
        // After creating the dictionary, we check the store again, if still null, we set it
        // For time being we do not want a lock here
        var dictionary = storage[SysLibStorageHiddenInputs] as ConcurrentDictionary<string, StringBuilder>;

        if (dictionary == null)
        {
            dictionary = new ConcurrentDictionary<string, StringBuilder>();

            if (storage[SysLibStorageHiddenInputs] == null)
            {
                storage[SysLibStorageHiddenInputs] = dictionary;
            }
            else
            {
                dictionary = storage[SysLibStorageHiddenInputs] as ConcurrentDictionary<string, StringBuilder>;
            }
        }

        if (level == 0)
        {
            // All hidden inputs within "first parent component", is now appended to the bottom in the DOM
            if (dictionary.ContainsKey(SysLibComponentStorageKey))
            {
                root.Append(dictionary[SysLibComponentStorageKey]);
            }

            root.Append($"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />");
            return;
        }

        // Adding or updating the string builder containing all inputs of type hidden
        var hidden = $"<input type='hidden' id=\"" + ssrId + $"\" data-rcssr=\"{componentFullName}\" data-rcssr-props=\"{HttpUtility.HtmlAttributeEncode(jsonProps)}\" />";

        dictionary.AddOrUpdate(
            SysLibComponentStorageKey,
            new StringBuilder(hidden),
            (key, oldValue) => oldValue.Append(hidden)
        );
    }
}