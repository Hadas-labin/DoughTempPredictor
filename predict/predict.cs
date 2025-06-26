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
            string filePath = Path.Combine(directory, "aabbcc.csv");

            using (var reader = new StreamReader(filePath))
            {
                string? headerLine = reader.ReadLine(); // לדלג על שורת כותרת

                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();
                    if (line == null) continue;

                    var values = line.Split(',');

                    double ZN = double.Parse(values[0], CultureInfo.InvariantCulture);
                    double INDUS = double.Parse(values[1], CultureInfo.InvariantCulture);
                    double CHAS = double.Parse(values[2], CultureInfo.InvariantCulture);
                    double NOX = double.Parse(values[3], CultureInfo.InvariantCulture);
                    double RM = double.Parse(values[4], CultureInfo.InvariantCulture);
                    double AGE = double.Parse(values[5], CultureInfo.InvariantCulture);
                    double DIS = double.Parse(values[6], CultureInfo.InvariantCulture);
                    double RAD = double.Parse(values[7], CultureInfo.InvariantCulture);
                    double TAX = double.Parse(values[8], CultureInfo.InvariantCulture);
                    double PTR = double.Parse(values[9], CultureInfo.InvariantCulture);
                    double B = double.Parse(values[10], CultureInfo.InvariantCulture);
                    double LST = double.Parse(values[11], CultureInfo.InvariantCulture);


                    double trueY = double.Parse(values[12], CultureInfo.InvariantCulture); // הערך האמיתי

                    double[] inputs = new double[] { ZN, INDUS, CHAS, NOX ,RM, AGE, DIS, RAD, TAX, PTR, B, LST};
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