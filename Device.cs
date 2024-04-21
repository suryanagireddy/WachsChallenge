using CsvHelper.Configuration.Attributes;

namespace RainfallAnalyzer
{
    public class Device
    {
        [Name("Device ID")]
        public string? DeviceId { get; set; }

        [Name("Device Name")]
        public string? Name { get; set; }

        public string? Location { get; set; }
    }
}
