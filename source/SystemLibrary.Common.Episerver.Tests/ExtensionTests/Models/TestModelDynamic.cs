using System.Collections.Generic;

namespace SystemLibrary.Common.Episerver.Tests;

public class TestModelDynamic
{
    public IEnumerable<string> IEnumerableStrings { get; set; }
    public IEnumerable<dynamic> IEnumerableDynamic { get; set; }
    public IEnumerable<int> IEnumerableInts { get; set; }
}
