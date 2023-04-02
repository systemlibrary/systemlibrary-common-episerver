using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using EPiServer.Cms.UI.AspNetIdentity;

using Microsoft.AspNetCore.Identity;

using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// CurrentUser can be injected into your services or controllers as you'd like, or simply 'new CurrentUser()' anywhere you need it
/// 
/// Remember: to use 'CurrentUser' as an injected object, you must call 'CommonEpiserverServices&lt;CurrentUser&gt;(...);', or register 'CurrentUser' as a 'service' yourself
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// // Implement your own CurrentUser object by inheriting 'CurrentUser'
/// 
/// public class AppUser : CurrentUser 
/// {
///     public string PhoneNumber { get; set;}
///     
///     public override void OnAddClaims(ClaimsIdentity claimsIdentity)
///     {
///         claimsIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, PhoneNumber));
///     }
/// }
/// 
/// // Remember to register your new user inside ConfigureServices() at startup of your app:
/// public void ConfigureServices(IServiceCollection services)
/// {
///     services.CommonEpiserverServices&lt;AppUser&gt;(...);
/// }
/// </code>
/// </example>
public class CurrentUser : ApplicationUser
{
    IPrincipal _Principal;
    IPrincipal Principal => _Principal != null ? _Principal :
        (_Principal = HttpContextInstance.Current?.User);

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// Returns true if current user is logged in and is in any of the roles: CmsAdmin, WebAdmins, Administrators, CmsEditors, WebEditors
    /// </summary>
    public bool IsCmsUser() => IsAuthenticated && Principal.IsInAnyRole(Roles.CmsRoles);

    /// <summary>
    /// Returns true if current user is logged in and is in any of the admin roles: CmsAdmins, WebAdmins, Administrators
    /// </summary>
    public bool IsAdministrator() => IsAuthenticated && Principal.IsInAnyRole(Roles.AdminRoles);

    /// <summary>
    /// Returns true if current user is logged in and is in the role specified
    /// - Checking for an 'unauthenticated role' does not work
    /// </summary>
    public bool IsInRole(string roleName) => IsAuthenticated && Principal.IsInAnyRole(roleName);

    /// <summary>
    /// Name of the Principal Identity
    /// </summary>
    public string Name => Principal?.Identity?.Name;

    /// <summary>
    /// First name taken from claim 'GivenName'
    /// </summary>
    public string GivenName => GetClaim(ClaimTypes.GivenName);

    /// <summary>
    /// Last name taken from claim 'Surname'
    /// </summary>
    public string Surname => GetClaim(ClaimTypes.Surname);

    /// <summary>
    /// Override 'OnAddClaims' to add additional claims to your 'CurrentUser' object.
    /// 
    /// NOTE: Not yet implemented
    /// </summary>
    /// <param name="claimsIdentity"></param>
    public virtual void OnAddClaims(ClaimsIdentity claimsIdentity)
    {
        claimsIdentity.AddClaim(new Claim(ClaimTypes.MobilePhone, PhoneNumber));
    }

    /// <summary>
    /// //TODO: Implement so this is in use and works like planned
    /// </summary>
    /// <param name="manager"></param>
    /// <returns></returns>
    public async Task<ClaimsIdentity> GenerateUserIdentityAsync(IUserClaimsPrincipalFactory<CurrentUser> manager)
    {
        var userIdentity = await manager.CreateAsync(this);

        var claimsIdentity = userIdentity.Identity as ClaimsIdentity;

        if (claimsIdentity == null) return null;

        if (UserName.IsNot())
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Username));

        if (GivenName.IsNot())
            claimsIdentity.AddClaim(new Claim(ClaimTypes.GivenName, GivenName));

        if (Email.IsNot())
            claimsIdentity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/email", Email));

        if (Email.IsNot())
            claimsIdentity.AddClaim(new Claim("email", Email));

        if (Surname.IsNot())
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Surname, Surname));

        OnAddClaims(claimsIdentity);

        return null;
    }

    string GetClaim(string type, string typeFallback = null, string defaultValue = null)
    {
        if (Principal is ClaimsPrincipal claimsPrincipal)
        {
            var claim1 = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == type);
            if (claim1 != null) return claim1.Value;

            var claim2 = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == typeFallback);
            if (claim2 != null) return claim2.Value;
        }

        return defaultValue;
    }
}
