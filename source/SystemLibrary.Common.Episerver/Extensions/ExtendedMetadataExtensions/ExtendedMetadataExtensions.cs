using Castle.Core.Internal;

using EPiServer.Shell.ObjectEditing;

namespace SystemLibrary.Common.Episerver.Extensions;

/// <summary>
/// Extension methods for 'EPiServer.Shell.ObjectEditing.ExtendedMetadata' which is inheriting DisplayMetadata from MVC  
/// </summary>
public static class ExtendedMetadataExtensions
{
    /// <summary>
    /// Return attribute of type that has been added to the property, or null if not added to the property
    /// <para>Finds for instance 'DisplayAttribute' on a public virtual property</para>
    /// Usually used within a ISelectionFactory in the method 'GetSelections'
    /// </summary>
    /// <example>
    /// <code>
    /// IEnumerable&lt;ISelectItem&gt; GetSelections(ExtendedMetadata metadata)
    /// {
    ///     var attribute = metadata.GetAttribute&lt;CustomAttribute&gt;();
    ///     
    ///     if(attribute != null)
    ///     {
    ///     }
    /// }
    /// </code>
    /// </example>
    public static T GetAttribute<T>(this ExtendedMetadata metadata) where T : System.Attribute
    {
        var attribute = metadata?.Attributes.LastOrDefault(x => x is T);

        if (attribute != null) return attribute as T;

        return metadata.ModelType.GetAttribute<T>();
    }
}
