using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace RainfallAnalyzer
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Read device list from CSV
                List<Device> devices = ReadDeviceList("Data/Devices.csv");

                // Read rainfall data from the last 2 sets of CSV files
                List<RainfallData> rainfallData = ReadRainfallData("Data/Data1.csv", "Data/Data2.csv");

                // Analyze rainfall data

                Dictionary<string, List<RainfallData>> rainfallDataByDevice = GroupRainfallDataByDevice(rainfallData);
                Dictionary<string, AnalysisResult> analysisResults = AnalyzeRainfallData(rainfallDataByDevice);

                // Display the results
                Console.WriteLine("Average Rainfall and Status for Last 4 Hours:");

                foreach (var kvp in analysisResults)
                {
                    Console.WriteLine($"Device ID: {kvp.Key}");

                    AnalysisResult result = kvp.Value;
                    Console.WriteLine($"  Average Rainfall: {result.AverageRainfall:F2}mm");
                    Console.WriteLine($"  Status: {result.Status}");
                    Console.WriteLine($"  Trend: {result.Trend}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static List<Device> ReadDeviceList(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Device list file not found.");

            List<Device> devices;

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                devices = csv.GetRecords<Device>().ToList();
            }

            return devices;
        }

        static List<RainfallData> ReadRainfallData(params string[] filepaths)
        {
            List<RainfallData> rainfallData = new List<RainfallData>();

            foreach (var filepath in filepaths)
            {
                if (!File.Exists(filepath))
                    throw new FileNotFoundException($"Rainfall data file not found: {filepath}");

                using (var reader = new StreamReader(filepath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    rainfallData.AddRange(csv.GetRecords<RainfallData>());
                }
            }

            return rainfallData;
        }

        static Dictionary<string, List<RainfallData>> GroupRainfallDataByDevice(List<RainfallData> rainfallData)
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

        static Dictionary<string, AnalysisResult> AnalyzeRainfallData(Dictionary<string, List<RainfallData>> dataByDevice)
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

    public enum Status
    {
        Green,
        Amber,
        Red
    }
}
