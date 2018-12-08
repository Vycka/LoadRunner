using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Playground.Tools;

namespace Viki.LoadRunner.Playground
{
    public class BlankScenario : IScenario
    {
        public static void Run()
        {
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new TimeDimension(TimeSpan.FromSeconds(4)))
                .Add(new CountMetric())
                .Add(new TransactionsPerSecMetric());

            //var kpiAggregator = new KpiPrinterAggregator(
            //    TimeSpan.FromSeconds(1),
            //    new MaxDurationMetric(),
            //    new CountMetric(Checkpoint.NotMeassuredCheckpoints),
            //    new TransactionsPerSecMetric());

            StrategyBuilder strategy = new StrategyBuilder()
                .SetScenario(new CountingScenarioFactory())
                .SetThreading(new FixedThreadCount(4))
                .SetLimit(new TimeLimit(TimeSpan.FromSeconds(13)))
                .SetAggregator(aggregator);

            strategy.Build().Run();

            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
        }

        public void ScenarioSetup(IIteration context)
        {
            
        }

        public void IterationSetup(IIteration context)
        {

        }

        public void ExecuteScenario(IIteration context)
        {
            //Thread.Sleep(200);
        }

        public void IterationTearDown(IIteration context)
        {

        }

        public void ScenarioTearDown(IIteration context)
        {

        }
    }
}

/*
   {
    "Time (s)": "0",
    "Count: Setup": 1348062,
    "Count: Iteration": 1348062,
    "Count: TearDown": 1348062,
    "TPS": 674761.49679643183
  },
  {
    "Time (s)": "2",
    "Count: Setup": 1431818,
    "Count: Iteration": 1431818,
    "Count: TearDown": 1431818,
    "TPS": 715909.357954679
  },
  {
    "Time (s)": "4",
    "Count: Setup": 1354222,
    "Count: Iteration": 1354222,
    "Count: TearDown": 1354222,
    "TPS": 670149.58661666419
  },
  {
    "Time (s)": "6",
    "Count: Setup": 1509802,
    "Count: Iteration": 1509802,
    "Count: TearDown": 1509802,
    "TPS": 750234.5411540221
  },
  {
    "Time (s)": "8",
    "Count: Setup": 1512103,
    "Count: Iteration": 1512103,
    "Count: TearDown": 1512103,
    "TPS": 741121.16937033308
  }

// With lots of background noise, to be remade
[
  {
    "Time (s)": "0",
    "Count: Setup": 1290852,
    "Count: Iteration": 1290852,
    "Count: TearDown": 1290852,
    "TPS": 650897.21416827361
  },
  {
    "Time (s)": "2",
    "Count: Setup": 1358476,
    "Count: Iteration": 1358476,
    "Count: TearDown": 1358476,
    "TPS": 677615.55120497243
  },
  {
    "Time (s)": "4",
    "Count: Setup": 1420186,
    "Count: Iteration": 1420186,
    "Count: TearDown": 1420186,
    "TPS": 701889.21358819911
  },
  {
    "Time (s)": "6",
    "Count: Setup": 1269466,
    "Count: Iteration": 1269466,
    "Count: TearDown": 1269466,
    "TPS": 634733.0
  },
  {
    "Time (s)": "8",
    "Count: Setup": 1456051,
    "Count: Iteration": 1456051,
    "Count: TearDown": 1456051,
    "TPS": 728025.75480901427
  }
]

    [
  {
    "Time (s)": "0",
    "Count: Setup": 4323723,
    "Count: Iteration": 4323723,
    "Count: TearDown": 4323723,
    "TPS": 1084685.606162854
  },
  {
    "Time (s)": "4",
    "Count: Setup": 3511072,
    "Count: Iteration": 3511072,
    "Count: TearDown": 3511072,
    "TPS": 875530.82175597851
  },
  {
    "Time (s)": "8",
    "Count: Setup": 4459555,
    "Count: Iteration": 4459555,
    "Count: TearDown": 4459555,
    "TPS": 1115180.1744590907
  },
  {
    "Time (s)": "12",
    "Count: Setup": 1034945,
    "Count: Iteration": 1034945,
    "Count: TearDown": 1034945,
    "TPS": 979081.26645547047
  }
]
[
  {
    "Count: Setup": 11223753,
    "Count: Iteration": 11223753,
    "Count: TearDown": 11223753,
    "max": "00:00:10.0318670"
  }
]
0 2850859
1 2865489
2 2760878
3 2746527
-------
11223753
TPS 1 118 809,99
0:00:10,031867

11223753/11223753 & 11223753 ? True && True
0 26331423
1 23584265
2 23133042
3 21751251
-------
94799981
TPS 9 331 730,16
0:00:10,1588858
 */
