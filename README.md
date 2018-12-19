### Generic performance testing library for executing load-tests written in .NET c# ###
* NuGet: `Install-Package Viki.LoadRunner`

### v0.8-alpha (Should be final in early 2019):
 * v0.7 at this point should be considered obsolete, v0.8 while not final - it is the one im actively using&testing it.
   - test-setup changed in v0.8 and learning v0.7 should be a waste of time.
   - https://github.com/Vycka/LoadRunner/tree/v0.8
 * Only things that can still shift are namespaces and maybe patching few metrics to make more sense.
   - No changes except bug-fixes are planned to the core until v0.8 goes into stable version.
 


## *Quick Intro* [for obsolete v0.7.*]
* Take a look at [LoadRunner.Demo](/demo) project and follow commented code :)
  - [DemoTestScenario.cs](/demo/DemoTestScenario.cs) - Setup your Load-test scenario for single thread
  - [AggregationSetup.cs](/demo/AggregationSetup.cs) - Setup what data will be saved/meassured
  - [ParametersSetup.cs](/demo/ParametersSetup.cs) - Setup execution parameters (threads, duration, speed, etc...)
  - [QuickIntroLoadTest.cs](/demo/QuickIntroLoadTest.cs) - Put it all together
  - [Program.cs](/demo/Program.cs) - Run it
  - [RawDataAggregationDemo.cs](/demo/RawDataAggregationDemo.cs) - Advanced RnD feature to checkout.
* [DemoResults.xlsx](/demo/DemoResults.xlsx) - import results to excel and do some charting :)
![](https://raw.githubusercontent.com/Vycka/LoadRunner/master/diagrams/Architecture.png)
* [Future](../../wiki/TODOs)

***Sorry for poor documentation, I don't have much time to work on it ATM, but if having any questions/suggestions/etc, feel free to contact me.***

[![Analytics](https://ga-beacon.appspot.com/UA-71045586-1/LoadRunner/readme?pixel)](https://github.com/Vycka/LoadRunner)
