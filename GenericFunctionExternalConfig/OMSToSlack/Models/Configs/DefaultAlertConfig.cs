namespace OMSToSlack.Models.Configs
{
    public class DefaultAlertConfig
    {
        public DefaultAlertConfig(string metricName, double warningThreshold, double criticalThreshold, bool lessThanThresholdIsBad
            , int minimumViolationsToAlert, double valueMultiplier)
        {
            MetricName = metricName;
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            LessThanThresholdIsBad = lessThanThresholdIsBad;
            MinimumViolationsToAlert = minimumViolationsToAlert;
            ValueMultiplier = valueMultiplier;
        }

        public string MetricName { get; private set; }
        public double ValueMultiplier { get; private set; }
        public double WarningThreshold { get; private set; }
        public double CriticalThreshold { get; private set; }
        public bool LessThanThresholdIsBad { get; private set; }
        public int MinimumViolationsToAlert { get; private set; }
    }
}
