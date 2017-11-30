using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom;
using Viki.LoadRunner.Engine.Strategies.Custom.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Validators;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class LoadRunnerUiBuilderExtensions
    {
        public static LoadRunnerUi BuildUi<TData>(this ReplayStrategyBuilder<TData> settings, DataItem validationDataItem = null)
        {
            return BuildUi((IReplayStrategySettings<TData>)settings, validationDataItem);
        }

        public static LoadRunnerUi BuildUi<TData>(this IReplayStrategySettings<TData> settings, DataItem validationDataItem = null)
        {
            ReplayScenarioValidator<TData> validator = validationDataItem != null
                ? new ReplayScenarioValidator<TData>(settings.ScenarioFactory, validationDataItem)
                : null;

            return BuildUi(settings, validator);
        }

        public static LoadRunnerUi BuildUi<TData>(this ReplayStrategyBuilder<TData> settings, IValidator validator)
        {
            return BuildUi((IReplayStrategySettings<TData>)settings, validator);
        }

        public static LoadRunnerUi BuildUi<TData>(this IReplayStrategySettings<TData> settings, IValidator validator)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            ReplayStrategyBuilder<TData> localSettings = settings.ShallowClone();

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