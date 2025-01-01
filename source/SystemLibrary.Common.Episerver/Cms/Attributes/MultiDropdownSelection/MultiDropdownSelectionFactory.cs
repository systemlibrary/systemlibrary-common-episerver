using System.Reflection;

using EPiServer.Shell.ObjectEditing;

using SystemLibrary.Common.Episerver.Abstract;
using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Attributes;

public class MultiDropdownSelectionFactory : BaseMultiSelectionFactory, ISelectionFactory
{
    public IEnumerable<ISelectItem> GetSelections(ExtendedMetadata metadata)
    {
        var items = new List<ISelectItem>();

        try
        {
            var propertyType = metadata.ModelType;

            var propertyListType = propertyType.GetTypeArgument();

            if (propertyListType == null)
                throw new Exception("Property " + metadata.PropertyName + ": Must be of type IList<string> or IList<Enum> (Enum is your own custom type 'public enum Colors ...'");

            if (propertyListType != SystemType.StringType && !propertyListType.IsEnum)
                throw new Exception("Property " + metadata.PropertyName + ": Must be of type IList with either String or Enum");

            var options = GetOptions<MultiDropdownSelectionAttribute>(metadata);

            var multiDropdownSelectionSaveString = propertyListType == SystemType.StringType;

            var multiDropdownSelectionDoFilter = propertyListType.IsEnum && (options.SelectionFactoryType != null || (options.EnumType != null && options.EnumType != propertyListType));

            SetEditorConfiguration(metadata, nameof(multiDropdownSelectionDoFilter), multiDropdownSelectionDoFilter);

            SetEditorConfiguration(metadata, nameof(multiDropdownSelectionSaveString), multiDropdownSelectionSaveString);

            SetEditorConfiguration(metadata, "multiDropdownShowExpiredItems", options.ShowExpiredItems);

            var defaultPropertyIListOfStrings = "epi-cms/contentediting/editors/propertyvaluelist/PropertyValueList";
            if (metadata.ClientEditingClass == defaultPropertyIListOfStrings)
                metadata.ClientEditingClass = "/SystemLibrary/CommonEpiserverCms/MultiDropdownSelection/Script";

            if (options.EnumType != null && !options.EnumType.IsEnum)
                throw new Exception("Property " + metadata.PropertyName + ": EnumType is filled in the attribute, but the type is not an Enum");

            var type = metadata.ModelType;

            var selectionType = options.EnumType ?? type.GetTypeArgument();

            if (options.SelectionFactoryType != null)
            {
                var selectionFactory = Activator.CreateInstance(options.SelectionFactoryType);

                var method = options.SelectionFactoryType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                   .Where(x => x.Name == "GetSelections")
                   .FirstOrDefault();

                if (method == null)
                    throw new Exception("Method 'GetSelections' is not existing on type: " + options.SelectionFactoryType.Name);

                var data = method.Invoke(selectionFactory, new object[] { metadata }) as IEnumerable<ISelectItem>;

                SetEditorConfiguration(metadata, "multiDropdownStoreOptions", null);

                return data ?? items;
            }
            else
            {
                var multiDropdownStoreOptions = new List<ISelectItem>();

                PopulateSelectionItems(multiDropdownStoreOptions, options, propertyListType, metadata);

                SetEditorConfiguration(metadata, "multiDropdownStoreOptions", multiDropdownStoreOptions);

                PopulateSelectionItems(items, options, selectionType, metadata);
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);

            items.Add(new SelectItem() { Text = "ERROR: " + ex.Message, Value = "-1" });
        }
        return items;
    }


    static void SetEditorConfiguration(ExtendedMetadata metadata, string name, object value)
    {
        if (metadata.EditorConfiguration.ContainsKey(name))
            metadata.EditorConfiguration[name] = value;
        else
            metadata.EditorConfiguration.Add(name, value);
    }

}