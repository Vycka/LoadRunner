### Generic performance testing library for executing load-tests written in .NET c# ###
* NuGet: `Install-Package Viki.LoadRunner -pre`

## *Quick Intro*
* Basically one needs to define 3 things to make fully working test:
  - [1] Write Scenario implementation, defining code it-self which will get executed multiple times on multiple threads
  - [2] Configure strategy on how it will get executed (e.g. How much threads, how long it should run. etc...)
  - [3][Optional] Configure how results get aggregated/presented
  
* Take a look at [LoadRunner.Demo](/demo) project and follow commented code :)
  - [[1] Scnario.cs](/demo/Detailed/Scenario.cs) - Setup your Load-test scenario for single thread
  - [[2] Strategy.cs](/demo/Detailed/Strategy.cs) - Setup execution settings (threads, duration, speed, etc...)
  - [[3] Aggregation.cs](/demo/Detailed/Aggregation.cs) - Setup what data will be saved/meassured
  - [DetailedDemo.cs](/demo/Detailed/DetailedDemo.cs) - Put it all together
  - [Program.cs](/demo/Program.cs) - Run it
  - [DemoResults.xlsx](/demo/DemoResults.xlsx) - import results to excel and do some charting :)
  
 * Alternative Bare-Minimum example
   - [MinimalDemo.cs](/demo/Minimum/MinimalDemo.cs)
   
 * Other nifty features   
   - [RawDataAggregation.cs](/demo/Features/RawDataAggregation.cs) - Advanced RnD feature to checkout.
   - [KpiOutput.cs](/demo/Features/KpiOutput.cs) - TODO 404
   - [ReplayDemo.cs](/demo/Features/ReplayDemo.cs) - TODO 404 ()
     - ReplayStrategy RnD (e.g. for replaying logs)
     - https://github.com/Vycka/LoadRunner/blob/v0.8/src/Viki.LoadRunner.Playground/Replay/ReplayDemo.cs
  
![](https://raw.githubusercontent.com/Vycka/LoadRunner/master/diagrams/Architecture.png)
* [Future](../../wiki/TODOs)

***Sorry for poor documentation, I don't have much time to work on it ATM, but if having any questions/suggestions/etc, feel free to contact me.***

[![Analytics](https://ga-beacon.appspot.com/UA-71045586-1/LoadRunner/readme?pixel)](https://github.com/Vycka/LoadRunner)
