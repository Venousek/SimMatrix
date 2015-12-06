using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimMatrix
{
    class Program
    {

        static void Main(string[] args)
        {
            int?[][] matrix = readCSV(@"C:\pv254_tmp\train_matrix.csv", ';');
            //int?[][] matrix_transposed = readCSV(@"C:\pv254_tmp\train_matrix_transposed.csv", ';');
            int?[][] test = readCSV(@"C:\pv254_tmp\test_cart.csv", ';');

            double[] userAvgRatings = new double[matrix.Length];

            for (int i = 0; i < matrix.Length; i++)
            {
                userAvgRatings[i] = matrix[i].Average() == null ? 0 : (double)matrix[i].Average();
            }

            Console.WriteLine("Training and test loaded. Average user ratings computed. Computing average item ratings.");
            double[] itemAvgRatings = createItemBasedBaseline(matrix);


            double?[][] correlMatrix = null;
            double?[][] cosineMatrix = null;

            do
            {                
                Console.WriteLine("To generate correlation matrix, press 1.");
                Console.WriteLine("To generate cosine matrix, press 2.");
                Console.WriteLine("To test userbased correlation matrix, press 3.");
                Console.WriteLine("To test userbased cosine matrix, press 4.");
                Console.WriteLine("To write correlation matrix to file, press 5.");
                Console.WriteLine("To write cosine matrix to file, press 6.");
                Console.WriteLine("To reload correlation matrix from file, press 7.");
                Console.WriteLine("To reload cosine matrix from file, press 8.");
                Console.WriteLine("To compute userbased baseline RMSE, press 9.");
                Console.WriteLine("To compute itembased baseline RMSE, press 10.");
                Console.WriteLine("To test itembased correlation matrix, press 11.");
                Console.WriteLine("To test itembased cosine matrix, press 12.");
                Console.WriteLine("To write correlation itembased matrix to file, press 13.");
                Console.WriteLine("To write cosine itembased matrix to file, press 14.");
                Console.WriteLine("To reload correlation itembased matrix from file, press 15.");
                Console.WriteLine("To reload cosine itembased matrix from file, press 16.");
                Console.WriteLine("To exit, press q.");

                string input = Console.ReadLine();

                switch (input) {
                    case "1" :
                        {                            
                            correlMatrix = createCorrelationSimMatrix(matrix);
                            break;
                        }
                    case "2":
                        {
                            cosineMatrix = createCosineSimMatrix(matrix);
                            break;
                        }
                    case "3":
                        {
                            testMatrix(correlMatrix, test, matrix, userAvgRatings, "Correlation matrix results");
                            break;
                        }
                    case "4":
                        {
                            testMatrix(cosineMatrix, test, matrix, userAvgRatings, "Cosine matrix results");
                            break;
                        }
                    case "5":
                        {
                            writeCSV(@"C:\pv254_tmp\simmatrix_userbased_correl.csv", ';', correlMatrix);
                            break;
                        }
                    case "6":
                        {
                            writeCSV(@"C:\pv254_tmp\simmatrix_userbased_cosine.csv", ';', cosineMatrix);
                            break;
                        }
                    case "7":
                        {
                            correlMatrix = readCSVDouble(@"C:\pv254_tmp\simmatrix_userbased_correl.csv", ';');
                            break;
                        }
                    case "8":
                        {
                            cosineMatrix = readCSVDouble(@"C:\pv254_tmp\simmatrix_userbased_cosine.csv", ';');
                            break;
                        }                          
                    case "9":
                        {
                            baselineUserBased(userAvgRatings, test);
                            break;
                        }
                    case "10":
                        {
                            baselineItemBased(itemAvgRatings, test);
                            break;
                        }
                    case "11":
                        {
                            testItembasedMatrix(correlMatrix, test, matrix, "Correlation matrix results");
                            break;
                        }
                    case "12":
                        {
                            testItembasedMatrix(cosineMatrix, test, matrix, "Cosine matrix results");
                            break;
                        }
                    case "13":
                        {
                            writeCSV(@"C:\pv254_tmp\simmatrix_itembased_correl.csv", ';', correlMatrix);
                            break;
                        }
                    case "14":
                        {
                            writeCSV(@"C:\pv254_tmp\simmatrix_itembased_cosine.csv", ';', cosineMatrix);
                            break;
                        }
                    case "15":
                        {
                            correlMatrix = readCSVDouble(@"C:\pv254_tmp\simmatrix_itembased_correl.csv", ';');
                            break;
                        }
                    case "16":
                        {
                            cosineMatrix = readCSVDouble(@"C:\pv254_tmp\simmatrix_itembased_cosine.csv", ';');
                            break;
                        }
                    case "q":
                        {
                            return;
                        }
                }
            } while (true);
        }


        private static int?[][] readCSV(string filename, char separator)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            List<List<int?>> matrix = new List<List<int?>>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = line.Split(separator);

                List<int?> lineList = new List<int?>();
                foreach (string s in values)
                {
                    int val = 0;
                    if (int.TryParse(s, out val))
                        lineList.Add(val);
                    else
                        lineList.Add(null);
                }
                matrix.Add(lineList);
            }
            
            int rows = matrix.Count;

            if (rows == 0)
                return null;
                        
            int cols = matrix[0].Count;

            int?[][] result = new int?[rows][];

            for (int i = 0; i < rows; i++)
            {
                List<int?> row = matrix[i];
                if (cols != row.Count)
                    return null;
                result[i] = new int?[cols];
                for (int j = 0; j < cols; j++)
                {
                    result[i][j] = row[j];
                }
            }
            return result;
        }

        private static double?[][] readCSVDouble(string filename, char separator)
        {
            var reader = new StreamReader(File.OpenRead(filename));
            List<double?[]> matrix = new List<double?[]>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = line.Split(separator);

                double?[] lineList = new double?[values.Length];
                int i = 0;
                foreach (string s in values)
                {
                    double val = 0;
                    if (double.TryParse(s, out val))
                        lineList[i++] = val;
                    else
                        lineList[i++] = null;
                }
                matrix.Add(lineList);
            }

            int rows = matrix.Count;

            if (rows == 0)
                return null;

            int cols = matrix[0].Length;

            double?[][] result = new double?[rows][];

            for (int i = 0; i < rows; i++)
            {
                double?[] row = matrix[i];
                if (cols != row.Length)
                    return null;
                result[i] = row;
            }
            return result;
        }

        private static void writeCSV(string filename, char separator, double?[][] matrix)
        {
            if (matrix == null)
                return;

            List<string> csv = new List<string>();

            int n = matrix.GetLength(0);

            for (int i = 0; i < n; i++)
            {
                var line = new StringBuilder();
                for (int j = 0; j < n; j++)
                {
                    if (j > 0)
                        line.Append(separator);

                    line.Append(String.Format("{0:0.0000000000}", matrix[i][j]));
                }
                csv.Add(line.ToString());
            }
            File.WriteAllLines(filename, csv);            
        }

        private static double cosineSimilarity(int?[] a, int?[] b)
        {
            if (a.Length != b.Length)
                throw new ArgumentException("values must be the same length");                

            int N = a.Length;            
            double dot = 0.0d;
            double mag1 = 0.0d;
            double mag2 = 0.0d;
            for (int n = 0; n < N; n++)
            {
                int aR = a[n] == null ? 0 : (int)a[n];
                int bR = b[n] == null ? 0 : (int)b[n];

                dot += (aR*bR);

                mag1 += Math.Pow(aR, 2);
                mag2 += Math.Pow(bR, 2);
            }

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }

        private static double pearsonCorrelation(int?[] a, int?[] b, bool skipEmpty)
        {
            if(a.Length != b.Length)
                throw new ArgumentException("values must be the same length");

            int n = a.Length;

            int aSum = 0;
            int bSum = 0;
            int aCount = 0;
            int bCount = 0;

            for (int i = 0; i < n; ++i)
            {
                if (a[i] != null)
                {
                    aSum += (int)a[i];
                    aCount++;
                }
                if (b[i] != null)
                {
                    bSum += (int)b[i];
                    bCount++;
                }
            }

            double avgA = ((double)aSum) / aCount;
            double avgB = ((double)bSum) / bCount;

            Double sumTop = 0;
            Double sumBottomA = 0;
            Double sumBottomB = 0;

            for (int i = 0; i < n; ++i)
            {
                if (skipEmpty && (a[i] == null || b[i] == null))
                    continue;

                double aR = a[i] == null ? avgA : (int)a[i];
                double bR = b[i] == null ? avgB : (int)b[i];

                sumTop += (aR - avgA) * (bR - avgB);
                sumBottomA += Math.Pow((aR - avgA), 2);
                sumBottomB += Math.Pow((bR - avgB), 2);
            }
            
            return sumTop / (Math.Sqrt(sumBottomA) * Math.Sqrt(sumBottomB));

        }

        private static double rmse(double?[] a, double?[] b)
        {
            if (a.Length != b.Length)
                return(-1);
            
            double sum = 0;
            int count = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != null && b[i] != null && !Double.IsNaN((double)a[i]) &&!Double.IsNaN((double)b[i]))
                {
                    sum += Math.Pow((double)(a[i] - b[i]), 2);
                    count++;
                }
                    
            }
            return Math.Sqrt(sum/count);
        }

        private static double?[][] createCorrelationSimMatrix(int?[][] userItemMatrix)
        {
            int rows = userItemMatrix.Length;

            double?[][] result = new double?[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double?[rows];
            }

            Console.WriteLine("Skip empty lines in computation? y/n");
            string skip = Console.ReadLine();
            bool skipEmpty = skip == "y";

            Console.WriteLine("Computing pearson correlation similarity matrix.");

            for (int i = 0; i < rows; i++)
            {
                if (i % 100 == 0)
                    Console.WriteLine(i + ". row processing");
                for (int j = i; j < rows; j++)
                {
                    if (j == i)
                    {
                        result[i][j] = 1d;
                        continue;
                    }
                    result[i][j] = result[j][i] = pearsonCorrelation(userItemMatrix[i], userItemMatrix[j], skipEmpty);
                }
            }

            Console.WriteLine("Success!");
            return result;            
        }

        private static double?[][] createCosineSimMatrix(int?[][] userItemMatrix)
        {

            int rows = userItemMatrix.GetLength(0);

            double?[][] result = new double?[rows][];
            for (int i = 0; i < rows; i++)
            {
                result[i] = new double?[rows];
            }

            Console.WriteLine("Computing cosine similarity matrix.");

            for (int i = 0; i < rows; i++)
            {
                if (i % 100 == 0)
                    Console.WriteLine(i + ". row processing");
                for (int j = i; j < rows; j++)
                {
                    if (j == i)
                    {
                        result[i][j] = 1d;
                        continue;
                    }
                    result[i][j] = result[j][i] = cosineSimilarity(userItemMatrix[i], userItemMatrix[j]);
                }
            }

            Console.WriteLine("Success!");
            return result;
        }

        private static void testMatrix(double?[][] simMatrix, int?[][] testMatrix, int?[][] trainMatrix, double[] userAvgRatings, string info)
        {
            if (simMatrix == null)
            {
                Console.WriteLine("Matrix is null, generate it first!");
                return;
            }

            Console.WriteLine("Please enter number of neighbors:");
            string neighStr = Console.ReadLine();

            int neigh = 0;
            if (!int.TryParse(neighStr, out neigh))
            {
                Console.WriteLine("Not a number.");
                return;
            }

            int testCases = testMatrix.Length;

            double?[] ratings = new double?[testCases];
            double?[] predictions = new double?[testCases];

            for (int i = 0; i < testCases; i++) {
                if (i % 100 == 0)
                    Console.WriteLine("Testcase no. " + i + " / " + testCases);
                int?[] testRow = testMatrix[i];
                ratings[i] = testRow[2];
                predictions[i] = predictUserBased((int)testRow[0], (int)testRow[1], neigh, simMatrix, trainMatrix, userAvgRatings);
            }

            double rmseVal = rmse(ratings, predictions);

            Console.WriteLine(info);
            Console.WriteLine(neigh + " neighbors userbased RMSE: "+rmseVal);
        }

        private static void testItembasedMatrix(double?[][] simMatrix, int?[][] testMatrix, int?[][] trainMatrix, string info)
        {
            if (simMatrix == null)
            {
                Console.WriteLine("Matrix is null, generate it first!");
                return;
            }

            Console.WriteLine("Please enter number of neighbors:");
            string neighStr = Console.ReadLine();

            int neigh = 0;
            if (!int.TryParse(neighStr, out neigh))
            {
                Console.WriteLine("Not a number.");
                return;
            }

            int testCases = testMatrix.Length;

            double?[] ratings = new double?[testCases];
            double?[] predictions = new double?[testCases];

            for (int i = 0; i < testCases; i++)
            {
                if (i % 100 == 0)
                    Console.WriteLine("Testcase no. " + i + " / " + testCases);
                int?[] testRow = testMatrix[i];
                ratings[i] = testRow[2];
                predictions[i] = predictItemBased((int)testRow[0], (int)testRow[1], neigh, simMatrix, trainMatrix);
            }

            double rmseVal = rmse(ratings, predictions);

            Console.WriteLine(info);
            Console.WriteLine(neigh + " neighbors itembased RMSE: " + rmseVal);
        }

        private static double? predictUserBased(int movieId, int userId, int neighbors, double?[][] simMatrix, int?[][] trainMatrix, double[] userAvgRatings)
        {
            double?[] userRow = simMatrix[userId-1];
            List<double?> topSimilarities = userRow.Distinct().OrderByDescending(i => i).ToList();
            double? ratingSum = 0;
            double simSum = 0;
            int neighborsPassed = 0;

            foreach (double? simN in topSimilarities)
            {
                if (simN == null)
                    continue;

                double sim = (double)simN;

                int[] similarUserIds = userRow.Select((b,i) => b == sim ? i : -1).Where(i => i != -1).ToArray();
                foreach(int similarUserId in similarUserIds)
                {
                    int? similarUserRating = trainMatrix[similarUserId][movieId-1];
                    if (similarUserRating != null) 
                    {
                        ratingSum += sim*(userAvgRatings[similarUserId] - similarUserRating);
                        simSum += sim;
                        neighborsPassed++;
                    }
                    if (neighborsPassed >= neighbors)
                        break;
                }
                if (neighborsPassed >= neighbors)
                    break;
            }
            return userAvgRatings[userId-1] + (ratingSum/simSum);
        }

        private static double? predictItemBased(int movieId, int userId, int neighbors, double?[][] simMatrix, int?[][] trainMatrix)
        {
            double?[] itemRow = simMatrix[movieId - 1];
            List<double?> topSimilarities = itemRow.Distinct().OrderByDescending(i => i).ToList();
            double? ratingSum = 0;
            double simSum = 0;
            int neighborsPassed = 0;

            foreach (double? simN in topSimilarities)
            {
                if (simN == null)
                    continue;

                double sim = (double)simN;

                int[] similarItemIds = itemRow.Select((b, i) => b == sim ? i : -1).Where(i => i != -1).ToArray();
                foreach (int similarItemId in similarItemIds)
                {
                    int? similarItemRating = trainMatrix[userId - 1][similarItemId];
                    if (similarItemRating != null)
                    {
                        ratingSum += sim * similarItemRating;
                        simSum += sim;
                        neighborsPassed++;
                    }
                    if (neighborsPassed >= neighbors)
                        break;
                }
                if (neighborsPassed >= neighbors)
                    break;
            }
            return ratingSum / simSum;
        }

        private static void baselineUserBased(double[] userAvgRatings,  int?[][] testMatrix)
        {
            int testCases = testMatrix.Length;

            double?[] ratings = new double?[testCases];
            double?[] predictions = new double?[testCases];

            for (int i = 0; i < testCases; i++)
            {
                int?[] testRow = testMatrix[i];
                ratings[i]     = testRow[2];
                predictions[i] = userAvgRatings[(int)testRow[1]-1];
            }

            double rmseVal = rmse(ratings, predictions);

            Console.WriteLine("User-based baseline RMSE: " + rmseVal);
        }

        private static void baselineItemBased(double[] itemAvgRatings, int?[][] testMatrix)
        {
            int testCases = testMatrix.Length;

            double?[] ratings = new double?[testCases];
            double?[] predictions = new double?[testCases];

            for (int i = 0; i < testCases; i++)
            {
                int?[] testRow = testMatrix[i];
                ratings[i] = testRow[2];
                predictions[i] = itemAvgRatings[(int)testRow[0] - 1];
            }

            double rmseVal = rmse(ratings, predictions);

            Console.WriteLine("Item-based baseline RMSE: " + rmseVal);
        }

        private static double[] createItemBasedBaseline(int?[][] matrix)
        {
            int cols = matrix[0].Length;
            int rows = matrix.Length;
            double[] itemAvgRatings = new double[cols];

            for (int i = 0; i < cols; i++)
            {
                if (i % 100 == 0)
                    Console.WriteLine("Item no. " + i);

                double colSum = 0;
                int rowCount = 0;
                for (int j = 0; j < rows; j++)
                {
                    int? rat = matrix[j][i];
                    if (rat != null)
                    {
                        colSum += (int)rat;
                        rowCount++;
                    }
                }
                if (rowCount > 0)
                    itemAvgRatings[i] = colSum / rowCount;
            }
            return itemAvgRatings;
        }
    }
}
