using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace SystemLibrary.Common.Episerver.Initialize
{
    public abstract class InitModule : IInitializableModule
    {
        public virtual void Initialize(InitializationEngine context)
        {
        }

        public virtual void Uninitialize(InitializationEngine context)
        {
        }
    }
}
