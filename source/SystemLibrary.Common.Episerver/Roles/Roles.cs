namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Plus those few additional like "administrator, admins, ..." that comes from SystemLibrary.Common.Web.Cache logic</para>
/// </summary>
internal static class Roles
{
    static string[] _CmsRoles;

    public static string[] CmsRoles => _CmsRoles != null ? _CmsRoles :
        (_CmsRoles = new[] {
        EPiServer.Authorization.Roles.Administrators,
        EPiServer.Authorization.Roles.WebAdmins,
        EPiServer.Authorization.Roles.WebEditors,
        EPiServer.Authorization.Roles.CmsAdmins,
        EPiServer.Authorization.Roles.CmsEditors,
        EPiServer.Authorization.Roles.VisitorGroupAdmins,
        "administrators",
        "administrator",
        "Administrator"
    });

    static string[] _AdminRoles;
    public static string[] AdminRoles => _AdminRoles != null ? _AdminRoles :
        (_AdminRoles = new[] {
            EPiServer.Authorization.Roles.Administrators,
            EPiServer.Authorization.Roles.VisitorGroupAdmins,
            EPiServer.Authorization.Roles.WebAdmins,
            EPiServer.Authorization.Roles.CmsAdmins,
            "admin",
            "Admin",
            "admins",
            "Admins",
            "Administrator",
            "administrator",
            "administrators"
    });

}