### Load/Stress generic test library for executing load-tests written in c# ###
* NuGet: `Install-Package Viki.LoadRunner`


## *Quick Intro*
* Take a look at [LoadRunner.Demo](/demo) project and follow commented code :)
  - [DemoTestScenario.cs](/demo/DemoTestScenario.cs) - Setup your Load-test scenario for single thread
  - [AggregationSetup.cs](/demo/AggregationSetup.cs) - Setup what will data be meassured
  - [ParametersSetup.cs](/demo/ParametersSetup.cs) - Setup execution parameters (threads, duration, speed, etc...)
  - [QuickIntroLoadTest.cs](/demo/QuickIntroLoadTest.cs) - Put it all together
  - [Program.cs](/demo/Program.cs) - Run it
* [DemoResults.xlsx](/demo/DemoResults.xlsx) to see what kind of output to expect from this tool.
![](https://raw.githubusercontent.com/Vycka/LoadRunner/master/diagrams/Architecture.png)

[](https://ga-beacon.appspot.com/UA-71045586-1/LoadRunner/readme?pixel)
