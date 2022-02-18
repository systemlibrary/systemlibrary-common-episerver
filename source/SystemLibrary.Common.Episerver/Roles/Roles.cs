namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// Various default Cms Roles
    /// </summary>
    public static class Roles
    {
        public static string Administrators => EPiServer.Authorization.Roles.Administrators;
        public static string WebAdmins => EPiServer.Authorization.Roles.WebAdmins;
        public static string WebEditors => EPiServer.Authorization.Roles.WebEditors;
        public static string CmsAdmins => EPiServer.Authorization.Roles.CmsAdmins;
        public static string CmsEditors => EPiServer.Authorization.Roles.CmsEditors;

        static string[] _CmsRoles;
        public static string[] CmsRoles => _CmsRoles != null ? _CmsRoles :
            (_CmsRoles = new[] {
            Administrators,
            WebAdmins,
            WebEditors,
            CmsAdmins,
            CmsEditors
        });
    }
}