using System.Globalization;
using System.IO;
using System.Text;

namespace DoughTempPredictor
{
    public class DataLoader
    {
        public static void LoadCsvData()
        {
            if (!File.Exists(PredictionGlobals.filePath))
                throw new FileNotFoundException("CSV file not found.", PredictionGlobals.filePath);


            var lines = File.ReadAllLines(PredictionGlobals.filePath, Encoding.UTF8);

            if (lines.Length < 2)
                throw new InvalidDataException("CSV file does not contain enough data.");

            // int numRows = Math.Min(500, lines.Length - 1); // Skip header
            int numRows = lines.Length - 1;
            int numFeatures = lines[1].Length - 2;
            PredictionGlobals.Xs = new double[numRows, 5];
            PredictionGlobals.Ys = new double[numRows];

            for (int i = 0; i < numRows; i++)
            {
                string line = lines[i + 1]; // Skip header row
                if (string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine($"Skipping empty or whitespace line at index {i}.");
                    continue; // דלג/י על שורות ריקות
                }

                string[] parts = line.Split(',').Select(v => v.Trim()).ToArray();

                if (parts.Length < 5)
                    throw new InvalidDataException($"Line {i + 2} does not contain enough columns.");

                try
                {
                    // parts[0] = שם המוצר → לא משתמשים בו
                    PredictionGlobals.Xs[i, 0] =1;         // ✅ תמיד 1
                    PredictionGlobals.Xs[i, 1] = double.Parse(parts[0], CultureInfo.InvariantCulture);   
                    PredictionGlobals.Xs[i, 2] = double.Parse(parts[1], CultureInfo.InvariantCulture);  
                    PredictionGlobals.Xs[i, 3] = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    PredictionGlobals.Xs[i, 4] = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    PredictionGlobals.Ys[i] = double.Parse(parts[4], CultureInfo.InvariantCulture); 

                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Invalid number format on line {i + 2}: {ex.Message}", ex);
                }
            }
        }
    }
}
