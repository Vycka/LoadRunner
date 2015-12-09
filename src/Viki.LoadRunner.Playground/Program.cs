using System;
using System.Linq;
using Viki.LoadRunner.Engine.Executor.Result;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main()
        {
            //ReadmeDemo.Run();

            var res = JsonStream.DeserializeSequenceFromJson<ReplayResult<Exception>>("d:\\test.stream.json")
                .Select(r => (IResult)r).ToList();

            //new ReplayResult<Exception>().
        }
    }
}
