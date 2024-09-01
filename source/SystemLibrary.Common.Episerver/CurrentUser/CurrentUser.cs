﻿using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

using EPiServer.Cms.UI.AspNetIdentity;

using SystemLibrary.Common.Net;
using SystemLibrary.Common.Web;
using SystemLibrary.Common.Web.Extensions;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// CurrentUser can be injected into your services or controllers as you'd like, or simply 'new CurrentUser()' anywhere you need it
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
    ClaimsPrincipal GetPrincipal()
    {
        return HttpContextInstance.Current?.User;
    }

    public bool IsAuthenticated => GetPrincipal()?.Identity?.IsAuthenticated == true;
    
    /// <summary>
    /// Returns true if current user is logged in and is in any of the roles: CmsAdmin, WebAdmins, Administrators, CmsEditors, WebEditors
    /// </summary>
    public bool IsCmsUser() => IsAuthenticated && GetPrincipal().IsInAnyRole(Roles.CmsRoles);

    /// <summary>
    /// Returns true if current user is logged in and is in any of the admin roles: CmsAdmins, WebAdmins, Administrators
    /// </summary>
    public bool IsAdministrator() => IsAuthenticated && GetPrincipal().IsInAnyRole(Roles.AdminRoles);

    /// <summary>
    /// Returns true if current user is logged in and is in the role specified
    /// - Checking for an 'unauthenticated role' does not work
    /// </summary>
    public bool IsInRole(string roleName) => IsAuthenticated && GetPrincipal().IsInAnyRole(roleName);

    /// <summary>
    /// Name of the Principal Identity
    /// </summary>
    public string Name => IsAuthenticated ? GetPrincipal().Identity?.Name ?? GetClaim(ClaimTypes.Name) ?? "" : "";

    /// <summary>
    /// First name taken from claim 'GivenName'
    /// </summary>
    public string GivenName => IsAuthenticated ? GetClaim(ClaimTypes.GivenName) : default;

    /// <summary>
    /// Last name taken from claim 'Surname'
    /// </summary>
    public string Surname => IsAuthenticated ? GetClaim(ClaimTypes.Surname) : default;

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
        var p = GetPrincipal();
        if (p?.Claims == null) return defaultValue;

        var claim1 = p.Claims.FirstOrDefault(x => x.Type == type);
        if (claim1 != null) return claim1.Value;

        if (typeFallback != null)
        {
            var claim2 = p.Claims.FirstOrDefault(x => x.Type == typeFallback);
            if (claim2 != null) return claim2.Value;
        }

        return defaultValue;
    }
}
