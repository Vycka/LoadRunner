using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom;
using Viki.LoadRunner.Engine.Strategies.Custom.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;
using Viki.LoadRunner.Engine.Validators;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class LoadRunnerUiBuilderExtensions
    {
        public static LoadRunnerUi BuildUi<TData>(this ReplayStrategyBuilder<TData> settings, DataItem validationDataItem = null)
        {
            return BuildUi<TData>((IReplayStrategySettings)settings, validationDataItem);
        }

        public static LoadRunnerUi BuildUi<TData>(this IReplayStrategySettings settings, DataItem validationDataItem = null)
        {
            ReplayScenarioValidator<TData> validator = validationDataItem != null
                ? new ReplayScenarioValidator<TData>(settings.ScenarioFactory, validationDataItem)
                : null;

            return BuildUi<TData>(settings, validator);
        }

        public static LoadRunnerUi BuildUi<TData>(this ReplayStrategyBuilder<TData> settings, IValidator validator)
        {
            return BuildUi<TData>((IReplayStrategySettings)settings, validator);
        }

        public static LoadRunnerUi BuildUi<TData>(this IReplayStrategySettings settings, IValidator validator)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            ReplayStrategyBuilder<TData> localSettings = settings.ShallowClone<TData>();

            localSettings.AddAggregator(ui);

            ui.Setup(new ReplayStrategy<TData>(localSettings), validator);

            return ui;
        }

        public static LoadRunnerUi BuildUi(this ICustomStrategySettings settings)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            var localSettings = settings.ShallowClone();

            localSettings.AddAggregator(ui);

            ui.Setup(new CustomStrategy(localSettings), new DefaultValidator(settings.ScenarioFactory));

            return ui;
        }
    }
}