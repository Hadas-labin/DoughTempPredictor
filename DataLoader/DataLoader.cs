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
            PredictionGlobals.Xs = new double[numRows, 13];
            PredictionGlobals.Ys = new double[numRows];

            for (int i = 0; i < numRows - 20; i++)
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
                    double bias = double.Parse(parts[1], CultureInfo.InvariantCulture);         // ✅ תמיד 1
                    double flourTemp = double.Parse(parts[2], CultureInfo.InvariantCulture);    // קמח
                    double oilTemp = double.Parse(parts[3], CultureInfo.InvariantCulture);      // שמן
                    double waterTemp = double.Parse(parts[4], CultureInfo.InvariantCulture);    // מים
                    double iceQty = double.Parse(parts[5], CultureInfo.InvariantCulture);       // קרח
                    double finalDoughTemp = double.Parse(parts[6], CultureInfo.InvariantCulture); // ✅ Y

                    PredictionGlobals.Xs[i, 0] = bias;
                    PredictionGlobals.Xs[i, 1] = flourTemp;
                    PredictionGlobals.Xs[i, 2] = oilTemp;
                    PredictionGlobals.Xs[i, 3] = waterTemp;
                    PredictionGlobals.Xs[i, 4] = iceQty;
                    PredictionGlobals.Ys[i] = finalDoughTemp;

                }
                catch (FormatException ex)
                {
                    throw new FormatException($"Invalid number format on line {i + 2}: {ex.Message}", ex);
                }
            }
        }
    }
}
