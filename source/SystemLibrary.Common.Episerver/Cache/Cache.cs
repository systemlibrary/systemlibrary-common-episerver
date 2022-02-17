using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace SystemLibrary.Common.Episerver
{
    /// <summary>
    /// Cache
    /// 
    /// Skip: 'true' means that 'getItem' is always invoked, no cache occurs within this class
    /// 
    /// Cache is always skipped:
    /// - if cacheKey is null or empty
    /// - if HttpContext.Current is null
    /// - if PageIsInEditMode is true
    /// 
    /// Optional to also skip for:
    /// - authenticated visitors
    /// - authenticated cms users
    /// 
    /// Implement your own custom skip logic that will trigger after the required skip conditions:
    /// - skipCacheFor parameter upon "Get()"
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

        static Cache()
        {
            MemoryCacheOptions options = new MemoryCacheOptions();
            options.CompactionPercentage = 25;
            options.ExpirationScanFrequency = TimeSpan.FromSeconds(120);
            options.SizeLimit = 200000;
            cache = new MemoryCache(options);
        }

        /// <summary>
        /// Return the items from getItem() or from cache based
        /// </summary>
        /// <param name="condition">Add to cache only if condition is met, for instance: data != null</param>
        /// <param name="skipCacheForAuthenticatedVisitors">Skip cache for logged in non-cms users</param>
        /// <param name="skipCacheForCmsUsers">Skip cache for Cms Users when they visit a page outside Edit Mode</param>
        /// <param name="skipCacheFor">Implement your own logic for when to skip cache</param>
        /// <returns>Returns data either from cache or from the getItem() method</returns>
        public static T Get<T>(Func<T> getItem, string cacheKey, TimeSpan duration = default, Func<T, bool> condition = null, bool skipCacheForAuthenticatedVisitors = false, bool skipCacheForCmsUsers = true, Func<bool> skipCacheFor = null) where T : class
        {
            if (!IsWebContext ||
                cacheKey.IsNot() ||
                BaseCms.IsInEditMode() ||
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
                    duration = TimeSpan.FromSeconds(60);

                cache.Set(cacheKey, value, DateTime.Now.Add(duration));
            }
        }

        public static void Remove(string cacheKey)
        {
            if (cacheKey.IsNot()) return;

            if (!IsWebContext) return;

            cache.Remove(cacheKey);
        }

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
            return user.IsAuthenticated && !user.IsCmsUser();
        }

        static bool SkipCacheForCmsUsers(CurrentUser user)
        {
            return user.IsCmsUser();
        }
    }
}
