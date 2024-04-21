using System;
using System.Collections.Generic;

namespace RainfallAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Read device list from CSV
                var dataReader = new RainfallDataReader();
                var devices = dataReader.ReadDeviceList("Data/Devices.csv");

                // Read rainfall data from the last 2 sets of CSV files
                var rainfallData = dataReader.ReadRainfallData("Data");

                // Analyze rainfall data
                var dataAnalyzer = new RainfallDataAnalyzer();
                var rainfallDataByDevice = dataAnalyzer.GroupRainfallDataByDevice(rainfallData);
                var analysisResults = dataAnalyzer.AnalyzeRainfallData(rainfallDataByDevice);

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
    }
}
