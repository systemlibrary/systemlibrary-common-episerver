using System.Security.Claims;

using EPiServer.Cms.UI.AspNetIdentity;

using SystemLibrary.Common.Framework;
using SystemLibrary.Common.Framework.Extensions;

namespace SystemLibrary.Common.Episerver;

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
    // NOTE: Todo, performance gain by storing AppCurrentUser in HttpContextInstance.Current.Items
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
