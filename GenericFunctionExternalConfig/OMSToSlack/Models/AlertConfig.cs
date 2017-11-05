namespace OMSToSlack.Models
{
    class AlertConfig
    {
        public AlertConfig(double warningThreshold, double criticalThreshold, bool lessThanThresholdIsBad
            , string metricName, int minimumViolationsToAlert, double valueMultiplier)
        {
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            LessThanThresholdIsBad = lessThanThresholdIsBad;
            MinimumViolationsToAlert = minimumViolationsToAlert;
            MetricName = metricName;
            ValueMultiplier = valueMultiplier;
        }

        public string MetricName { get; private set; }
        public double ValueMultiplier { get; set; }
        public double WarningThreshold { get; private set; }
        public double CriticalThreshold { get; private set; }
        public bool LessThanThresholdIsBad { get; private set; }
        public int MinimumViolationsToAlert { get; private set; }
    }
}
