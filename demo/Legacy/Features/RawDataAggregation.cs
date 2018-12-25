using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Aggregators.Utils;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Tools.Aggregators;

namespace LoadRunner.Demo.Legacy.Features
{
    public class RawDataAggregationDemo
    {

        // Thats advanced undocummented feature-aggregator, which writes all raw meassurement data to json file. (Not fool-proof: if stopped abnormally, json array will get corrupted and requires hand-fixing)
        // It comes from other RnD nuget package:
        // * https://www.nuget.org/packages/Viki.LoadRunner.Tools
        // * Install-Package Viki.LoadRunner.Tools -Pre
        //
        // The main idea behind this is:
        // You save raw meassurements to the file, and then do aggregations from it after the tests.
        // This approach provides few key advantages like:
        // * One can aggregate all data again and again in any way one will think after the test.
        // * Less overhead to cpu, as complex on-the-fly aggregations can be cpu/ra taxing.
        // * It provides a way to merge results into a single aggregation if one creates scenario which is executed from multiple computers at the same time ***
        //
        // To enable it: Append [RawDataAggregationDemo.BuildJsonStreamAggregator] to DetailedDemo.Run() [Line 23]
        // E.g.:  strategy.AddAggregator(histogramAggregator, RawDataAggregationDemo.BuildJsonStreamAggregator());
        //
        // And this method will get called after pressing any key after the test finishes.
        public static IAggregator BuildJsonStreamAggregator()
        {
            return new JsonStreamAggregator("masterdata.json");
        }

        public static void Aggregate()
        {
            // Feature disabled?
            if (!File.Exists("masterdata.json"))
                return;

            // Any random aggregator
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new FuncDimension("Created Threads", i => i.CreatedThreads.ToString()))
                .Add(new CountMetric());

            // [TUserData] - Type of UserData object in your tests (If not using it, leave object)
            // Correct type is required to prevent deserialization problems.
            JsonStreamAggregator.Replay<object>("masterdata.json", aggregator);

            Console.WriteLine("## JsonStreamAggregator demo ##");
            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            Console.ReadKey();
        }

        // ***
        private static void AggregateFromMultipleSources()
        {
            IEnumerable<ReplayResult<object>> pc1 = JsonStreamAggregator.Load<object>("masterdata.json");
            IEnumerable<ReplayResult<object>> pc2 = JsonStreamAggregator.Load<object>("masterdata.json");
            IEnumerable<ReplayResult<object>> merged = pc1.Concat(pc2);

            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            StreamAggregator.Replay(merged);

            Console.WriteLine("## Merged demo ##");
            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            Console.ReadKey();
        }
    }
}