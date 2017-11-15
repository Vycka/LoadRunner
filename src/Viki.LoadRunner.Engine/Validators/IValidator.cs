using Viki.LoadRunner.Engine.Core.Collector;

namespace Viki.LoadRunner.Engine.Validators
{
    public interface IValidator
    {
        IterationResult Validate();
    }
}