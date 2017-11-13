using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;
using Viki.LoadRunner.Tools.Validators;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class LoadRunnerUiBuilderExtensions
    {
        public static LoadRunnerUi BuildUi<TData>(this ReplayStrategyBuilder<TData> settings, DataItem validationDataItem = null)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            ReplayStrategyBuilder<TData> localSettings = settings.Clone();

            localSettings.AddAggregator(ui);

            ReplayScenarioValidator<TData> validator = validationDataItem != null
                ? new ReplayScenarioValidator<TData>(settings.ScenarioFactory, validationDataItem)
                : null;

            ui.Setup(new ReplayStrategy<TData>(localSettings), validator);

            return ui;
        }

        public static LoadRunnerUi BuildUi(this StrategyBuilder settings)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            var localSettings = settings.Clone();

            localSettings.AddAggregator(ui);

            ui.Setup(new CustomStrategy(localSettings), new DefaultValidator(settings.ScenarioFactory));

            return ui;
        }
    }
}