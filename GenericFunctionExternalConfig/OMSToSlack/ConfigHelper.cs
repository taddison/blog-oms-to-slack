using CsvHelper;
using OMSToSlack.Models;
using OMSToSlack.Models.Configs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMSToSlack
{
    public static class ConfigHelper
    {
        private static List<DefaultAlertConfig> __defaultAlertConfigs;
        private static List<OverrideAlertConfig> __overrideAlertConfigs;
        private static List<DefaultAlertNotificationConfig> __defaultAlertNotificationConfigs;
        private static List<OverrideAlertNotificationConfig> __overrideAlertNotificationConfigs;

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

        public static AlertNotificationConfig GetAlertNotificationConfig(Alert alert)
        {
            var defaultConfig = GetDefaultAlertNotificationConfigs().Single(c => c.MetricName == alert.MetricName);
            var channels = new List<string> { defaultConfig.Channel };
            var isDefaultChannels = true;
            var overrides = GetOverrideAlertNotificationConfigs();

            // TODO: Support for multiple matches on overrides [to add multiple channels]

            // MachineName + MetricName override
            var exactMatch = overrides.FirstOrDefault(o =>
                (o.MachineName != null && o.MetricName != null)
                && o.MachineName == alert.MachineName
                && o.MetricName == o.MetricName
                && o.Channel?.Count() > 0);
            if(exactMatch != null)
            {
                // Overwrite
                channels = new List<string> { exactMatch.Channel };
                isDefaultChannels = false;
            }

            // MachineName override
            var machineMatch = overrides.FirstOrDefault(o => 
                o.MachineName != null 
                && o.MetricName == null 
                && o.MachineName == alert.MachineName 
                && o.Channel?.Count() > 0);
            if(machineMatch != null)
            {
                if(isDefaultChannels)
                {
                    channels = new List<string> { machineMatch.Channel };
                    isDefaultChannels = false;
                }
                else
                {
                    channels = channels.Union(new List<string> { machineMatch.Channel }).ToList();
                }
            }

            // MetricName override
            var metricMatch = overrides.FirstOrDefault(o =>
                o.MetricName != null
                && o.MachineName == null
                && o.MetricName == alert.MetricName
                && o.Channel?.Count() > 0);
            if (metricMatch != null)
            {
                if (isDefaultChannels)
                {
                    channels = new List<string> { metricMatch.Channel };
                    isDefaultChannels = false;
                }
                else
                {
                    channels = channels.Union(new List<string> { metricMatch.Channel }).ToList();
                }
            }

            return new AlertNotificationConfig(
                defaultConfig.FormatString
                , defaultConfig.AlertMessage
                , channels
            );
        }

        private static List<DefaultAlertConfig> GetDefaultAlertConfigs()
        {
            if(__defaultAlertConfigs == null)
            {
                using (var tr = File.OpenText("./Configuration/defaultAlertConfig.csv"))
                {
                    // TODO: Unique on metricName
                    var csv = new CsvReader(tr);
                    var configs = csv.GetRecords<DefaultAlertConfig>();
                    __defaultAlertConfigs = configs.ToList();
                }
            }

            return __defaultAlertConfigs;
        }

        private static List<OverrideAlertConfig> GetOverrideAlertConfigs()
        {
            if(__overrideAlertConfigs == null)
            {
                // TODO: Unique on metric + machine names
                using (var tr = File.OpenText("./Configuration/overrideAlertConfig.csv"))
                {
                    var csv = new CsvReader(tr);
                    var configs = csv.GetRecords<OverrideAlertConfig>();
                    __overrideAlertConfigs = configs.ToList();
                }
            }

            return __overrideAlertConfigs;
        }

        private static List<DefaultAlertNotificationConfig> GetDefaultAlertNotificationConfigs()
        {
            if( __defaultAlertNotificationConfigs is null)
            {
                using (var tr = File.OpenText("./Configuration/defaultAlertNotificationConfig.csv"))
                {
                    var csv = new CsvReader(tr);
                    var configs = csv.GetRecords<DefaultAlertNotificationConfig>();
                    __defaultAlertNotificationConfigs = configs.ToList();
                }
            }

            return __defaultAlertNotificationConfigs;
        }

        private static List<OverrideAlertNotificationConfig> GetOverrideAlertNotificationConfigs()
        {
            if(__overrideAlertNotificationConfigs == null)
            {
                using (var tr = File.OpenText("./Configuration/overrideAlertNotificationConfig.csv"))
                {
                    var csv = new CsvReader(tr);
                    var configs = csv.GetRecords<OverrideAlertNotificationConfig>();
                    __overrideAlertNotificationConfigs = configs.ToList();
                }
            }

            return __overrideAlertNotificationConfigs;
        }
    }
}
