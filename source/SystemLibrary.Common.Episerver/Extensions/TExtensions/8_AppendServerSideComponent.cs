using System.Text;

using React;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Net.Configurations;

namespace SystemLibrary.Common.Episerver.Extensions;

partial class TExtensions
{
    static bool? _IsDevelopment = null;
    static bool IsDevelopment
    {
        get
        {
            if (_IsDevelopment == null)
                _IsDevelopment = EnvironmentConfig.IsLocal;

            return _IsDevelopment.Value;
        }
    }

    static void RenderServerSideComponent(StringBuilder root, IReactComponent serverSideReactComponent, string ssrId, string tagName, bool renderClientSide)
    {
        if (serverSideReactComponent == null) return;

        root.Append(serverSideReactComponent.RenderHtml(false, true));
    }
}