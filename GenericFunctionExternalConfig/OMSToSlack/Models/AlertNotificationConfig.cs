using System.Collections.Generic;

namespace OMSToSlack.Models
{
    class AlertNotificationConfig
    {
        public AlertNotificationConfig(string formatString, string alertMessage, List<string> channels)
        {
            FormatString = formatString;
            AlertMessage = alertMessage;
            Channels = channels;
        }

        public string FormatString { get; private set; }
        public string AlertMessage { get; private set; }
        public List<string> Channels { get; private set; }
    }
}
