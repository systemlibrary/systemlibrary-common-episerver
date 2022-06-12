using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// A static class to get any service ("service locator pattern") instead of injecting everything everywhere
    /// </summary>
    public static class Services
    {
        //NOTE: Might use 'static Injected<LocalizationService> Services' - if 'Injected' actually also triggers through 'static ctor' calls
        internal static IServiceProvider Instance;
        internal static IServiceCollection Collection;

        public static T Get<T>() where T : class
        {
                //..GetService<T>();

            if (Instance == null)
                throw new Exception("ServiceProvider instance is null. You either are using Services.Get() too early or you are not initializing it: within your 'startup.cs' and inside 'ConfigureServices(IServiceCollection s)' call on the extension method 's.CommonEpiserverServices()'");

            return Instance.GetService(typeof(T)) as T;
        }

        public static void Remove<T>() 
        {
            if (Collection == null)
            {
                Log.Warning("Services.Collection is null - cannot remove " + typeof(T).Name);
                return;
            }

            var type = typeof(T);
            if(type.IsClass || type.IsInterface)
            {
                Log.Warning("Removed " + type.Name);
                Collection.RemoveAll<T>();
            }
            else
                Log.Warning(typeof(T).Name + " is not an interface nor a class, cannot be removed");
        }
    }
}