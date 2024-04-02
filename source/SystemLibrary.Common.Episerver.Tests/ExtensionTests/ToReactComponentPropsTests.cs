
using System;

using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ToReactComponentPropsTests
{
    [TestMethod]
    public void PropsModel_To_ReactProps_Success()
    {
        PropsModel propsModel = null;

        var result = propsModel.ReactServerSideRender(propsModel.GetType());

        Dump.Write(result);
    }

    public class PropsModel
    {
        public string A;
        public string B { get; set; }
        public int C { get; set; }
        public DateTime D;
        public DateTimeOffset E { get; set; }
    }

    public class PropsBlockData : BlockData
    {
        public virtual string Title { get; set; }

        public virtual bool Flag { get; set; }

        public int Age;

        public int Year { get; set; }
    }
}
