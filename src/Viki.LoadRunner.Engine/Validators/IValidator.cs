using Viki.LoadRunner.Engine.Core.Collector;

namespace Viki.LoadRunner.Engine.Validators
{
    public interface IValidator
    {
        IterationResult Validate(int threadId = 0, int threadIterationId = 0, int globalIterationId = 0);
    }
}