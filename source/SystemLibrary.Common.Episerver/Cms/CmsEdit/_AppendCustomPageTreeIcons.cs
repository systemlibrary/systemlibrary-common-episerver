using System.Text;

using static SystemLibrary.Common.Episerver.Attributes.ContentIconAttribute;

namespace SystemLibrary.Common.Episerver.Cms
{
    partial class CmsEditController
    {
        static void AppendCustomPageTreeIcons(StringBuilder sb)
        {
            if (ContentIconRouteModule.ContentDescriptorSettings == null) return;

            foreach (var setting in ContentIconRouteModule.ContentDescriptorSettings)
            {
                if (setting.Item4.Is())
                {
                    sb.Append("." + setting.Item3.Replace(" ", "."));

                    var url = setting.Item4;

                    if (url.StartsWith("~"))
                        url = url.Substring(1);
                    if (!url.StartsWith("/"))
                        url = "/" + url;

                    sb.Append("{background-image: url(" + url + ");}");
                }
            }
        }
    }
}
