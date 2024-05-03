using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

using SystemLibrary.Common.Episerver.Extensions;
using SystemLibrary.Common.Episerver.Tests;

namespace SystemLibrary.Common.Episerver.Benchmarks;

[SimpleJob(RuntimeMoniker.Net70, warmupCount: 2, launchCount: 2, iterationCount: 3, invocationCount: 1500)]
[MemoryDiagnoser]
[RPlotExporter]
public class GetSsrIdBenchmarks
{
    [Benchmark]
    public StringBuilder Render_Simple_Model_With_AdditionalEnumerableItems_BenchMark()
    {
        var temp = new TestBlock();

        temp.Year = 9999;
        temp.Title = "Hello world 123456";
        temp.Flag = true;
        temp.ShouldBeImplicitlyExported = "ShouldBeImplicitlyExported";

        temp.InnerBlocks = new List<NestedTestBlock>() {
            new NestedTestBlock
            {
                 Age = 12345,
                 InnerTitle = "EnumerableTitle"
            }
        };

        var result = temp.ReactServerSideRender(new
        {
            enumerableItems = temp.InnerBlocks.Where(x => x != null)
        },
        renderClientOnly: true);

        return result;
    }
}
