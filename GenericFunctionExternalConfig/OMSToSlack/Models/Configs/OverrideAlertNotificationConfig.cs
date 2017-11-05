namespace OMSToSlack.Models.Configs
{
    public class OverrideAlertNotificationConfig
    {
        public OverrideAlertNotificationConfig(string metricName = null, string machineName = null, string channel = null)
        {
            MetricName = string.IsNullOrWhiteSpace(metricName) ? null : metricName;
            MachineName = string.IsNullOrWhiteSpace(machineName) ? null : machineName;
            Channel = channel;
        }

        public string MetricName { get; private set; }
        public string MachineName { get; private set; }
        public string Channel { get; private set; }
    }
}
