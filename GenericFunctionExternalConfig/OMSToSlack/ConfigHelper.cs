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

            var overrideConfig = GetOverrideAlertConfigs().FirstOrDefault(c => c.MetricName == alert.MetricName && c.MachineName == alert.MachineName);

            return new AlertConfig(
                overrideConfig?.WarningThreshold ?? defaultConfig.WarningThreshold
                , overrideConfig?.CriticalThreshold ?? defaultConfig.CriticalThreshold
                , defaultConfig.LessThanThresholdIsBad
                , overrideConfig?.MinimumViolationsToAlert ?? defaultConfig.MinimumViolationsToAlert
                , defaultConfig.ValueMultiplier
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
                new OverrideAlertConfig("Processor Usage %", "Server2", 0.2, 0.4, 3)
            };

            return configs;
        }

        public static AlertNotificationConfig GetAlertNotificationConfig(Alert alert)
        {
            var defaultConfig = GetDefaultAlertNotificationConfigs().Single(c => c.MetricName == alert.MetricName);
            var channels = defaultConfig.Channels;
            var isDefaultChannels = true;
            var overrides = GetOverrideAlertNotificationConfigs();

            // MachineName + MetricName override
            var exactMatch = overrides.FirstOrDefault(o =>
                (o.MachineName != null && o.MetricName != null)
                && o.MachineName == alert.MachineName
                && o.MetricName == o.MetricName
                && o.Channels?.Count() > 0);
            if(exactMatch != null)
            {
                // Overwrite
                channels = exactMatch.Channels;
                isDefaultChannels = false;
            }

            // MachineName override
            var machineMatch = overrides.FirstOrDefault(o => 
                o.MachineName != null 
                && o.MetricName == null 
                && o.MachineName == alert.MachineName 
                && o.Channels?.Count() > 0);
            if(machineMatch != null)
            {
                if(isDefaultChannels == true)
                {
                    channels = machineMatch.Channels;
                    isDefaultChannels = false;
                }
                else
                {
                    channels = channels.Union(machineMatch.Channels).ToList();
                }
            }

            // MetricName override
            var metricMatch = overrides.FirstOrDefault(o =>
                o.MetricName != null
                && o.MachineName == null
                && o.MetricName == alert.MetricName
                && o.Channels?.Count() > 0);
            if (metricMatch != null)
            {
                if (isDefaultChannels == true)
                {
                    channels = metricMatch.Channels;
                    isDefaultChannels = false;
                }
                else
                {
                    channels = channels.Union(metricMatch.Channels).ToList();
                }
            }

            return new AlertNotificationConfig(
                defaultConfig.FormatString
                , defaultConfig.AlertMessage
                , channels
            );
        }

        private static List<DefaultAlertNotificationConfig> GetDefaultAlertNotificationConfigs()
        {
            var defaultChannels = new List<string> { "#alerts" };

            var configs = new List<DefaultAlertNotificationConfig>
            {
                new DefaultAlertNotificationConfig("Processor Usage %", defaultChannels, "P0", "Infra - CPU"),
                new DefaultAlertNotificationConfig("Free Space %", defaultChannels, "P0", "Infra - Drive Space"),
                new DefaultAlertNotificationConfig("Free Megabytes", defaultChannels, "N0", "Infra - Memory")
            };

            return configs;
        }

        private static List<OverrideAlertNotificationConfig> GetOverrideAlertNotificationConfigs()
        {
            var configs = new List<OverrideAlertNotificationConfig>
            {
                new OverrideAlertNotificationConfig(
                    machineName: "Server1"
                    ,channels: new List<string> { "#Server1Team" }
                ),
                new OverrideAlertNotificationConfig(
                    metricName: "Free Space %"
                    ,channels: new List<string> { "#memory-monitors" }
                ),
                new OverrideAlertNotificationConfig(
                    metricName: "Processor Usage %"
                    ,machineName: "Server2"
                    ,channels: new List<string> { "#server2-cpu" }
                )
            };

            return configs;
        }
    }
}
