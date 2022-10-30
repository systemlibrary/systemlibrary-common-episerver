//using SystemLibrary.Common.Net;

//namespace SystemLibrary.Common.Episerver.Cms.Admin.Reports
//{
//    //https://krompaco.nu/2022/10/identify-external-embedded-resources-in-cms-content/
//    public class LinksInContentText
//    {
//        /// <param name="isExternal">-1 for all, 0 for only internal contentlinks, 1 for only external links</param>
//        static string GetQuery(int isExternal = -1)
//        {
//            string q = Assemblies.GetEmbeddedResource("Cms/Admin/Reports/" + nameof(LinksInContentText), "Get" + nameof(LinksInContentText) + ".sql");

//            return q.Replace("@" + nameof(isExternal) + "@", isExternal + "");
//        }
//    }
//}
