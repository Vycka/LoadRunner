### Generic performance testing library for executing load-tests written in .NET c# ###
* NuGet: `Install-Package Viki.LoadRunner -pre`

## *Quick Intro*
* Basically one needs to define 3 things to make fully working test:
  - [1] Write Scenario implementation, defining code it-self which will get executed multiple times on multiple threads
  - [2][Optional] Configure how results get aggregated/presented
  - [3] Configure strategy on how it will get executed (e.g. How much threads, how long it should run. etc...)

* Bare-Minimum example
   - [MinimalDemo.cs](/demo/Minimum/MinimalDemo.cs)  
   
* Proper demo is now being rewritten, stuff below might be outdated
  
* [Future](../../wiki/TODOs)

***Sorry for poor documentation, I don't have much time to work on it ATM, but if having any questions/suggestions/etc, feel free to contact me.***

[![Analytics](https://ga-beacon.appspot.com/UA-71045586-1/LoadRunner/readme?pixel)](https://github.com/Vycka/LoadRunner)
