using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5
{
    public static class AdditionsSolveExtensions
    {
        public static double[,] CreateAdditions(this double[,] matrix)
        {
            var size = matrix.GetLength(0);
            var result = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] = Math.Round(Math.Pow((-1), i + j) * matrix[i, j], 2);
                }
            }

            return result;
        }
    }
}
