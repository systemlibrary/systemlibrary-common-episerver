using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SystemLibrary.Common.Episerver;

internal static class Dictionaries
{
    internal static ConcurrentDictionary<int, PropertyInfo[]> ReactPropPropertiesCache;
    internal static ConcurrentDictionary<int, PropertyInfo[]> ModelToPropsPropertiesCache;

    static Dictionaries()
    {
        ReactPropPropertiesCache = new ConcurrentDictionary<int, PropertyInfo[]>();
        ModelToPropsPropertiesCache = new ConcurrentDictionary<int, PropertyInfo[]>();
    }
}
