using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using SystemLibrary.Common.Web;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Dependency injection registry with all services
/// - Look at it as ServiceLocator 
/// </summary>
public static class Services
{
    internal static IServiceCollection Collection;

    /// <summary>
    /// Returns the service registered for the type T or null if not found
    /// </summary>
    public static T Get<T>() where T : class
    {
        return HttpContextInstance.Current?.RequestServices?.GetService<T>();
    }

    /// <summary>
    /// Tries to remove the service registered, or does nothing if already removed
    /// 
    /// Throws exception if called too early in the "middleware pipeline"
    /// </summary>
    public static void Remove<T>()
    {
        if (Collection == null)
            throw new System.Exception("You are calling 'Remove()' of " + typeof(T).Name + " too early, call after CommonEpiserverServices() has been ran");

        var type = typeof(T);
        if (type.IsClass || type.IsInterface)
        {
            Collection.RemoveAll<T>();
        }
        else
            Log.Error(typeof(T).Name + " is not an interface nor a class, cannot be removed");
    }
}