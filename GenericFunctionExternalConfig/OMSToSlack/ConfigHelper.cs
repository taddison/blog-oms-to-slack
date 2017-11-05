using OMSToSlack.Models.Configs;
using System.Collections.Generic;
using System;
using OMSToSlack.Models;
using System.Linq;

namespace OMSToSlack
{
    public static class ConfigHelper
    {
        public static AlertConfig GetAlertConfig(Alert alert)
        {
            var defaultConfig = GetDefaultAlertConfigs().Single(c => c.MetricName == alert.MetricName);

            var valueMultiplier = defaultConfig.ValueMultiplier;
            var lessThanThresholdIsBad = defaultConfig.LessThanThresholdIsBad;
            var minimumViolations = defaultConfig.MinimumViolationsToAlert;
            var warning = defaultConfig.WarningThreshold;
            var critical = defaultConfig.CriticalThreshold;

            var overrideConfig = GetOverrideAlertConfigs().FirstOrDefault(c => c.MetricName == alert.MetricName && c.MachineName == alert.MachineName);

            warning = overrideConfig?.WarningThreshold ?? warning;
            critical = overrideConfig?.CriticalThreshold ?? critical;
            minimumViolations = overrideConfig?.MinimumViolationsToAlert ?? minimumViolations;

            return new AlertConfig(
                warning
                , critical
                , lessThanThresholdIsBad
                , minimumViolations
                , valueMultiplier
                );
        }

        private static List<DefaultAlertConfig> GetDefaultAlertConfigs()
        {
            // TODO: Unique on metricName
            var configs = new List<DefaultAlertConfig>
            {
                new DefaultAlertConfig("Processor Usage %", 0.35, 0.5, false, 3, 0.01),
                new DefaultAlertConfig("Free Space %", 0.2, 0.1, true, 1, 0.01),
                new DefaultAlertConfig("Free Megabytes", 10000, 5000, true, 1, 1)
            };

            return configs;
        }

        private static List<OverrideAlertConfig> GetOverrideAlertConfigs()
        {
            // TODO: Unique on metric + machine names
            var configs = new List<OverrideAlertConfig>
            {
                new OverrideAlertConfig("Processor Usage %", "Server2", 0.5, 0.8, 3)
            };

            return configs;
        }
    }
}
