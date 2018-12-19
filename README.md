### Generic performance testing library for executing load-tests written in .NET c# ###
* NuGet: `Install-Package Viki.LoadRunner -pre`

## *Quick Intro*
* Basically one needs to define 3 things to make fully working test:
  - [1] Write Scenario implementation, defining code it-self which will get executed multiple times on multiple threads
  - [2][Optional] Configure how results get aggregated/presented
  - [3] Configure strategy on how test  gets executed (e.g. How much threads, how long it should run. etc...)

* Start with this small setup to get a feel on how it's configured.
  - [QuickStartDemo.cs](/demo/QuickStart/QuickStartDemo.cs)
* Later continue on documented topics either through [Program.cs](/demo/Program.cs) or links below:
  - [At the moment only the QuickStart is available]
* Until demo project is completed, one can also checkout my messy setups i use for debugging:
  - [src/Playground/Program.cs](/src/Viki.LoadRunner.Playground/Program.cs)
  
* [Future](../../wiki/TODOs)

***Have any questions/suggestions/etc, feel free to contact me.***

[![Analytics](https://ga-beacon.appspot.com/UA-71045586-1/LoadRunner/readme?pixel)](https://github.com/Vycka/LoadRunner)
