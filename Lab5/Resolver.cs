using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.Math;

namespace Lab5
{
    public class MinorResolver
    {
        private readonly double[,] _matrix;

        public MinorResolver(double[,] matrix)
        {
            _matrix = matrix;
        }

        public double[,] Resolve() // include only start!
        {
            var size = _matrix.GetLength(0);
            var minorMatrix = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    minorMatrix[i, j] = GetMinor(i, j);
                }
            }

            return minorMatrix;
        }

        private double GetMinor(int row, int column)
        {
            var size = _matrix.GetLength(0);
            var result = new double[size - 1, size - 1];

            int m = 0;
            int k = 0;

            for (int i = 0; i < size; i++)
            {
                if (i == row)
                {
                    continue;
                }

                k = 0;
                for (int j = 0; j < size; j++)
                {
                    if (j == column)
                    {
                        continue;
                    }

                    result[m, k++] = _matrix[i, j];
                }

                m++;
            }

            return result.Determinant();
        }
    }

    internal class Resolver
    {
        public static double[] ReadVector(string line)
        {
            List<double> answer = new List<double>();
            var vector = line.Split(' ');
            foreach (var x in vector)
                answer.Add(Convert.ToDouble(x));
            var vectorA = new double[answer.Count];
            for (int i = 0; i < answer.Count; i++)
                vectorA[i] = answer[i];
            return vectorA;
        }

        public static double[,] ReadMatrix(string line)
        {
            var matr = line.Split(' ');
            var size = Convert.ToInt32(Math.Sqrt(matr.Length));
            var count = 0;
            var matrix = new double[size, size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    matrix[i, j] = Convert.ToDouble(matr[count]);
                    count++;
                }
            }
            return matrix;
        }

        public static string Result(double[,] matrix, double[] v)
        {
            var resStr = "";
            //double[,] matrix = new double[,] { { 2, 3, 3, 1 },
            //                         { 3, 5, 3, 2 },
            //                         { 5, 7, 6, 2 },
            //                         { 4, 4, 3, 1 } };
            //double[] v = new double[] { 1, 2, 3, 4 };
            var det = matrix.Determinant();
            if (det == 0)
            {
                resStr += ("Определитель не может быть 0");
            }

            var resolver = new MinorResolver(matrix);
            var minors = resolver.Resolve();

            var result = minors.CreateAdditions();
            result = result.Transpose();
            var result1 = result.Dot(v);
            var itog = result1.Divide(-3);
            for (var i = 0; i < itog.Length; i++)
            {
                Console.WriteLine(Math.Round(itog[i], 2));
                resStr += Math.Round(itog[i], 2) + " ";
            }

            return resStr;
            // Все, result - и есть обратная матрица 
        }
    }
}
