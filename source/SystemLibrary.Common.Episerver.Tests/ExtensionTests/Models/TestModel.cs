using System;

namespace SystemLibrary.Common.Episerver.Tests;

public class TestModel
{
    public string A;
    public string B { get; set; }
    public int C { get; set; }
    public DateTime D;
    public DateTimeOffset E { get; set; }
    private DateTimeOffset F { get; set; }
    string G { get; set; }
    internal static string H { get; set; }

    public virtual string AnotherA
    {
        get
        {
            return "A?" + A?.ToLower() + "AnotherA";
        }
    }
}
