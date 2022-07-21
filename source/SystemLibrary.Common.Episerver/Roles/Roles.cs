namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// All roles that comes with the Episerver CMS which defines if the logged in user is a Cms User or not
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

        static string[] _AdminRoles;
        public static string[] AdminRoles => _AdminRoles != null ? _AdminRoles :
            (_AdminRoles = new[] {
                "Admin", 
                "Admins", 
                "Administrator",
                EPiServer.Authorization.Roles.Administrators,
                EPiServer.Authorization.Roles.WebAdmins,
                EPiServer.Authorization.Roles.CmsAdmins,
        });

    }
}