﻿using System.Security.Claims;

using EPiServer.Cms.UI.AspNetIdentity;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver.Users;

/// <summary>
/// CurrentUser can be injected or 'new CurrentUser()' whenever you need it
/// <para>Remember: to use 'CurrentUser' as an injected object, you must call 'AddCommonCmsServices&lt;CurrentUser&gt;(...);' or similar</para>
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// // Implement your own CurrentUser object by inheriting 'CurrentUser'
/// 
/// public class AppUser : CurrentUser
/// {
///     public string PhoneNumber => Claim&lt;string&gt;("PhoneNumber");
/// }
/// 
/// // Register user class within ConfigureServices() at startup:
/// public void ConfigureServices(IServiceCollection services)
/// {
///     services.AddCommonCmsServices&lt;AppUser, ...&gt;(...);
/// }
/// 
/// // Use your AppUser anywhere you'd like
/// public ActionResult Index()
/// {
///     var user = new AppUser();
///     user.IsAdministrator || user.IsCmsUser
/// }
/// </code>
/// </example>
public class CurrentUser : ApplicationUser
{
    ClaimsPrincipal Principal() => HttpContextInstance.Current?.User ?? new ClaimsPrincipal();

    public bool IsAuthenticated => Principal()?.Identity?.IsAuthenticated == true;
    
    /// <summary>
    /// Returns true if current user is logged in and is in any of the roles: CmsAdmin, WebAdmins, Administrators, CmsEditors, WebEditors
    /// </summary>
    public bool IsCmsUser => IsAuthenticated && Principal().IsInAnyRole(Roles.CmsRoles);

    /// <summary>
    /// Returns true if current user is logged in and is in any of the admin roles: CmsAdmins, WebAdmins, Administrators
    /// </summary>
    public bool IsAdministrator => IsAuthenticated && Principal().IsInAnyRole(Roles.AdminRoles);

    /// <summary>
    /// Returns true if current user is logged in and is in the role specified
    /// - Checking for an 'unauthenticated role' does not work
    /// </summary>
    public bool IsInRole(string roleName) => IsAuthenticated && Principal().IsInAnyRole(roleName);

    /// <summary>
    /// Name of the Principal Identity
    /// </summary>
    public string Name => Principal()?.Identity?.Name ?? GetClaim(ClaimTypes.Name);

    /// <summary>
    /// First name taken from claim 'GivenName'
    /// </summary>
    public string GivenName => GetClaim(ClaimTypes.GivenName);

    /// <summary>
    /// Last name taken from claim 'Surname'
    /// </summary>
    public string Surname => GetClaim(ClaimTypes.Surname);

    public T Claim<T>(string claim, T defaultValue = default)
    {
        if (claim.IsNot()) return defaultValue;

        var value = GetClaim(claim);

        if(value == null) return defaultValue;

        if (value is T t) return t;

        var type = typeof(T);
        if (type == SystemType.IntType)
            return (T)(object)int.Parse(value);

        if (type == SystemType.DateTimeType)
            return (T)(object)DateTime.Parse(value);

        if(type == SystemType.BoolType)
            return (T)(object) bool.Parse(value);

        throw new Exception("Unsupported claim type " + type.Name + ". Supports only int, string, bool and datetime for now. Return the value as a string and convert it yourself.");
    }

    string GetClaim(string type, string typeFallback = null, string defaultValue = null)
    {
        if (Principal() is ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal?.Claims == null) return defaultValue;

            var claim1 = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == type);
            if (claim1 != null) return claim1.Value;

            if (typeFallback != null)
            {
                var claim2 = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == typeFallback);
                if (claim2 != null) return claim2.Value;
            }
        }

        return defaultValue;
    }
}
