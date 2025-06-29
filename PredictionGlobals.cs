namespace DoughTempPredictor
{
    public static class PredictionGlobals
    {
        public static int ColCount;
        public static int RowsCount;
        public static double[,] Xs;
        public static double[] Ys;
        public static double[] theta;
        public const double iceTemperature = -78.5;
        public static double predictedY = 26;
        public static string filePath = @"..\data\Multivariate_Linear_Regression.csv";
        public static double[] mean;
        public static double[] std;
        public static double Y_Mean { get; set; }
        public static double Y_Std { get; set; }
    }
}
