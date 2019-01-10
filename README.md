### Generic performance testing library for executing load-tests written in .NET c# ###
* Originally aimed for developers (and up to corner case scenarios) for maximum flexibility.
* But easy enough to use as writing integration tests - just for more threads. :)
* NuGet: `Install-Package Viki.LoadRunner -pre`


## *Quick Intro*
Current documentation is only in form of code examples. 
* It's far from completed, but IMO its should be good enough to see whether this tool can be useful.

Start with this small setup to get a feel on how it's configured.
* One needs to define 3 things to make fully working test (See [QuickStartDemo.cs](/demo/Guides/QuickStart/QuickStartDemo.cs)):
  - [1] Write Scenario implementation, defining code it-self which will get executed concurrently multiple times.
  - [2][Optional] Configure how results get aggregated/presented.
  - [3] Configure strategy on how test gets executed (e.g. How much threads, how long it should run. etc...)

HistogramAggregator is a powerful tool of its own which can aggregate any generic data.
 * Given its flexibility documentation will take time till its done.
 * Until then - it should be easy enough to figure out its workings through various examples already available:
  - [HistogramAggregatorDemo.cs#L30](/demo/Guides/Aggregation/HistogramAggregatorDemo.cs#L30) - WiP
  - [QuickStartDemo.cs#L64](/demo/Guides/QuickStart/QuickStartDemo.cs#L64)
  - [RawDataMeasurementsDemo.cs#L59](/demo/Guides/Aggregation/RawDataMeasurementsDemo.cs#L59)
  - [AggregationImpactDemo.cs#L39](/demo/Theoretical/AggregationImpactDemo.cs#L39)

Rest of the demo project:
 * Index
  - [Program.cs](/demo/Program.cs)
 * Feature specific demos (WiP)
  - [RawDataMeasurementsDemo.cs](/demo/Guides/Aggregation/RawDataMeasurementsDemo.cs)
  - [ScenarioFactoryDemo.cs](/demo/Guides/StrategyBuilderFeatures/ScenarioFactoryDemo.cs)
 * Engine throughut (WiP)
  - [TheoreticalSpeedDemo.cs](demo/Theoretical/TheoreticalSpeedDemo.cs) - Theoretical throughput without doing any measurements.
  - [AggregationImpactDemo.cs](demo/Theoretical/AggregationImpactDemo.cs) - Theoretical throughput with aggregation pipeline attached.
  
Until demo project is completed, one can also checkout my messy setups i use for debugging:
 * [src/Playground/Program.cs](/src/Viki.LoadRunner.Playground/Program.cs)
  
[Future](../../wiki/TODOs)

***Have any questions/suggestions/etc, feel free to contact me.***

[![Analytics](https://ga-beacon.appspot.com/UA-71045586-1/LoadRunner/readme?pixel)](https://github.com/Vycka/LoadRunner)
