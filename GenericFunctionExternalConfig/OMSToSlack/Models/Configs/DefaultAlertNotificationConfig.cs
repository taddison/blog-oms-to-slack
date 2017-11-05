using System.Collections.Generic;

namespace OMSToSlack.Models.Configs
{
    public class DefaultAlertNotificationConfig
    {
        public DefaultAlertNotificationConfig(string metricName, List<string> channels, string formatString, string alertMessage)
        {
            MetricName = metricName;
            Channels = channels;
            FormatString = formatString;
            AlertMessage = alertMessage;
        }

        public string MetricName { get; set; }
        public List<string> Channels { get; set; }
        public string FormatString { get; set; }
        public string AlertMessage { get; set; }
    }
}
