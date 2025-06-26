using System.Globalization;

namespace DoughTempPredictor.RegressionCalculator
{
    public class PredictionEnhancements
    {
        public static double yMean;
        public static double yStd ;
        public static void LoadThetaAndNormalizationFromCsv()
        {
            string directory = Path.GetDirectoryName(PredictionGlobals.filePath);
            string thetaCsvPath = Path.Combine(directory, "ThetaParams.csv");

            if (!File.Exists(thetaCsvPath))
                throw new FileNotFoundException($"File not found: {thetaCsvPath}");

            List<double> thetaList = new List<double>();
            List<double> meanList = new List<double>();
            List<double> stdList = new List<double>();
           

            using (StreamReader reader = new StreamReader(thetaCsvPath))
            {
                string? line;
                string? section = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string upper = line.ToUpper();

                    Console.WriteLine($"Line: '{line}' (section: {section})");

                    if (upper == "THETA" || upper == "MEAN" || upper == "STD" || upper == "Y_MEAN" || upper == "Y_STD")
                    {
                        section = upper;
                        continue;
                    }

                    if (double.TryParse(line, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
                    {
                        switch (section)
                        {
                            case "THETA":
                                thetaList.Add(val);
                                break;
                            case "MEAN":
                                meanList.Add(val);
                                break;
                            case "STD":
                                stdList.Add(val);
                                break;
                            case "Y_MEAN":
                                yMean = val;
                                break;
                            case "Y_STD":
                                yStd = val;
                                break;
                            default:
                                throw new InvalidDataException("Data section not recognized before values.");
                        }
                    }
                    else
                    {
                        throw new FormatException($"Invalid numeric value in CSV: '{line}'");
                    }
                }
            }

            PredictionGlobals.theta = thetaList.ToArray();
            PredictionGlobals.mean = meanList.ToArray();
            PredictionGlobals.std = stdList.ToArray();
        }


        public static double IsolateFeature(double[] inputs, int featureIndexToIsolate)
        {
            double sum = PredictionGlobals.theta[0]; // bias term
            int inputIndex = 1;
            Console.WriteLine("------------");
            for (int i = 1; i < PredictionGlobals.theta.Length; i++)
            {
                if (i == featureIndexToIsolate)
                {
                    inputIndex++; // קופצת רק באינפוטים, לא בלולאה
                    continue;
                }
                sum += PredictionGlobals.theta[i] * inputs[inputIndex - 1];
                Console.WriteLine(i + " - sum:  " + sum);
                inputIndex++;
            }
            double predY = (PredictionGlobals.predictedY - yMean) / yStd;

            double weightToIsolate = PredictionGlobals.theta[featureIndexToIsolate];

            double isolatedValue = (predY - sum) / weightToIsolate;

            double realValue = isolatedValue * PredictionGlobals.std[featureIndexToIsolate - 1] + PredictionGlobals.mean[featureIndexToIsolate - 1];

            return realValue;
        }

        public static (double IceAmount, double? OilAmount) CheckAndSendFeature(
         double[] inputs,
         double totalLiquidAmount)
        {
            try
            {
                LoadThetaAndNormalizationFromCsv();

                //if (PredictionGlobals.theta.Length != inputs.Length)
                //{
                //    throw new InvalidDataException("theta.Length = " + PredictionGlobals.theta.Length + " inputs.Length = " + inputs.Length);
                //}

                // מנרמל את הקלטים לפי הממוצע וסטיית התקן
                double[] normalizedInputs = new double[inputs.Length];
                normalizedInputs[0] = inputs[0]; // bias stays the same (usually 1)
                Console.WriteLine("------------");
                for (int i = 0; i < inputs.Length - 1; i++)
                { 
                    double mean = PredictionGlobals.mean[i];
                    double std = PredictionGlobals.std[i];
                    if (std == 0) std = 1; // הגנה מפני חילוק באפס
                    normalizedInputs[i] = (inputs[i] - mean) / std;
                    Console.WriteLine($"{i}- inputs: {inputs[i]} \tnormalizedInputs: {normalizedInputs[i]}");
                }
                Console.WriteLine("------------");



                int isolatedIce = 4;
                int indexOil = 2;
                double isolatedWater = IsolateFeature(normalizedInputs, isolatedIce);

                if (isolatedWater < 0.5 * totalLiquidAmount)
                {
                    inputs[indexOil] = 0.5 * totalLiquidAmount;
                    PredictAndPrint(normalizedInputs);
                    Console.WriteLine("isolatedIce =   " + isolatedWater);
                    return (isolatedWater, null);
                }
                else
                {
                    inputs[indexOil] = 0.5 * totalLiquidAmount;
                    double isolatedOil = IsolateFeature(normalizedInputs, indexOil);
                    Console.WriteLine("isolatedIce =   " + isolatedIce);
                    Console.WriteLine("isolatedOil =   " + isolatedOil);
                    return (0.5 * totalLiquidAmount, isolatedOil);
                }
            }
            catch (Exception e)
            {
                throw new InvalidDataException(e.Message);
            }
        }


            // Predicts using linear regression and prints the result
            public static void PredictAndPrint( double[] input)
            {
                if (PredictionGlobals.theta.Length != input.Length + 1)
                {
                    Console.WriteLine("Error: Theta length must be one more than input length (to include bias).");
                    return;
                }

            double prediction = PredictionGlobals.theta[0]; // bias term

                for (int i = 0; i < input.Length; i++)
                {
                    prediction += PredictionGlobals.theta[i + 1] * input[i];
                }

                Console.WriteLine($"Prediction: {prediction:F3}");
            }
        }

}