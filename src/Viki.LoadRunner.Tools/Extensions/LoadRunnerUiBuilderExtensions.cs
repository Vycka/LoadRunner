using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Custom;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class LoadRunnerUiBuilderExtensions
    {
        public static LoadRunnerUi BuildUi<>(this ReplayStrategyBuilder settings)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            settings.Add(ui);

            ui.Setup(new ReplayStrategy(settings), settings.TestScenarioType);

            return ui;
        }

        public static LoadRunnerUi BuildUi(this StrategyBuilder settings)
        {
            LoadRunnerUi ui = new LoadRunnerUi();
            settings.Add(ui);

            ui.Setup(new CustomStrategy(settings), settings.TestScenarioType);

            return ui;
        }
    }
}