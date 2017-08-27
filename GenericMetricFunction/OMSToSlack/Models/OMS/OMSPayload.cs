namespace OMSToSlack.Models.OMS
{
    public class OMSPayload
    {
        public double WarningThreshold { get; set; }
        public double CriticalThreshold { get; set; }
        public string Channel { get; set; }
        public bool LessThanThresholdIsBad { get; set; }
        public string AlertMessage { get; set; }
        public string MetricName { get; set; }
        public string FormatString { get; set; }
        public int ObservationThreshold { get; set; }
        public SearchResults SearchResults { get; set; }
    }
}