using System.Globalization;

namespace DoughTempPredictor
{
    internal class predict
    {
        public static double Predict(double[] XTest)
        {
            try
            {
                // נורמליזציה של הקלט
                double[] normalizedInputs = new double[XTest.Length];
                for (int i = 0; i < XTest.Length; i++)
                {
                    double mean = PredictionGlobals.mean[i];
                    double std = PredictionGlobals.std[i];
                    if (std == 0) std = 1;
                    normalizedInputs[i] = (XTest[i] - mean) / std;
                }

                // יצירת מטריצה עם bias
                double[] X_withIntercept = new double[ normalizedInputs.Length + 1];
                X_withIntercept[0] = 1.0; // bias term

                for (int i = 0; i < normalizedInputs.Length; i++)
                {
                    X_withIntercept[i + 1] = normalizedInputs[i];
                }

                // חישוב התחזית המנורמלת
                double normalizedPrediction = DotProduct(X_withIntercept, PredictionGlobals.theta);

                // החזרת התוצאה לערך האמיתי
                double realPrediction = normalizedPrediction * PredictionGlobals.Y_Std + PredictionGlobals.Y_Mean;

                return realPrediction;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public static void PredictAndPrintFromCsv()
        {
            string directory = Path.GetDirectoryName(PredictionGlobals.filePath);
            string filePath = Path.Combine(directory, "predict.csv");

            using (var reader = new StreamReader(filePath))
            {
                string? headerLine = reader.ReadLine(); // לדלג על שורת כותרת

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) continue;

                    var values = line.Split(',');

                    double X1 = double.Parse(values[0], CultureInfo.InvariantCulture);
                    double X2 = double.Parse(values[1], CultureInfo.InvariantCulture);
                    double X3 = double.Parse(values[2], CultureInfo.InvariantCulture);
                    double X4 = double.Parse(values[3], CultureInfo.InvariantCulture);
                    double trueY = double.Parse(values[4], CultureInfo.InvariantCulture); // הערך האמיתי

                    double[] inputs = new double[] {X1, X2, X3, X4};
                    double predictedY = Predict(inputs);

                    if (trueY == 0)
                    {
                        Console.WriteLine($" True Y={trueY:F3} | Predicted Y={predictedY:F3} | Error=undefined (True Y is 0)");
                    }
                    else
                    {
                        double errorPercent = Math.Abs((trueY - predictedY) / trueY) * 100;
                        Console.WriteLine($" True Y={trueY:F3} | Predicted Y={predictedY:F3} | Error={errorPercent:F2}%");
                    }
                }
            }
        }

        public static double DotProduct(double[] a, double[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("Vectors must be of the same length");

            double sum = 0;
            for (int i = 0; i < a.Length; i++)
            {
                sum += a[i] * b[i];
            }

            return sum;
        }


        //public static void PredictAndPrintFromCsv()
        //{
        //    string directory = Path.GetDirectoryName(PredictionGlobals.filePath);

        //    string filePath = Path.Combine(directory, "aabbcc.csv");
        //    using (var reader = new StreamReader(filePath))
        //    {
        //        string? headerLine = reader.ReadLine(); // לדלג על שורת כותרת, אם קיימת

        //        while (!reader.EndOfStream)
        //        {
        //            string? line = reader.ReadLine();
        //            if (line == null) continue;

        //            var values = line.Split(',');

        //            double flour = double.Parse(values[0], CultureInfo.InvariantCulture);
        //            double oil = double.Parse(values[1], CultureInfo.InvariantCulture);
        //            double water = double.Parse(values[2], CultureInfo.InvariantCulture);
        //            double ice = double.Parse(values[3], CultureInfo.InvariantCulture);
        //            double trueY = double.Parse(values[4], CultureInfo.InvariantCulture); // הערך האמיתי

        //            double[] inputs = new double[] { flour, oil, water, ice };

        //            double predictedY = Predict(inputs);

        //            double errorPercent = Math.Abs((trueY - predictedY) / trueY) * 100;

        //            Console.WriteLine($" True Y={trueY:F3} | Predicted Y={predictedY:F3} | Error={errorPercent:F2}%");
        //        }
        //    }
        //}
        //public static void PredictAndPrintFromCsv()
        //{
        //    string directory = Path.GetDirectoryName(PredictionGlobals.filePath);
        //    string filePath = Path.Combine(directory, "aabbcc.csv");

        //    using (var reader = new StreamReader(filePath))
        //    {
        //        string? headerLine = reader.ReadLine(); // לדלג על שורת כותרת

        //        while (!reader.EndOfStream)
        //        {
        //            string? line = reader.ReadLine();
        //            if (line == null) continue;

        //            var values = line.Split(',');

        //            double flour = double.Parse(values[0], CultureInfo.InvariantCulture);
        //            double oil = double.Parse(values[1], CultureInfo.InvariantCulture);
        //            double water = double.Parse(values[2], CultureInfo.InvariantCulture);
        //            double ice = double.Parse(values[3], CultureInfo.InvariantCulture);
        //            double trueY = double.Parse(values[4], CultureInfo.InvariantCulture); // הערך האמיתי

        //            double[] inputs = new double[] { flour, oil, water, ice };
        //            double predictedY = Predict(inputs);

        //            if (trueY == 0)
        //            {
        //                Console.WriteLine($" True Y={trueY:F3} | Predicted Y={predictedY:F3} | Error=undefined (True Y is 0)");
        //            }
        //            else
        //            {
        //                double errorPercent = Math.Abs((trueY - predictedY) / trueY) * 100;
        //                Console.WriteLine($" True Y={trueY:F3} | Predicted Y={predictedY:F3} | Error={errorPercent:F2}%");
        //            }
        //        }
        //    }
        //}


    }
}

//public static (double YPred, double ErrorPercentage) PredictWithError(double[] XTest, double YTrue)
//{
//    double YPred = Predict(XTest);
//    double error = Math.Abs((YTrue - YPred) / YTrue) * 100;
//    return (YPred, error);
//}