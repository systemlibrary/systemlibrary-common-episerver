using EPiServer.Core;
using EPiServer.Web;

using Microsoft.AspNetCore.Http;

namespace SystemLibrary.Common.Episerver;

partial class WebApplicationInitializer
{
    bool InitializedSiteDefinitions(HttpContext context)
    {
        if (siteDefinitionRepository?.List()?.Any() == true) return false;

        var request = context?.Request;

        var host = request?.Host ?? new HostString("http://localhost", 80);

        var site = new SiteDefinition
        {
            Id = Guid.NewGuid(),
            Name = host.Host,
            SiteUrl = new Uri(request.Scheme + "://" + host.Host + ":" + host.Port)
        };

        if (site.Hosts == null)
            site.Hosts = new List<HostDefinition>();

        var primaryHostName = host.Host.GetPrimaryDomain();

        site.Hosts.Add(new HostDefinition()
        {
            Name = primaryHostName + ":" + host.Port,
            Type = HostDefinitionType.Primary
        });

        site.Hosts.Add(new HostDefinition()
        {
            Name = HostDefinition.WildcardHostName,
            Type = HostDefinitionType.Undefined
        });

        site.StartPage = ContentReference.RootPage;

        siteDefinitionRepository.Save(site);
        return true;
    }
}
