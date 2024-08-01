using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using SystemLibrary.Common.Net.Extensions;

namespace SystemLibrary.Common.Episerver.Benchmarks;

[SimpleJob(RuntimeMoniker.Net70, warmupCount: 2, launchCount: 2, iterationCount: 2, invocationCount: 150)]
[MemoryDiagnoser]
[RPlotExporter]
public class GetSsrIdBenchmarks
{
    //[Benchmark]
    //public StringBuilder Render_Simple_Model_With_AdditionalEnumerableItems_BenchMark()
    //{
    //    var temp = new TestBlock();

    //    temp.Year = 9999;
    //    temp.Title = "Hello world 123456";
    //    temp.Flag = true;
    //    temp.ShouldBeImplicitlyExported = "ShouldBeImplicitlyExported";

    //    temp.InnerBlocks = new List<NestedTestBlock>() {
    //        new NestedTestBlock
    //        {
    //             Age = 666,
    //             InnerTitle = "WHAT"
    //        }
    //    };

    //    var result = temp.ReactServerSideRender(new
    //    {
    //        enumerableItems = temp.InnerBlocks.Where(x => x != null)
    //    },
    //    renderClientOnly: true);


    //    //Dump.Write(result);
    //    return result;
    //}

    Type type = typeof(Car);
    Car car = new Car();
    Type typeModel = typeof(CarModel);
    CarModel carModel = new CarModel();
    Type typeViewModel = typeof(CarViewModel);
    CarViewModel carViewModel = new CarViewModel();
    class Car
    {
        public string Name { get; set; }
    }

    class CarModel
    {
        public string Name { get; set; }
    }

    class CarViewModel
    {
        public string Name { get; set; }
    }

}
