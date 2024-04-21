using CsvHelper.Configuration.Attributes;
using System;

namespace RainfallAnalyzer
{
    public class RainfallData
    {
        [Name("Device ID")]
        public string? DeviceId { get; set; }

        [Name("Time")]
        public DateTime? Time { get; set; }

        [Name("Rainfall")]
        public double? Rainfall { get; set; }
    }
}
