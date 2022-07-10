namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// Various default Cms Roles
    /// </summary>
    public static class Roles
    {
        static string[] _CmsRoles;
        public static string[] CmsRoles => _CmsRoles != null ? _CmsRoles :
            (_CmsRoles = new[] {
            EPiServer.Authorization.Roles.Administrators,
            EPiServer.Authorization.Roles.WebAdmins,
            EPiServer.Authorization.Roles.WebEditors,
            EPiServer.Authorization.Roles.CmsAdmins,
            EPiServer.Authorization.Roles.CmsEditors
        });
    }
}