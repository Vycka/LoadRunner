### Load/Stress test library for executing tests written in c# ###
* NuGet: `Install-Package Viki.LoadRunner`

## *Quick Intro*
* If you are planning to use things like `WebRequest` for API tests, don't forget to lift the connection limit in .NET (`<connectionManagement><add address = "*" maxconnection = "100" /></connectionManagement>`)

### *ILoadTestScenario*
Implement `ILoadTestScenario` interface by defining test scenario for single thread instance.
 * [ILoadTestScenario documentation and example](/../../wiki/ILoadTestScenario)

### *Setup [LoadRunnerEngine] parameters with [LoadRunnerParameters]*
 * [Read more about IThreadingStrategies](/../../wiki/IThreadingStrategy)
```cs
// LoadRunnerParameters initializes defaults shown below
LoadRunnerParameters loadRunnerParameters = new LoadRunnerParameters
{
    Limits = new ExecutionLimits
    {
        // Maximum LoadTest duration threshold, after which test is stopped
        MaxDuration = TimeSpan.FromSeconds(30),
        
        // Maximum executet iterations count threshold, after which test is stopped
        MaxIterationsCount = Int32.MaxValue,

        // Once LoadTest execution finishes because of [maxDuration] or [maxIterationsCount] limit
        // coordinating thread will wait [FinishTimeout] amount of time before 
        // terminating them with Thread.Abort()
        //
        // Aborted threads won't get the chance to call IterationTearDown() or ScenarioTearDown()
        // neither it will broadcast TestContextResultReceived() to aggregators with the state as it is after abort.
        FinishTimeout = TimeSpan.FromSeconds(60)
    },

    // [ISpeedStrategy] defines maximum allowed load by dampening executed Iterations per second count
    // * Other existing version of [ISpeedStrategy]
    //    - IncremantalSpeed(initialRequestsPerSec: 1.0, increasePeriod: TimeSpan.FromSeconds(10), increaseStep: 3.0)
    SpeedStrategy = new FixedSpeed(maxIterationsPerSec: Double.MaxValue),

    // [IThreadingStrategy] defines allowed worker thread count
    ThreadingStrategy = new SemiAutoThreading(minThreadCount: 10, maxThreadCount: 10)
};
```

### *Choose your [IResultsAggregator]*
* [Read more about available IResultsAggregator's](/../../wiki/IResultsAggregator)
```cs
  // This aggregation is similar to SoapUI (Like Min, Max, Avg, ...)
  TotalsResultsAggregator resultsAggregator = new TotalsResultsAggregator();
  
  // This one aggregates same results as DefaultResultsAggregator, but splits into time-based histogram
  HistogramResultsAggregator histogramResultsAggregator = new TimeHistogramResultsAggregator(TimeSpan.FromSeconds(3));
```
* Histogram results can be exported to CSV using [HistogramCsvExport](src/Viki.LoadRunner.Engine/Utils/HistogramCsvExport.cs) util.

### *Put it all together*

```cs
  // Initialize parameters
  LoadRunnerParameters parameters = new LoadRunnerParameters();
  
  // Initialize aggregator
  DefaultResultsAggregator resultsAggregator = new DefaultResultsAggregator();
  
  // Initializing LoadTest Client
  LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<TestScenario>(parameters, resultsAggregator);
  
  // Run test (blocking call)
  loadRunner.Run();
  
  // results will contain all logged exceptions within LoadTest execution
  ResultsContainer results = resultsAggregator.GetResults();
  Console.WriteLine(JsonConvert.SerializeObject(defaultResults, Formatting.Indented));
```
### *Enjoy the result*

```js
{
  "Results": [
    {
      "Name": "First Checkpoint",
      "MomentMin": "00:00:00.0151666",
      "MomentMax": "00:00:03.9400971",
      "MomentAvg": "00:00:01.9450000",
      "SummedMin": "00:00:00.0161491",
      "SummedMax": "00:00:03.9400971",
      "SummedAvg": "00:00:01.9460000",
      "SuccessIterationsPerSec": 3.897076831011435,
      "Count": 132,
      "ErrorRatio": 0.0,
      "ErrorCount": 0
    },
    {
      "Name": "Last Checkpoint",
      "MomentMin": "00:00:00",
      "MomentMax": "00:00:00",
      "MomentAvg": "00:00:00",
      "SummedMin": "00:00:00.0161491",
      "SummedMax": "00:00:03.9400971",
      "SummedAvg": "00:00:01.9330000",
      "SuccessIterationsPerSec": 3.5723204284271488,
      "Count": 121,
      "ErrorRatio": 0.083333333333333343,
      "ErrorCount": 11
    },
    {
      "Name": "SYS_ITERATION_END",
      "MomentMin": "00:00:00.0019844",
      "MomentMax": "00:00:00.9949691",
      "MomentAvg": "00:00:00.4980000",
      "SummedMin": "00:00:00.2472860",
      "SummedMax": "00:00:04.6650285",
      "SummedAvg": "00:00:02.4470000",
      "SuccessIterationsPerSec": 3.5427971191013041,
      "Count": 120,
      "ErrorRatio": 0.0082644628099173556,
      "ErrorCount": 1
    }
  ],
  "Totals": {
    "TotalIterationsStartedCount": 134,
    "TotalSuccessfulIterationCount": 120,
    "TotalFailedIterationCount": 14,
    "TotalDuration": "00:00:33.8715416",
    "AverageWorkingThreads": 10.664179104477611,
    "IterationErrorsRatio": 0.1044776119402985,
    "SuccessIterationsPerSec": 3.5427971191013041,
    "IterationSetupErrorCount": 2,
    "IterationTearDownErrorCount": 5
  }
}
```
