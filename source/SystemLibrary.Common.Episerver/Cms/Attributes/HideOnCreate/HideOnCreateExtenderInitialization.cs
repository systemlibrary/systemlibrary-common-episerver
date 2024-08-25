using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Shell.ObjectEditing;

namespace SystemLibrary.Common.Episerver.Attributes;

[InitializableModule]
[ModuleDependency(typeof(ServiceContainerInitialization))]
public class HideOnCreateExtenderInitialization : IInitializableModule
{
    public void Initialize(InitializationEngine context)
    {
        if (context.HostType == HostType.WebApplication)
        {
            var registry = context.Locate.Advanced.GetInstance<MetadataHandlerRegistry>();
            registry.RegisterMetadataHandler(typeof(ContentData), new HideOnCreateMetadataExtender());
        }
    }

    public void Preload(string[] parameters) { }

    public void Uninitialize(InitializationEngine context) { }
}