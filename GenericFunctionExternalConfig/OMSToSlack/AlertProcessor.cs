using OMSToSlack.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OMSToSlack
{
    public static class AlertProcessor
    {
        private static Func<double, double, bool> LessThan = (double value, double threshold) => { return value < threshold; };
        private static Func<double, double, bool> MoreThan = (double value, double threshold) => { return value > threshold; };

        public static async void ProcessAlert(Alert alert)
        {
            var alertConfig = GetAlertConfig(alert.MetricName);

            // Is this a < or > alert?
            var comparison = alertConfig.LessThanThresholdIsBad ? LessThan : MoreThan;

            // Machine-specific overrides?
            // TODO: This needs to be baked into GetAlertConfig
            var (warning, critical) = GetMachineDefaultThresholdOverrides(alert.MachineName, alert.MetricName);
            var criticalThreshold = critical ?? alertConfig.CriticalThreshold;
            var warningThreshold = warning ?? alertConfig.WarningThreshold;

            // Aggregate metrics to produce a single summary record
            var processedValues = alert.MetricValues.Select(mv => mv.Value * alertConfig.ValueMultiplier);
            
            var totals = new
            {
                Average = processedValues.Average()
                ,Min = processedValues.Min()
                ,Max = processedValues.Max()
                ,Critical = processedValues.Count(v => comparison(v, criticalThreshold))
                ,Warning = processedValues.Count(v => comparison(v, warningThreshold))
            };

            // Determine alert criticality
            var isWarning = totals.Warning >= alertConfig.MinimumViolationsToAlert;
            var isCritical = totals.Critical >= alertConfig.MinimumViolationsToAlert;
            
            // If the alert doesn't cross the warning threshold return
            if(!isWarning)
            {
                return;
            }

            // Where should the alert go
            var notificationConfig = GetAlertNotificationConfig(alert);

            // TODO: This should be baked into GetAlertNotificationConfig
            IEnumerable<string> channels = new List<string>() { notificationConfig.Channel };
            channels = channels.Union(GetMachineChannels(alert.MachineName));
            channels = channels.Union(GetMetricChannels(alert.MetricName));

            // Build message
            // Infra - CPU [CRIT] :: Server1 :: 56%/59%/61% (min/avg/max Processor Usage %)
            // Infra - Disk [WARN] :: Server1 - E: :: 5%/6%/6% (min/avg/max Free Space %)
            var instance = string.IsNullOrWhiteSpace(alert.InstanceName) ? "" : $" - {alert.InstanceName}";
            var message = $"{notificationConfig.AlertMessage} [{(isCritical ? "CRIT" : "WARN")}] :: {alert.MachineName}{instance} :: ";
            message += $"{totals.Min.ToString(notificationConfig.FormatString)}/{totals.Average.ToString(notificationConfig.FormatString)}/{totals.Max.ToString(notificationConfig.FormatString)} ";
            message += $"(min/avg/max {alert.MetricName})";
            
            if(isCritical)
            {
                message += " @channel";
            }
                
            // Send message
            foreach(var channel in channels)
            {
                await SlackHelper.SendSlackMessage(channel, message);
            }
        }

        private static AlertConfig GetAlertConfig(string metricName)
        {
            var valueMultiplier = 1d;
            var lessThanThresholdIsBad = true;

            switch(metricName)
            {
                case "Processor Usage %":
                    valueMultiplier = 0.01;
                    lessThanThresholdIsBad = false;
                    break;
                case "Free Space %":
                    valueMultiplier = 0.01;
                    break;
                default:
                    break;
            }

            return new AlertConfig(
                0.5
                , 0.75
                , lessThanThresholdIsBad
                , metricName
                , 1
                , valueMultiplier
                );
        }

        private static AlertNotificationConfig GetAlertNotificationConfig(Alert alert)
        {
            var valueMultiplier = "N0";
            if(alert.MetricName.Contains("%"))
            {
                valueMultiplier = "P0";
            }

            return new AlertNotificationConfig(valueMultiplier, "ALARM", "#database");
        }

        private static (double? warningThreshold, double? criticalThreshold) GetMachineDefaultThresholdOverrides(string machineName, string metricName)
        {
            var combined = $"{machineName}|{metricName}";

            switch(combined)
            {
                case "Server2|Processor Usage %":
                    return (0.8, 0.9);
                default:
                    return (null, null);
            }
        }

        private static IEnumerable<string> GetMachineChannels(string machineName)
        {
            switch (machineName)
            {
                case "Server1":
                    return new List<string>() { "#Server1Team" };
                default:
                    return new List<string>();
            }
        }

        private static IEnumerable<string> GetMetricChannels(string metricName)
        {
            switch (metricName)
            {
                case "Free Megabytes":
                    return new List<string>() { "#memory-monitors" };
                default:
                    return new List<string>();
            }
        }
    }
}
