using System.Security.Claims;

using EPiServer.Cms.UI.AspNetIdentity;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Episerver.Users;

/// <summary>
/// CurrentUser can be injected or 'new CurrentUser()' whenever you need it
/// <para>Remember: to use 'CurrentUser' as an injected object, you must call 'AddCommonCmsServices&lt;CurrentUser&gt;(...);' or similar</para>
/// </summary>
/// <example>
/// <code class="language-csharp hljs">
/// // Implement your own CurrentUser object by inheriting 'CurrentUser'
/// 
/// public class AppUser : AppCurrentUser
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
public static class CurrentUser
{
    public static string Id => new AppCurrentUser().Id;

    public static string GivenName => new AppCurrentUser().GivenName;
    public static string Surname => new AppCurrentUser().Surname;

    public static string PhoneNumber => new AppCurrentUser().PhoneNumber;

    public static string Email => new AppCurrentUser().Email;

    public static bool IsCmsUser => new AppCurrentUser().IsCmsUser;
    public static bool IsAuthenticated => new AppCurrentUser().IsAuthenticated;

    public static bool IsInRole(string roleName) => new AppCurrentUser().IsInRole(roleName);

    /// <summary>
    /// Returns true if current user is logged in and is in any of the admin roles: CmsAdmins, WebAdmins, Administrators
    /// </summary>
    public static bool IsAdministrator => new AppCurrentUser().IsAdministrator;

    public static string UserName => new AppCurrentUser().UserName;

    public static T Claim<T>(string claim, T defaultValue = default) => new AppCurrentUser().Claim(claim, defaultValue);

    public static ClaimsPrincipal GetClaimsPrincipal() => new AppCurrentUser().Principal();
}

public class AppCurrentUser : ApplicationUser
{
    internal ClaimsPrincipal Principal()
    {
        var u1 = HttpContextInstance.Current?.User;
        var u2 = HttpContextInstance.Current?.User;

        return u1 == u2 ? u1 : null;
    }

    public bool IsAuthenticated
    {
        get
        {
            return Principal()?.Identity?.IsAuthenticated == true;
        }
    }

    public bool IsCmsUser
    {
        get
        {
            if (!IsAuthenticated) return false;

            if (!Principal().IsInAnyRole(Roles.CmsRoles)) return false;

            var userAgent = HttpContextInstance.Current.Request?.Headers["User-Agent"] + "";

            // Black list
            if (userAgent.Length < 12 ||
                userAgent.Contains("Linux", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("SamsungBrowser", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("iPad", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
                userAgent.Contains("Brave", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // White list
            return userAgent.Contains("Chrome", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Firefox", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("Edg", StringComparison.OrdinalIgnoreCase);
        }
    }

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

        if (value == null) return defaultValue;

        if (value is T t) return t;

        var type = typeof(T);
        if (type == SystemType.IntType)
            return (T)(object)int.Parse(value);

        if (type == SystemType.DateTimeType)
            return (T)(object)DateTime.Parse(value);

        if (type == SystemType.BoolType)
            return (T)(object)bool.Parse(value);

        throw new Exception("Unsupported claim type " + type.Name + ". Supports only int, string, bool and datetime for now. Return the value as a string and convert it yourself.");
    }

    string GetClaim(string type, string typeFallback = null, string defaultValue = null)
    {
        var principal = Principal();

        if (principal != null && principal is ClaimsPrincipal claimsPrincipal)
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
