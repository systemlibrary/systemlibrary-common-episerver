using System;

namespace SystemLibrary.Common.Episerver.Tests;

partial class Fakes
{
    static int Random()
    {
        Random r = new Random(DateTime.Now.Millisecond);
        return r.Next(100);
    }
}