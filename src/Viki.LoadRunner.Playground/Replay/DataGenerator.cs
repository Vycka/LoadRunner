using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;

namespace Viki.LoadRunner.Playground.Replay
{
    public class DataGenerator
    {
        public static IEnumerable<DataItem> Create(int bigStep, int smallStep, int bigSteps, int smallSteps)
        {
            for (int i = 0; i < bigSteps; i++)
            {
                int offset = i * bigStep;
                for (int j = 0; j < smallSteps; j++)
                {
                    int delaySeconds = offset + smallStep * j;
                    yield return new DataItem(TimeSpan.FromSeconds(delaySeconds), $"i:{i} j:{j} t:{delaySeconds}");
                }
            }
        }
    }
}