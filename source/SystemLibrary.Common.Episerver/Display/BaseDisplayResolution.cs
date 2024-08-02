using EPiServer.Framework.Localization;
using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace SystemLibrary.Common.Episerver.Display;

/// <summary>
/// Internal: Copy pasted from 'Foundation'
/// </summary>
internal abstract class BaseDisplayResolution : IDisplayResolution
{
    Injected<LocalizationService> LocalizationService { get; set; }

    protected BaseDisplayResolution(string name, int width, int height)
    {
        Id = GetType().FullName;
        Name = Translate(name);
        Width = width;
        Height = height;
    }

    public string Id { get; protected set; }

    public string Name { get; protected set; }

    public int Width { get; protected set; }

    public int Height { get; protected set; }

    string Translate(string resourceKey)
    {
        if (!LocalizationService.Service.TryGetString(resourceKey, out var value))
            value = resourceKey;

        return value;
    }
}
