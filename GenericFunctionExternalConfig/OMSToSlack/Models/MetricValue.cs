using System;

namespace OMSToSlack.Models
{
    public struct MetricValue
    {
        public DateTime Date;
        public Double Value;

        public MetricValue(DateTime date, Double value)
        {
            Date = date;
            Value = value;
        }
    }
}
