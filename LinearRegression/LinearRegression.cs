using System.Globalization;
using System.Runtime.Serialization.Formatters;
using DoughTempPredictor;

namespace DoughTempPredictor
{
    internal class LinearRegression
    {
        

        public static void PerformGradientDescent()
        {
            try
            {
                DataLoader.LoadCsvData();

                //double[,] X = PredictionGlobals.Xs;
                //double[] y = PredictionGlobals.Ys;

                int m = PredictionGlobals.Ys.Length;

                // X = PrepareFeatures(X);
                //  Globals.Xs = X;

                //int n = PredictionGlobals.Xs.GetLength(1);

                InitializeTheta();

                //PredictionGlobals.theta = theta;

                NormalizeY();


                NormalizeFeatures();
                RunGradientDescent();

                //PredictionGlobals.theta = theta;

                SaveThetaToFile();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during training process: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// סידור מחדש של עמודות מטריצת הקלט – מעביר את עמודת האחדות לראש.
        /// </summary>
        private static double[,] PrepareFeatures(double[,] X)
        {
            try
            {
                int m = X.GetLength(0);
                int n = X.GetLength(1);
                double[,] Xnew = new double[m, n];

                for (int i = 0; i < m; i++)
                {
                    Xnew[i, 0] = X[i, 3]; // עמודת אחדות
                    Xnew[i, 1] = X[i, 0];
                    Xnew[i, 2] = X[i, 1];
                    Xnew[i, 3] = X[i, 2];
                }

                return Xnew;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// מאתחל את וקטור θ אם אינו קיים או שאינו באורך מתאים.
        /// </summary>
        private static void InitializeTheta()
        {
            try
            {
                int n = PredictionGlobals.Xs.GetLength(1);

                if (PredictionGlobals.theta != null && PredictionGlobals.theta.Length == n)
                    throw new InvalidOperationException("Theta already initialized.");
                
                double[] theta = new double[n];

                Random rand = new Random();
                for (int i = 0; i < n; i++)
                {
                    theta[i] = (rand.NextDouble() - 0.5) * 0.02;
                }

                Console.WriteLine("ערכים התחלתיים של θ:");
                for (int i = 0; i < theta.Length; i++)
                {
                    Console.WriteLine($"θ[{i}] = {theta[i]:F6}");
                }
                PredictionGlobals.theta = theta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void NormalizeY()
        {
            int m = PredictionGlobals.Ys.Length;
            double sum = 0;
            for (int i = 0; i < m; i++) sum += PredictionGlobals.Ys[i];
            double mean = sum / m;

            double std = 0;
            for (int i = 0; i < m; i++) std += Math.Pow(PredictionGlobals.Ys[i] - mean, 2);
            std = Math.Sqrt(std / m);

            if (std == 0) std = 1;

            for (int i = 0; i < m; i++)
            {
                PredictionGlobals.Ys[i] = (PredictionGlobals.Ys[i] - mean) / std;
            }

            PredictionGlobals.Y_Mean = mean;
            PredictionGlobals.Y_Std = std;
        }

        /// <summary>
        /// מנרמל את כל עמודות המאפיינים פרט לעמודת האחדות.
        /// </summary>
        private static void NormalizeFeatures()
        {
            try
            {

                int m = PredictionGlobals.Xs.GetLength(0);
                int n = PredictionGlobals.Xs.GetLength(1);
                double[] mean1 = new double[n - 1];
                double[] std1 = new double[n - 1];

                for (int j = 1; j < n; j++) 
                {
                    double mean = 0, std = 0;
                    for (int i = 0; i < m; i++) mean += PredictionGlobals.Xs[i, j];
                    mean /= m;
                    mean1[j - 1] = mean;
                    Console.WriteLine($"lolaa {j} = {mean}");

                    for (int i = 0; i < m; i++) std += Math.Pow(PredictionGlobals.Xs[i, j] - mean, 2);
                    std = Math.Sqrt(std / m);

                    if (std == 0)
                    {
                        std = 1;
                    }
                    std1[j - 1] = std;

                    for (int i = 0; i < m; i++)
                    {
                        PredictionGlobals.Xs[i, j] = (PredictionGlobals.Xs[i, j] - mean) / std;
                    }
                }
                PredictionGlobals.mean = mean1;
                PredictionGlobals.std = std1;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// מריץ את אלגוריתם Gradient Descent ומחשב את θ.
        /// </summary>
        private static void RunGradientDescent(/*double[,] X, double[] y, double[] theta*/)
        {
            try
            {
                int m = PredictionGlobals.Xs.GetLength(0);
                int n = PredictionGlobals.Xs.GetLength(1);
                int maxIterations = 1000;
                double learningRate = 0.01;
                double tolerance = 1e-6;
                double previousCost = double.MaxValue;

                List<double> costHistory = new List<double>();

                for (int iter = 0; iter < maxIterations; iter++)
                {
                    double[] predictions = MultiplyMatrixVector(PredictionGlobals.Xs, PredictionGlobals.theta);
                    //חישוב שגיאה
                    double[] errors = new double[m];
                    for (int i = 0; i < m; i++)
                    {
                        errors[i] = predictions[i] - PredictionGlobals.Ys[i];
                    }

                    for (int j = 0; j < n; j++)
                    {
                        double gradient = 0;
                        //נוסחת ירידת רגרסיה
                        for (int i = 0; i < m; i++)
                        {
                            gradient += errors[i] * PredictionGlobals.Xs[i, j];
                        }
                        gradient /= m;
                       // Console.Write($"Gradient for θ[{j}] = {gradient:F6}" + "\t");


                        if (double.IsNaN(gradient))
                        {
                            throw new Exception($"Gradient at index {j} became NaN. Possible data issue.");
                        }
                        //עידכון ה theta 
                        PredictionGlobals.theta[j] -= learningRate * gradient;
                    }

                    double[] updatedPredictions = MultiplyMatrixVector(PredictionGlobals.Xs, PredictionGlobals.theta);
                    double cost = computeCostFunction(updatedPredictions, PredictionGlobals.Ys);
                    if (double.IsNaN(cost))
                    {
                        throw new Exception("Cost function returned NaN. Aborting gradient descent.");
                    }

                    costHistory.Add(cost);

                    if (iter == 0 || iter % 100 == 0 || iter == maxIterations - 1 || iter == 1)
                    {
                        Console.WriteLine($"\nIteration {iter}");
                        Console.WriteLine($"Cost: {cost:F6}");
                        for (int i = 0; i < PredictionGlobals.theta.Length; i++)
                        {
                            Console.WriteLine($"θ[{i}] = {PredictionGlobals.theta[i]:F6}");
                        }
                    }

                    //if (Math.Abs(previousCost - cost) < tolerance)
                    //{
                    //    throw new Exception($"Stopped at iteration {iter} due to convergence.");
                    //}

                    previousCost = cost;
                }

                Console.WriteLine("Final cost: " + costHistory.Last());
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred during gradient descent: " + e.Message);
            }
        }

        public static double[] MultiplyMatrixVector(double[,] matrix, double[] vector)
        {
            try
            {
                int rows = matrix.GetLength(0);
                int cols = matrix.GetLength(1);
                if (vector.Length != cols)
                    throw new ArgumentException("Vector size must match number of matrix columns.");
                double[] result = new double[rows];
                for (int i = 0; i < rows; i++)
                {
                    double sum = 0;
                    for (int j = 0; j < cols; j++)
                    {
                        sum += matrix[i, j] * vector[j];
                    }
                    result[i] = sum;
                }
                return result;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static double computeCostFunction(double[] Y_pred, double[] Y_text)
        {
            try
            {
                double sumSquaredErrors = 0;
                int count = Y_pred.Length;

                for (int i = 0; i < count; i++)
                {
                    double error = Y_pred[i] - Y_text[i];
                    sumSquaredErrors += error * error;
                }

                return sumSquaredErrors / (2 * count);
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void SaveThetaToFile()
        {
            try
            {

                // מוציאים את התיקיה מתוך Globals.filePath (שבה נמצא הקובץ המקורי)
                string directory = Path.GetDirectoryName(PredictionGlobals.filePath);

                // בונים נתיב מלא חדש לתיקיה הזו עם שם הקובץ ThetaParams.csv
                string thetaCsvPath = Path.Combine(directory, "ThetaParams.csv");

                using (StreamWriter writer = new StreamWriter(thetaCsvPath, false)) // false = דריסת קובץ אם קיים
                {
                    writer.WriteLine("Theta"); // כותרת עמודה
                    foreach (var val in PredictionGlobals.theta)
                    {
                        writer.WriteLine(val.ToString(CultureInfo.InvariantCulture)); // שומר מספר בפורמט נקודה עשרונית
                    }

                    writer.WriteLine(); 

                    writer.WriteLine("Mean");
                    foreach (var val in PredictionGlobals.mean)
                    {
                        writer.WriteLine(val.ToString(CultureInfo.InvariantCulture));
                    }

                    writer.WriteLine(); 

                    // שמירת סטיות תקן
                    writer.WriteLine("Std");
                    foreach (var val in PredictionGlobals.std)
                    {
                        writer.WriteLine(val.ToString(CultureInfo.InvariantCulture));
                    }

                    writer.WriteLine();
                    writer.WriteLine("Y_Mean");
                    writer.WriteLine(PredictionGlobals.Y_Mean.ToString(CultureInfo.InvariantCulture));

                    writer.WriteLine();
                    writer.WriteLine("Y_Std");
                    writer.WriteLine(PredictionGlobals.Y_Std.ToString(CultureInfo.InvariantCulture));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("שגיאה בשמירה: " + ex.Message);
            }
        }
    }
}

