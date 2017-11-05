using CsvHelper;
using Microsoft.Azure.WebJobs;
using OMSToSlack.Models;
using OMSToSlack.Models.Configs;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OMSToSlack
{
    public class ConfigHelper
    {
        private ExecutionContext _context;
        private List<DefaultAlertConfig> _defaultAlertConfigs;
        private List<OverrideAlertConfig> _overrideAlertConfigs;
        private List<DefaultAlertNotificationConfig> _defaultAlertNotificationConfigs;
        private List<OverrideAlertNotificationConfig> _overrideAlertNotificationConfigs;

        public ConfigHelper(ExecutionContext context)
        {
            _context = context;
            _defaultAlertConfigs = GetDefaultAlertConfigs();
            _overrideAlertConfigs = GetOverrideAlertConfigs();
            _defaultAlertNotificationConfigs = GetDefaultAlertNotificationConfigs();
            _overrideAlertNotificationConfigs = GetOverrideAlertNotificationConfigs();
        }

        public AlertConfig GetAlertConfig(Alert alert, ExecutionContext context)
        {
            var defaultConfig = _defaultAlertConfigs.Single(c => c.MetricName == alert.MetricName);
            
            var overrideConfig = _overrideAlertConfigs.FirstOrDefault(c => c.MetricName == alert.MetricName && c.MachineName == alert.MachineName);

            return new AlertConfig(
                overrideConfig?.WarningThreshold ?? defaultConfig.WarningThreshold
                , overrideConfig?.CriticalThreshold ?? defaultConfig.CriticalThreshold
                , defaultConfig.LessThanThresholdIsBad
                , overrideConfig?.MinimumViolationsToAlert ?? defaultConfig.MinimumViolationsToAlert
                , defaultConfig.ValueMultiplier
                );
        }

        public AlertNotificationConfig GetAlertNotificationConfig(Alert alert)
        {
            var defaultConfig = _defaultAlertNotificationConfigs.Single(c => c.MetricName == alert.MetricName);
            var channels = new List<string> { defaultConfig.Channel };
            var isDefaultChannels = true;
            var overrides = _overrideAlertNotificationConfigs;

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

        private List<DefaultAlertConfig> GetDefaultAlertConfigs()
        {
            using (var tr = File.OpenText(_context.FunctionAppDirectory + "/Configuration/defaultAlertConfig.csv"))
            {
                // TODO: Unique on metricName
                var csv = new CsvReader(tr);
                return csv.GetRecords<DefaultAlertConfig>().ToList();
            }
        }

        private List<OverrideAlertConfig> GetOverrideAlertConfigs()
        {
            // TODO: Unique on metric + machine names
            using (var tr = File.OpenText(_context.FunctionAppDirectory + "/Configuration/overrideAlertConfig.csv"))
            {
                var csv = new CsvReader(tr);
                return csv.GetRecords<OverrideAlertConfig>().ToList();
            }
        }

        private List<DefaultAlertNotificationConfig> GetDefaultAlertNotificationConfigs()
        {
            using (var tr = File.OpenText(_context.FunctionAppDirectory + "/Configuration/defaultAlertNotificationConfig.csv"))
            {
                var csv = new CsvReader(tr);
                return csv.GetRecords<DefaultAlertNotificationConfig>().ToList();
            }
        }

        private List<OverrideAlertNotificationConfig> GetOverrideAlertNotificationConfigs()
        {
            using (var tr = File.OpenText(_context.FunctionAppDirectory + "/Configuration/overrideAlertNotificationConfig.csv"))
            {
                var csv = new CsvReader(tr);
                return csv.GetRecords<OverrideAlertNotificationConfig>().ToList();
            }
        }
    }
}
