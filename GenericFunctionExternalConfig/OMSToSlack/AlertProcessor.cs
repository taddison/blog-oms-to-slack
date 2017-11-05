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

            // Process metrics
            foreach(var mv in alert.MetricValues)
            {
                mv.Value = mv.Value * alertConfig.ValueMultiplier;
            }

            // Is this a < or > alert?
            var comparison = alertConfig.LessThanThresholdIsBad ? LessThan : MoreThan;

            // Machine-specific overrides?
            var (warning, critical) = GetMachineDefaultThresholdOverrides(alert.MachineName, alert.MetricName);
            var criticalThreshold = critical ?? alertConfig.CriticalThreshold;
            var warningThreshold = warning ?? alertConfig.WarningThreshold;

            // Where should the alert go
            var defaultChannel = "#alerts";
            IEnumerable<string> channels = new List<string>() { defaultChannel };
            channels = channels.Union(GetMachineChannels(alert.MachineName));
            channels = channels.Union(GetMetricChannels(alert.MetricName));

            // Aggregate metrics to produce a single summary record
            var totals = alert.MetricValues.GroupBy(_ => 1).Select(g => new
            {
                Average = g.Average(m => m.Value)
                ,Min = g.Min(m => m.Value)
                ,Max = g.Max(m => m.Value)
                ,Critical = g.Count(m => comparison(m.Value, criticalThreshold))
                ,Warning = g.Count(m => comparison(m.Value, warningThreshold))
            }).Single();

            // Determine alert criticality
            var isWarning = totals.Warning >= alertConfig.MinimumViolationsToAlert;
            var isCritical = totals.Critical >= alertConfig.MinimumViolationsToAlert;
            
            // If the alert doesn't cross the warning threshold return
            if(!isWarning)
            {
                return;
            }

            // Build message
            // Infra - CPU [CRIT] :: Server1 :: 56%/59%/61% (min/avg/max Processor Usage %)
            // Infra - Disk [WARN] :: Server1 - E: :: 5%/6%/6% (min/avg/max Free Space %)
            var instance = string.IsNullOrWhiteSpace(alert.InstanceName) ? "" : $" - {alert.InstanceName}";
            var message = $"{alertConfig.DefaultAlertMessage} [{(isCritical ? "CRIT" : "WARN")}] :: {alert.MachineName}{instance} :: ";
            message += $"{totals.Min.ToString(alertConfig.FormatString)}/{totals.Average.ToString(alertConfig.FormatString)}/{totals.Max.ToString(alertConfig.FormatString)} ";
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
            var formatString = "N0";
            var lessThanThresholdIsBad = true;

            switch(metricName)
            {
                case "Processor Usage %":
                    valueMultiplier = 0.01;
                    formatString = "P0";
                    lessThanThresholdIsBad = false;
                    break;
                case "Free Space %":
                    valueMultiplier = 0.01;
                    formatString = "P0";
                    break;
                default:
                    break;
            }

            return new AlertConfig(
                0.5
                , 0.75
                , lessThanThresholdIsBad
                , "ALARM"
                , metricName
                , formatString
                , 1
                , valueMultiplier
                );
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
