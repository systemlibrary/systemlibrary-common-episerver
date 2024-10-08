﻿using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace SystemLibrary.Common.Episerver.Initialize;

/// <summary>
/// Create your own InitializableModule by inheriting InitModule
/// <para>- No need to create both Initialize and Uninitialize as they come with a default empty implementation, unless you need both of course</para>
/// - Override the method(s) you need to use in your InitializableModule
/// </summary>
/// <example>
/// <code>
/// [InitializableModule]
/// public class ContentIconRouteModule : StartupModule 
/// {
///     public override void Initialize(InitializationEngine context)
///     {
///         //Do something for instance here in "Initialize"
///     }
///     
///     public override void Uninitialize(ServiceConfigurationContext context) 
///     {
///         //Or do something here
///     }
///     
///     public override void ConfigureContainer(ServiceConfigurationContext context) 
///     {
///         //Or do something here inside "ConfigureContainer" 
///     }
/// }
/// </code>
/// </example>
public abstract class StartupModule : IConfigurableModule
{
    public virtual void ConfigureContainer(ServiceConfigurationContext context)
    {
    }

    public virtual void Initialize(InitializationEngine context)
    {
    }

    public virtual void Uninitialize(InitializationEngine context)
    {
    }
}
