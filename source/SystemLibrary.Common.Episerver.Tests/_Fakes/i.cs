using System;

namespace SystemLibrary.Common.Episerver.Tests;

partial class Fakes
{
    static int i()
    {
        Random r = new Random(DateTime.Now.Millisecond);
        return r.Next(100);
    }
}