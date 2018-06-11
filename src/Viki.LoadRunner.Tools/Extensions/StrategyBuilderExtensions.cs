using System;
using Viki.LoadRunner.Engine.Strategies.Extensions;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Validators;
using Viki.LoadRunner.Tools.Windows;

namespace Viki.LoadRunner.Tools.Extensions
{
    public static class StrategyBuilderExtensions
    {
        public static LoadRunnerUi BuildUi(this IStrategyBuilder builder, IValidator validator = null)
        {
            if (!(builder is IAggregatorFeature))
                throw new ArgumentException("Strategy builder must support IAggrregatorFeature");

            LoadRunnerUi ui = new LoadRunnerUi();
            IAggregatorFeature localBuilder = (IAggregatorFeature)builder.ShallowCopy();

            localBuilder.AddAggregator(ui);

            ui.Setup(localBuilder.BuildStrategy(), validator);

            return ui;
        }


    }
}