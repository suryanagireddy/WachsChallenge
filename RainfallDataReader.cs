using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace RainfallAnalyzer
{
    public class RainfallDataReader
    {
        public List<Device> ReadDeviceList(string filePath)
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

        public List<RainfallData> ReadRainfallData(params string[] filepaths)
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
    }
}
