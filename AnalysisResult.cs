using CsvHelper.Configuration.Attributes;

namespace RainfallAnalyzer
{
    public class AnalysisResult
    {
        public double AverageRainfall { get; set; }
        public Status Status { get; set; }
        public string? Trend { get; set; }
    }
}
