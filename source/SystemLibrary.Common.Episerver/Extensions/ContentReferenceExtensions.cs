using EPiServer.Core;
using EPiServer.Web;
using EPiServer.Web.Routing;

namespace SystemLibrary.Common.Episerver.Extensions
{
    public static class ContentReferenceExtensions
    {
        static IUrlResolver _IUrlResolver;
        static IUrlResolver IUrlResolver =>
            _IUrlResolver != null ? _IUrlResolver :
            (_IUrlResolver = Services.Get<IUrlResolver>());

        public static bool IsNot(this ContentReference contentReference)
        {
            if (contentReference == null || contentReference.ID < 1) return true;

            return false;
        }

        public static bool Is(this ContentReference contentReference)
        {
            return contentReference != null && contentReference.ID > 0;
        }

        public static T To<T>(this ContentReference contentReference) where T : ContentData
        {
            if (contentReference.IsNot()) return default;

            return BaseCms.Get<T>(contentReference);
        }

        public static string ToFriendlyUrl(this ContentReference contentReference)
        {
            if (contentReference.IsNot()) return null;

            var url = IUrlResolver.GetUrl(contentReference, null, new UrlResolverArguments { ContextMode = ContextMode.Default });

            return url;
        }
    }
}
