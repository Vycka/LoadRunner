using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Parameters;

namespace LoadRunner.Demo
{
    public class QuickIntroLoadTest
    {
        public static void Run()
        {
            // Initialize LoadRunnerEngine by providing:
            // * Type of class which implements ILoadTestScenario (e.g DemoTestScenario)
            // * LoadRunnerParameters
            // * As many aggregators as you like 
            HistogramAggregator histogramAggregator = AggregationSetup.Build();
            LoadRunnerParameters parameters = ParametersSetup.Build();
            LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<DemoTestScenario>(parameters, histogramAggregator);

            // Run test (blocking call)
            loadRunner.Run();

            // Once finished, extract information from used aggregators, and do some exceling :)
            // BuildResultsObjects() will produce results in structure compatible with JSON -> CSV converters. (See ~20 lines below)
            IEnumerable<object> defaultResults = histogramAggregator.BuildResultsObjects();

            Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));
            Console.ReadKey();

            //Alternative export way is 
            // HistogramResults results = histogramAggregator.BuildResults();
            //
            // results will be presented in this structure:
            // * string[] ColumnNames;
            // * object[][] Values;
        }
    }
}

// BuildResultsObjects() results
// You will get result similar to this.
// This structure can be used with online JSON -> CSV converters.
// Imported to excel, and some charts could be drawn with it.
// See DemoResults.xlsx 
/*
[
  {
    "Time (s)": "0",
    "Min (ms)": 30,
    "Avg (ms)": 483,
    "Max (ms)": 964,
    "50% (ms)": 497,
    "80% (ms)": 733,
    "90% (ms)": 800,
    "95% (ms)": 853,
    "99% (ms)": 921,
    "Success: Count": 363,
    "Errors: Totals": 61,
    "Errors: Iteration": 38,
    "Errors: Teardown": 12,
    "Errors: Setup": 11,
    "Created Threads": 20
  },
  {
    "Time (s)": "10",
    "Min (ms)": 6,
    "Avg (ms)": 512,
    "Max (ms)": 994,
    "50% (ms)": 508,
    "80% (ms)": 759,
    "90% (ms)": 834,
    "95% (ms)": 905,
    "99% (ms)": 958,
    "Success: Count": 688,
    "Errors: Totals": 123,
    "Errors: Iteration": 73,
    "Errors: Teardown": 30,
    "Errors: Setup": 20,
    "Created Threads": 40
  },
  {
    "Time (s)": "20",
    "Min (ms)": 13,
    "Avg (ms)": 499,
    "Max (ms)": 980,
    "50% (ms)": 499,
    "80% (ms)": 743,
    "90% (ms)": 818,
    "95% (ms)": 874,
    "99% (ms)": 924,
    "Success: Count": 1052,
    "Errors: Totals": 202,
    "Errors: Iteration": 134,
    "Errors: Teardown": 46,
    "Errors: Setup": 22,
    "Created Threads": 60
  },
  {
    "Time (s)": "30",
    "Min (ms)": 110,
    "Avg (ms)": 577,
    "Max (ms)": 961,
    "50% (ms)": 607,
    "80% (ms)": 806,
    "90% (ms)": 888,
    "95% (ms)": 912,
    "99% (ms)": 957,
    "Success: Count": 54,
    "Errors: Totals": 8,
    "Errors: Iteration": 6,
    "Errors: Teardown": 2,
    "Errors: Setup": null,
    "Created Threads": 80
  }
]*/
