using System;
using System.Collections.Generic;

namespace RainfallAnalyzer
{
    public class RainfallDataAnalyzer
    {
        public Dictionary<string, List<RainfallData>> GroupRainfallDataByDevice(List<RainfallData> rainfallData)
        {
            Dictionary<string, List<RainfallData>> dataByDevice = new Dictionary<string, List<RainfallData>>();

            foreach (var data in rainfallData)
            {
                if (data.DeviceId != null)
                {
                    if (!dataByDevice.ContainsKey(data.DeviceId))
                    {
                        dataByDevice[data.DeviceId] = new List<RainfallData>();
                    }
                    dataByDevice[data.DeviceId].Add(data);
                }
            }

            return dataByDevice;
        }

        public Dictionary<string, AnalysisResult> AnalyzeRainfallData(Dictionary<string, List<RainfallData>> dataByDevice)
        {
            Dictionary<string, AnalysisResult> analysisResults = new Dictionary<string, AnalysisResult>();

            foreach (var kvp in dataByDevice)
            {
                List<RainfallData> dataForDevice = kvp.Value;

                double totalRainfall = 0;
                bool increasingTrend = true;

                for (int i = dataForDevice.Count - 1; i >= 0; i--)
                {
                    totalRainfall += dataForDevice[i].Rainfall ?? 0;

                    // Check the trend
                    if (i > 0 && dataForDevice[i].Rainfall > dataForDevice[i - 1].Rainfall)
                    {
                        increasingTrend = false;
                    }
                }

                double averageRainfall = totalRainfall / Math.Min(dataForDevice.Count, 4);

                // Determine status
                Status status;
                if (averageRainfall < 10)
                {
                    status = Status.Green;
                }
                else if (averageRainfall < 15)
                {
                    status = Status.Amber;
                }
                else
                {
                    status = Status.Red;
                }

                // Check if any reading in the last 4 hours > 30mm
                foreach (var data in dataForDevice)
                {
                    if (data.Rainfall > 30)
                    {
                        status = Status.Red;
                        break;
                    }
                }

                // Add the analysis result
                analysisResults[kvp.Key] = new AnalysisResult
                {
                    AverageRainfall = averageRainfall,
                    Status = status,
                    Trend = increasingTrend ? "Increasing" : "Decreasing"
                };
            }

            return analysisResults;
        }
    }
}
