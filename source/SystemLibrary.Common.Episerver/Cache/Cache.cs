using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace SystemLibrary.Common.Episerver;

/// <summary>
/// Cache
/// 
/// Default duration is 180 seconds
/// 
/// Required skip cache scenarios:
/// - If cachekey is null or empty
/// - If not in a web context
/// 
/// Optional skip cache scenarios by parameters to Cache.Get():
/// - authenticated visitors, every user that is signed, but is not part of Roles.CmsRoles, default to false
/// - authenticated cms users, defaults to true, so logged in cms users do not get cached content at all
/// 
/// Can implement your own custom skip logic, additional to the ones that are required:
/// - pass in 'skipCacheFor' parameter in Cache.Get() function
/// </summary>
public static class Cache
{
    static bool? _IsWebContext;
    static bool IsWebContext
    {
        get
        {
            if (_IsWebContext == null)
                _IsWebContext = Services.Get<IHttpContextAccessor>()?.HttpContext != null;

            return _IsWebContext.Value;
        }
    }

    static IMemoryCache cache;
    static object cacheLock = new object();

    static int _DefaultDuration = -1;
    static int DefaultDuration
    {
        get
        {
            if(_DefaultDuration == -1)
            {
                _DefaultDuration = AppSettings.Current.SystemLibraryCommonEpiserver.Cache.DefaultDuration;
            }
            return _DefaultDuration;
        }
    }

    static Cache()
    {
        MemoryCacheOptions options = new MemoryCacheOptions();
        options.CompactionPercentage = 25;
        options.ExpirationScanFrequency = TimeSpan.FromSeconds(180);
        options.SizeLimit = 200000;
        cache = new MemoryCache(options);
    }

    /// <summary>
    /// Get data from cache, or add it to cache before it is returned
    /// </summary>
    /// <example>
    /// <code class="language-csharp hljs">
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(cacheKey, () => {
    ///     return "hello world";
    /// });
    /// //'data' is now 'hello world', if called multiple times within the cache duration, "hello world" is returned from the cache
    /// 
    /// //Another example with more options:
    /// var cacheKey = "hello-world-key";
    /// var data = Cache.Get(cacheKey, () => {
    ///         return "hello world";
    ///     }, 
    ///     duration: TimeSpan.FromSeconds(1),
    ///     condition: x => x != "hello world",
    ///     skipCacheForCmsUsers: false);
    ///     
    /// //'data' is equal to 'hello world', cache duration is 1 second, but it only adds the result to cache, if it is not equal to "hello world"
    /// //So in this scenario - "hello world" is never added to cache, and our function that returns "hello world" is always invoked
    /// </code>
    /// </example>
    /// <param name="condition">Add to cache only if condition is met, for instance: data != null</param>
    /// <param name="skipCacheForAuthenticatedVisitors">Skip cache for logged in non-cms users</param>
    /// <param name="skipCacheForCmsUsers">Skip cache for Cms Users when they visit a page outside Edit Mode</param>
    /// <param name="skipCacheFor">Implement your own logic for when to skip cache, let it return true on your conditions to avoid caching</param>
    /// <returns>Returns data either from cache or from the getItem() method</returns>
    public static T Get<T>(Func<T> getItem, string cacheKey, TimeSpan duration = default, Func<T, bool> condition = null, bool skipCacheForAuthenticatedVisitors = false, bool skipCacheForCmsUsers = true, Func<bool> skipCacheFor = null) where T : class
    {
        if (!IsWebContext ||
            cacheKey.IsNot() ||
            SkipCache(skipCacheForAuthenticatedVisitors, skipCacheForCmsUsers, skipCacheFor))
            return getItem();

        var cached = cache.Get(cacheKey) as T;

        if (cached != null) return cached;

        cached = getItem();

        if (condition == null || condition(cached))
            Insert(cacheKey, cached, duration);

        return cached;
    }

    static void Insert(string cacheKey, object value, TimeSpan duration = default)
    {
        if (!IsWebContext) return;

        if (value == null)
            Remove(cacheKey);
        else
        {
            if (duration == default)
                duration = TimeSpan.FromSeconds(DefaultDuration);

            cache.Set(cacheKey, value, DateTime.Now.Add(duration));
        }
    }

    /// <summary>
    /// Remove a single item from cache based on cacheKey
    /// - If it does not exist, it does nothing
    /// - If context is not web, it does nothing
    /// </summary>
    public static void Remove(string cacheKey)
    {
        if (cacheKey.IsNot()) return;

        if (!IsWebContext) return;

        cache.Remove(cacheKey);
    }

    /// <summary>
    /// Clear everything found in cache
    /// </summary>
    public static void Clear()
    {
        if (!IsWebContext) return;

        var entries = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
        if (entries == null) return;

        var entriesCollection = entries.GetValue(cache) as ICollection;
        if (entriesCollection == null || entriesCollection.Count == 0) return;

        var keys = new List<string>();
        if (entriesCollection != null)
        {
            foreach (var item in entriesCollection)
            {
                var key = item.GetType().GetProperty("Key")?.GetValue(item);
                if(key!= null)
                    keys.Add(key.ToString());
            }
        }
        lock (cacheLock)
        {
            foreach(var key in keys)
                cache.Remove(key);
        }
    }

    static bool SkipCache(bool skipForVisitors, bool skipForCmsUsers, Func<bool> skipCacheFor)
    {
        if (skipForVisitors || skipForCmsUsers || skipCacheFor != null)
        {
            var user = new CurrentUser();

            if (skipForVisitors && SkipCacheForVisitors(user))
                return true;

            if (skipForCmsUsers && SkipCacheForCmsUsers(user))
                return true;

            if (skipCacheFor != null && skipCacheFor())
                return true;
        }

        return false;
    }

    static bool SkipCacheForVisitors(CurrentUser user)
    {
        return !user.IsCmsUser();
    }

    static bool SkipCacheForCmsUsers(CurrentUser user)
    {
        return user.IsCmsUser();
    }
}
