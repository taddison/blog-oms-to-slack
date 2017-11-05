namespace OMSToSlack.Models.Configs
{
    public class DefaultAlertNotificationConfig
    {
        public DefaultAlertNotificationConfig(string metricName, string channel, string formatString, string alertMessage)
        {
            MetricName = metricName;
            Channel = channel;
            FormatString = formatString;
            AlertMessage = alertMessage;
        }

        public string MetricName { get; private set; }
        public string Channel { get; private set; }
        public string FormatString { get; private set; }
        public string AlertMessage { get; private set; }
    }
}
