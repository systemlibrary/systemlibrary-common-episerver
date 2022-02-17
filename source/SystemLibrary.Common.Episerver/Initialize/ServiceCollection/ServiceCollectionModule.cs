using EPiServer.Cms.Shell;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace SystemLibrary.Common.Episerver.Initialize
{
    [InitializableModule]
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class ServiceCollectionModule : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            Services.Instance = context.Locate.Advanced;
        }

        public void Preload(string[] parameters) { }

        public void Uninitialize(InitializationEngine context) { }
    }
}
