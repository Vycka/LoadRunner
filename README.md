### Load/Stress test library for executing tests written in c#
* NuGet: `Install-Package Viki.LoadRunner -Pre`

## *Quick Intro*

### *ITestScenario*
Implement `ITestScenario` interface by defining test scenario for single thread instance.
Each worker-thread will create its own `ITestScenario` instance and will keep it persistent until the test is over.
```cs
public class TestScenario : ITestScenario
{
    private static readonly Random Random = new Random(42);

    public void ExecuteScenario(ITestContext testContext)
    {
        Console.WriteLine(
            "ExecuteScenario defines single iteration for load test scenario, " +
            "It is called after each successful IterationSetup call. " +
            "Execution time is meassured only for this function" +
            "You can use testContext.Checkpoint() function to mark points, where time should be meassured"
        );
        Thread.Sleep(Random.Next(5000));

        testContext.Checkpoint("First Checkpoint");

        if (Random.Next(100) % 10 == 0)
            throw new Exception("random err"); 

        testContext.Checkpoint("Last Checkpoint");
        Thread.Sleep(Random.Next(1000));

        if (Random.Next(100) % 100 == 0)
            throw new Exception("random err 2");
    }

    public void ScenarioSetup(ITestContext testContext)
    {
        Console.WriteLine("ScenarioSetup Executes on thread creation");
        Console.WriteLine("Exceptions here are not handled!");
    }

    public void ScenarioTearDown(ITestContext testContext)
    {
        Console.WriteLine("ScenarioTearDown Executes once LoadTest execution is over");
        //(unless thread is terminated by finishTimeoutMilliseconds abort)
        
        Console.WriteLine("Exceptions here are not handled!");
    }

    public void IterationSetup(ITestContext testContext)
    {
        Console.WriteLine("IterationSetup is executed before each ExecuteScenario call");
    }

    public void IterationTearDown(ITestContext testContext)
    {
        Console.WriteLine("IterationTearDown is executed each time after ExecuteScenario iteration is finished");
        //(unless thread is terminated by finishTimeoutMilliseconds abort)
    }
}
```
### *Configure Load-test runner parameters*
```cs
ExecutionParameters executionParameters = new ExecutionParameters(

    // Maximum LoadTest duration (after it load test stops)
    maxDuration: TimeSpan.FromSeconds(15),

    // Maximum iteratations count (after it load test stops)
    maxIterationsCount: 2000,

    // Minimum amount of worker-threads to precreate
    minThreads: 1,

    // Maximum amount of worker-threads that can be created 
    // New threads only be created if maxRequestsPerSecond speed is not achieved
    // with current amount of threads          
    maxThreads: 200,

    // Maximum count of requests that will be created per second
    // (Unless there are no available worker-threads left)
    maxRequestsPerSecond: 20,

    // Once LoadTest execution finishes because of maxDuration or maxIterationsCount limit
    // Coordinating thread will wait finishTimeoutMilliseconds amount of time before 
    // terminating them with Thread.Abort()
    finishTimeoutMilliseconds: 60000
);
```

### Choose your Load-test aggregation
There is already one created which gives similar stats to SoapUI
```cs
  DefaultResultsAggregator resultsAggregator = new DefaultResultsAggregator();
```

Or `IResultsAggregator` interface could be implemented, thus giving access to raw meassurements
```cs
    public interface IResultsAggregator
    {
        /// <summary>
        /// Results from all running threads will be poured into this one.
        /// </summary>
        void TestContextResultReceived(TestContextResult result);
    }
```

### *Put it all together*

```cs
ExecutionParameters executionParameters = new ExecutionParameters(
        maxDuration: TimeSpan.FromSeconds(15),
        maxIterationsCount: 2000,
        minThreads: 1,
        maxThreads: 200,
        maxRequestsPerSecond: 20,
        finishTimeoutMilliseconds: 60000
    );
    
DefaultResultsAggregator resultsAggregator = new DefaultResultsAggregator();

// Initializing LoadTest Client
LoadTestClient testClient = LoadTestClient.Create<TestScenario>(executionParameters,resultsAggregator);

// Run test (blocking call)
testClient.Run();

// ResultItem will have all logged exceptions within LoadTest execution
List<ResultItem> results = resultsAggregator.BuildResultsObject();
Console.WriteLine(JsonConvert.SerializeObject(results, Formatting.Indented));
```
## *Enjoy the result*

```
[
  {
    "Name": "SYS_ITERATION_START",
    "MomentMin": "00:00:00",
    "MomentMax": "00:00:00",
    "MomentAverage": "00:00:00",
    "TotalMin": "00:00:00",
    "TotalMax": "00:00:00",
    "TotalAverage": "00:00:00",
    "CountPerSecond": "Infinity",
    "Count": 296,
    "ErrorCount": 0,
    "ErrorRate": 0.0
  },
  {
    "Name": "First Checkpoint",
    "MomentMin": "00:00:00.0100870",
    "MomentMax": "00:00:04.9481776",
    "MomentAverage": "00:00:02.4350000",
    "TotalMin": "00:00:00.0100870",
    "TotalMax": "00:00:04.9481776",
    "TotalAverage": "00:00:02.4350000",
    "CountPerSecond": 0.41060473783667639,
    "Count": 296,
    "ErrorCount": 29,
    "ErrorRate": 0.097972972972972971
  },
  {
    "Name": "Last Checkpoint",
    "MomentMin": "00:00:00.0000012",
    "MomentMax": "00:00:00.0000157",
    "MomentAverage": "00:00:00",
    "TotalMin": "00:00:00.0100900",
    "TotalMax": "00:00:04.9481800",
    "TotalAverage": "00:00:02.4670000",
    "CountPerSecond": 372540.81205525325,
    "Count": 267,
    "ErrorCount": 3,
    "ErrorRate": 0.011235955056179775
  },
  {
    "Name": "SYS_ITERATION_END",
    "MomentMin": "00:00:00.0000033",
    "MomentMax": "00:00:00.9994817",
    "MomentAverage": "00:00:00.4440000",
    "TotalMin": "00:00:00.1087478",
    "TotalMax": "00:00:05.6061666",
    "TotalAverage": "00:00:02.8790000",
    "CountPerSecond": 2.2527981406353974,
    "Count": 296,
    "ErrorCount": 0,
    "ErrorRate": 0.0
  }
]
```
