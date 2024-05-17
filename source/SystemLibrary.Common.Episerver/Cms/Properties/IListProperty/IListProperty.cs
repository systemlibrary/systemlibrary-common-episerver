using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Properties;

/// <summary>
/// Enable your custom C# class to be stored as JSON in the DB through IList&lt;T&gt;
/// </summary>
/// <example>
/// <code>
/// [PropertyDefinitionTypePlugIn(Description = "List of cars", GUID = "...")]
/// public class CarProperty : IListProperty&lt;Car&gt; // 'CarProperty' enables 'Car' to be used as 'virtual IList...'
/// {
/// }
/// 
/// public class Car
/// {
///     public virtual string Name { get; set;}
///     public virtual string Age { get; set;}
/// }
/// 
/// public class ArticlePage : PageData
/// {
///     [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor&lt;Car&gt;))]
///     public virtual IList&lt;Car&gt; Cars { get;set; } 
/// }
/// </code>
/// </example>
public abstract class IListProperty<T> : PropertyList<T>
{
    /// <summary>
    /// Enable your custom C# class to be stored as JSON in the DB through 'IList&gt;T&lt;'
    /// </summary>
    /// <example>
    /// <code>
    /// [PropertyDefinitionTypePlugIn(Description = "List of cars", GUID = "...")]
    /// public class CarProperty : IListProperty&lt;Car&gt;  // 'CarProperty' enables 'Car' to be used as 'virtual IList...'
    /// {
    /// }
    /// 
    /// public class Car
    /// {
    ///     public virtual string Name { get; set;}
    ///     public virtual string Age { get; set;}
    /// }
    /// 
    /// public class ArticlePage : PageData
    /// {
    ///     [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor&lt;Car&gt;))]
    ///     public virtual IList&lt;Car&gt; Cars { get;set; } 
    /// }
    /// </code>
    /// </example>
    public IListProperty()
    {
    }
}