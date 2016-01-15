using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkingTests
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine();
			Console.WriteLine("Begin neural network back-propagation demo");

			var numInput = 4; // number features
			var numHidden = 5;
			var numOutput = 3; // number of classes for Y
			var numRows = 1000;
			var seed = 1; // gives nice demo

			Console.WriteLine();
			Console.WriteLine("Generating {0} artificial data items with {1} features", numRows, numInput);
			var allData = MakeAllData(numInput, numHidden, numOutput, numRows, seed);
			Console.WriteLine("Done");

			//ShowMatrix(allData, allData.Length, 2, true);

			Console.WriteLine();
			Console.WriteLine("Creating train (80%) and test (20%) matrices");
			double[][] trainData;
			double[][] testData;
			SplitTrainTest(allData, 0.80, seed, out trainData, out testData);
			Console.WriteLine("Done.");
			Console.WriteLine();

			Console.WriteLine("Training data:");
			ShowMatrix(trainData, 4, 2, true);
			Console.WriteLine("Test data:");
			ShowMatrix(testData, 4, 2, true);

			Console.WriteLine("Creating a {0}-{1}-{2} neural network.", numInput, numHidden, numOutput);
			var nn = new NeuralNetwork(numInput, numHidden, numOutput);

			// This value is arbitrary.
			var maxEpochs = 1000;

			// The learning rate controls how much each weight and bias value can change in each update step.
			// Larger values increase the speed of training at the risk of overshooting optimal weight values.
			var learnRate = 0.05;

			// The momentum rate helps prevent training from getting stuck with local, non-optimal weight
			// values and also prevents oscillation where training never converges to stable values.
			var momentum = 0.01;
			Console.WriteLine();
			Console.WriteLine("Setting maxEpochs = {0}", maxEpochs);
			Console.WriteLine("Setting learnRate = {0}", learnRate.ToString("F2"));
			Console.WriteLine("Setting momentum  = {0}", momentum.ToString("F2"));

			Console.WriteLine();
			Console.WriteLine("Starting training");
			var weights = nn.Train(trainData, maxEpochs, learnRate, momentum);
			Console.WriteLine("Done.");
			Console.WriteLine();
			Console.WriteLine("Final neural network model weights and biases:");
			Console.WriteLine();
			ShowVector(weights, 2, 10, true);

			//double[] y = nn.ComputeOutputs(new double[] { 1.0, 2.0, 3.0, 4.0 });
			//ShowVector(y, 3, 3, true);

			double trainAcc = nn.Accuracy(trainData);
			Console.WriteLine();
			Console.WriteLine("Final accuracy on training data = {0}", trainAcc.ToString("F4"));

			var testAcc = nn.Accuracy(testData);
			Console.WriteLine("Final accuracy on test data     = {0}", testAcc.ToString("F4"));

			Console.WriteLine();
			Console.WriteLine("End back-propagation demo");
			Console.WriteLine();
			Console.ReadLine();
		}

		private static void ShowMatrix(double[][] matrix, int numRows, int decimals, bool indices)
		{
			var len = matrix.Length.ToString().Length;
			for (var i = 0; i < numRows; ++i)
			{
				if (indices)
				{
					Console.Write("[" + i.ToString().PadLeft(len) + "]  ");
				}
				for (var j = 0; j < matrix[i].Length; ++j)
				{
					var v = matrix[i][j];
					if (v >= 0.0)
					{
						Console.Write(" "); // '+'
					}
					Console.Write(v.ToString("F" + decimals) + "  ");
				}
				Console.WriteLine("");
			}

			if (numRows < matrix.Length)
			{
				Console.WriteLine(". . .");
				var lastRow = matrix.Length - 1;
				if (indices)
				{
					Console.Write("[" + lastRow.ToString().PadLeft(len) + "]  ");
				}
				for (var j = 0; j < matrix[lastRow].Length; ++j)
				{
					var v = matrix[lastRow][j];
					if (v >= 0.0)
					{
						Console.Write(" "); // '+'
					}
					Console.Write(v.ToString("F" + decimals) + "  ");
				}
			}
			Console.WriteLine("\n");
		}

		private static void ShowVector(double[] vector, int decimals, int lineLen, bool newLine)
		{
			for (var i = 0; i < vector.Length; ++i)
			{
				if ((i > 0) && ((i % lineLen) == 0))
				{
					Console.WriteLine("");
				}
				if (vector[i] >= 0)
				{
					Console.Write(" ");
				}
				Console.Write(vector[i].ToString("F" + decimals) + " ");
			}
			if (newLine)
			{
				Console.WriteLine("");
			}
		}

		/// <summary>
		/// Generate a synthetic data set.
		/// </summary>
		/// <param name="numInput"></param>
		/// <param name="numHidden"></param>
		/// <param name="numOutput"></param>
		/// <param name="numRows"></param>
		/// <param name="seed"></param>
		/// <returns></returns>
		private static double[][] MakeAllData(int numInput, int numHidden, int numOutput, int numRows, int seed)
		{
			var rnd = new Random(seed);
			var numWeights = (numInput * numHidden) + numHidden + (numHidden * numOutput) + numOutput;
			var weights = new double[numWeights]; // actually weights & biases
			for (var i = 0; i < numWeights; ++i)
			{
				weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to 10.0]
			}

			Console.WriteLine("Generating weights and biases:");
			ShowVector(weights, 2, 10, true);

			var result = new double[numRows][]; // allocate return-result
			for (var i = 0; i < numRows; ++i)
			{
				result[i] = new double[numInput + numOutput]; // 1-of-N in last column
			}

			var gnn = new NeuralNetwork(numInput, numHidden, numOutput); // generating NN
			gnn.SetWeights(weights);

			for (var r = 0; r < numRows; ++r)
			{
				// generate random inputs
				var inputs = new double[numInput];
				for (int i = 0; i < numInput; ++i)
				{
					inputs[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to -10.0]
				}

				// compute outputs
				var outputs = gnn.ComputeOutputs(inputs);

				// translate outputs to 1-of-N
				var oneOfN = new double[numOutput]; // all 0.0

				var maxIndex = 0;
				var maxValue = outputs[0];
				for (var i = 0; i < numOutput; ++i)
				{
					if (outputs[i] > maxValue)
					{
						maxIndex = i;
						maxValue = outputs[i];
					}
				}
				oneOfN[maxIndex] = 1.0;

				// place inputs and 1-of-N output values into curr row
				var c = 0; // column into result[][]
				for (var i = 0; i < numInput; ++i) // inputs
				{
					result[r][c++] = inputs[i];
				}
				for (var i = 0; i < numOutput; ++i) // outputs
				{
					result[r][c++] = oneOfN[i];
				}
			}
			return result;
		}

		/// <summary>
		/// Split the data randomly into training data and test data.
		/// </summary>
		/// <param name="allData"></param>
		/// <param name="trainPct"></param>
		/// <param name="seed"></param>
		/// <param name="trainData"></param>
		/// <param name="testData"></param>
		private static void SplitTrainTest(double[][] allData, double trainPct, int seed, out double[][] trainData, out double[][] testData)
		{
			var rnd = new Random(seed);
			var totRows = allData.Length;
			var numTrainRows = (int)(totRows * trainPct); // usually 0.80
			var numTestRows = totRows - numTrainRows;
			trainData = new double[numTrainRows][];
			testData = new double[numTestRows][];

			var copy = new double[allData.Length][]; // ref copy of data
			for (var i = 0; i < copy.Length; ++i)
			{
				copy[i] = allData[i];
			}

			for (var i = 0; i < copy.Length; ++i) // scramble order
			{
				var r = rnd.Next(i, copy.Length); // use Fisher-Yates
				var tmp = copy[r];
				copy[r] = copy[i];
				copy[i] = tmp;
			}
			for (var i = 0; i < numTrainRows; ++i)
			{
				trainData[i] = copy[i];
			}

			for (var i = 0; i < numTestRows; ++i)
			{
				testData[i] = copy[i + numTrainRows];
			}
		}
	}
}
