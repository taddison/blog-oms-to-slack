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
            var alertConfig = GetAlertConfig(alert);

            // Is this a < or > alert?
            var comparison = alertConfig.LessThanThresholdIsBad ? LessThan : MoreThan;

            // Aggregate metrics to produce a single summary record
            var processedValues = alert.MetricValues.Select(mv => mv.Value * alertConfig.ValueMultiplier);
            
            var totals = new
            {
                Average = processedValues.Average()
                ,Min = processedValues.Min()
                ,Max = processedValues.Max()
                ,Critical = processedValues.Count(v => comparison(v, alertConfig.CriticalThreshold))
                ,Warning = processedValues.Count(v => comparison(v, alertConfig.WarningThreshold))
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
            foreach(var channel in notificationConfig.Channels)
            {
                await SlackHelper.SendSlackMessage(channel, message);
            }
        }

        private static AlertConfig GetAlertConfig(Alert alert)
        {
            var valueMultiplier = 1d;
            var lessThanThresholdIsBad = true;

            switch(alert.MetricName)
            {
                case "Processor Usage %":
                    valueMultiplier = 0.01;
                    lessThanThresholdIsBad = false;
                    break;
                case "Free Space %":
                    valueMultiplier = 0.01;
                    break;
            }

            var warning = 0.5;
            var critical = 0.75;
            var minimumViolations = 1;

            switch (alert.MetricName)
            {
                case "Free Megabytes":
                    warning = 10000d;
                    critical = 5000d;
                    break;
                case "Free Space %":
                    warning = 0.2;
                    critical = 0.1;
                    break;
                case "Processor Usage %":
                    warning = 0.35;
                    critical = 0.5;
                    break;
            }

            var combined = $"{alert.MachineName}|{alert.MetricName}";

            switch (combined)
            {
                case "Server2|Processor Usage %":
                    warning = 0.8d;
                    critical = 0.9d;
                    break;
            }
            
            return new AlertConfig(
                warning
                , critical
                , lessThanThresholdIsBad
                , minimumViolations
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
            var channels = new List<string>();

            switch (alert.MachineName)
            {
                case "Server1":
                    channels.Add("#Server1Team");
                    break;
            }

            switch (alert.MetricName)
            {
                case "Free Megabytes":
                    channels.Add("#memory-monitors");
                    break;
            }

            if(channels.Count == 0)
            {
                channels.Add("#default");
            }

            return new AlertNotificationConfig(valueMultiplier, "ALARM", channels);
        }
    }
}
