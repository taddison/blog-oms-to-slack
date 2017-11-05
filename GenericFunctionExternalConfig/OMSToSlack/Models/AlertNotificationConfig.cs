namespace OMSToSlack.Models
{
    class AlertNotificationConfig
    {
        public AlertNotificationConfig(string formatString, string alertMessage, string channel)
        {
            FormatString = formatString;
            AlertMessage = alertMessage;
            Channel = channel;
        }

        public string FormatString { get; private set; }
        public string AlertMessage { get; private set; }
        public string Channel { get; private set; }
    }
}
