using System.Text;

using EPiServer.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using SystemLibrary.Common.Episerver.Extensions;

namespace SystemLibrary.Common.Episerver.Tests;

[TestClass]
public class ContentAreaExtensionsTests
{
    [TestMethod]
    public void Is_And_IsNot_Success()
    {
        ContentArea contentArea = null;
        Assert.IsFalse(contentArea.Is(), "Content area Is() was true when null");
        Assert.IsTrue(contentArea.IsNot(), "Content area IsNot() was true when null");

        contentArea = new ContentArea();
        Assert.IsFalse(contentArea.Is(), "Content area Is() was true when empty");
        Assert.IsTrue(contentArea.IsNot(), "Content area IsNot() was true when empty");

        contentArea = Fakes.ContentArea();
        Assert.IsTrue(contentArea.Is(), "ContentArea has 1 item, but Is is false");
        Assert.IsFalse(contentArea.IsNot(), "ContentArea has 1 item, but IsNot is true");
    }


    class Car
    {
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
        public int Age { get; set; }

    }

    [TestMethod]
    public void T()
    {
        var c0 = new Car();

        c0.First = "Hello";
        c0.Middle = "World";
        c0.Age = 10;

        var c1 = new Car();

        c1.First = "Hello";
        c1.Middle = "World";
        c1.Age = 10;

        var c2 = new Car();

        c2.First = "Hello";
        c2.Middle = "World";
        c2.Age = 10;


        var c3 = new Car();

        c3.First = "Hello";
        c3.Middle = "World";
        c3.Age = 9;

        var c4 = new Car();

        c4.First = "Hello";
        c4.Middle = "World";
        c4.Age = 11;

        var c5 = new Car();

        c5.First = "Hello";
        c5.Middle = "World";
        c5.Last = "";
        c5.Age = 11;

        var c6 = new Car();
        c6.First = "Hello";
        c6.Middle = "World";
        c6.Last = "!";
        c6.Age = 10;

        var c7 = new Car();
        c7.First = "H";
        c7.Middle = "W";
        c7.Last = "!";
        c7.Age = 10;

        var c8 = new Car();
        c8.First = "H";
        c8.Middle = "w";
        c8.Last = "!";
        c8.Age = 10;


        Dump.Write(GetSsrId(c0));
        Dump.Write(GetSsrId(c1));
        Dump.Write(GetSsrId(c2));
        Dump.Write(GetSsrId(c3));
        Dump.Write(GetSsrId(c4));
        Dump.Write(GetSsrId(c5));
        Dump.Write(GetSsrId(c6));
        Dump.Write(GetSsrId(c7));
        Dump.Write(GetSsrId(c8));

        Dump.Write(("1234567890123456".GetHashCode() % 100000));
    }


    static string GetSsrId(Car c)
    {
        var props = typeof(Car).GetProperties();

        var sb = new StringBuilder();

        foreach(var p in props)
        {
            var v = p.GetValue(c);

            if(v == null)
            {
                sb.Append("x");
                continue;
            }
            if(v == "")
            {
                sb.Append("X");
                continue;
            }

            var type = p.PropertyType;

            if(type == typeof(int))
            {
                sb.Append("&");
                sb.Append(v.ToString());
                continue;
            }

            if(type == typeof(string))
            {
                var txt = v as string;

                if(txt.Length > 255)
                {
                    sb.Append(txt[txt.Length - 1] + (txt.Substring(0, 16).GetHashCode() % 10000) + txt.Length);
                }
                else
                {
                    if(txt.Length <= 6)
                    {
                        sb.Append(txt);
                    }
                    else
                    {
                        sb.Append(txt[0]);
                        sb.Append(txt[1]);
                        sb.Append(txt[2]);
                        sb.Append(txt[txt.Length-1]);
                        sb.Append(txt.Length.ToString());
                        sb.Append(txt.GetHashCode() % 1000000);
                    }
                }
            }
        }


        return sb.ToString();

    }
}
