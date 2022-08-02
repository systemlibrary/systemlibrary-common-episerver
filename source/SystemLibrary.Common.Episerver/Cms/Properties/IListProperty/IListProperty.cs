using EPiServer.Core;

namespace SystemLibrary.Common.Episerver.Cms.Properties;

/// <summary>
/// Enable your custom C# class to be stored as JSON in the DB through 'IList&gt;T&lt;'
/// </summary>
/// <example>
/// <code>
/// [PropertyDefinitionTypePlugIn(Description = "List of cars", GUID = "...")]
/// public class CarProperty : IListProperty&gt;Car&lt;
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
///     [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<Car>))]
///     public virtual IList&gt;Car&lt; Cars { get;set; }       //'CarProperty' enables 'Car' to be used in the IList
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
    /// public class CarProperty : IListProperty&gt;Car&lt;
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
    ///     [EditorDescriptor(EditorDescriptorType = typeof(CollectionEditorDescriptor<Car>))]
    ///     public virtual IList&gt;Car&lt; Cars { get;set; }       //'CarProperty' enables 'Car' to be used in the IList
    /// }
    /// </code>
    /// </example>
    public IListProperty()
    {

    }
}

