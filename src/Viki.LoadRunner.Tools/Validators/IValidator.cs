using Viki.LoadRunner.Engine.Core.Collector;

namespace Viki.LoadRunner.Tools.Validators
{
    public interface IValidator
    {
        IterationResult Validate();
    }
}