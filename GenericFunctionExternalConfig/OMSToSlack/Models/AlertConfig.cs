namespace OMSToSlack.Models
{
    class AlertConfig
    {
        public AlertConfig(double warningThreshold, double criticalThreshold, bool lessThanThresholdIsBad
            , string alertMessage, string metricName, string formatString, int minimumViolationsToAlert, double valueMultiplier)
        {
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            DefaultAlertMessage = alertMessage;
            LessThanThresholdIsBad = lessThanThresholdIsBad;
            MinimumViolationsToAlert = minimumViolationsToAlert;
            MetricName = metricName;
            FormatString = formatString;
            ValueMultiplier = valueMultiplier;
        }

        public double WarningThreshold { get; private set; }
        public double CriticalThreshold { get; private set; }
        public int MinimumViolationsToAlert { get; private set; }
        public bool LessThanThresholdIsBad { get; private set; }
        public string DefaultAlertMessage { get; private set; }
        public string MetricName { get; private set; }
        public string FormatString { get; private set; }
        public double ValueMultiplier { get; set; }

    }
}
