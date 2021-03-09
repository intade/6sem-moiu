﻿using System;
using System.Collections.Generic;
using System.Linq;
using Accord.Math;

namespace lab2
{
    internal class lab2
    {
        private static List<int> basis_indexes;
        private static List<int> non_basis_indexes;
        private static double[,] matrix_B;
        private static double[,] inverse_matrix_B;
        private static double[,] matrix_N;

        public static void Main(string[] args)
        {
            Console.WriteLine("Enter the dimension:");
            var dims = int.Parse(Console.ReadLine() ?? throw new Exception());
            Console.WriteLine("Enter the target function's coefs:");
            var c = MoiuLib.FillMatrix(1, dims);
            
            Console.WriteLine("Enter count of conditions:");
            var rows = int.Parse(Console.ReadLine() ?? throw new Exception());
            
            if(rows > dims)
                throw new Exception("count of conditions must be less or equal than dimension");
            
            Console.WriteLine("Fill the matrix:");
            var matrix = MoiuLib.FillMatrix(rows, dims);

            Console.WriteLine("Enter zero point:");
            var x = MoiuLib.FillMatrix(1, dims);

            basis_indexes = x.GetRow(0).Select(((d, i) => new {d, i})).OrderByDescending((arg => arg.d)).Take(rows).Select((arg => arg.i)).ToList();
            non_basis_indexes = x.GetRow(0).Select(((d, i) => i)).ToList().Except(basis_indexes).ToList();
            
            matrix_B = GetSubMatrixByCols(matrix, basis_indexes.ToArray());
            matrix_N = GetSubMatrixByCols(matrix, non_basis_indexes.ToArray());
            
            Console.WriteLine("matrix_B");
            MoiuLib.PrintMatrix(matrix_B);

            double[,] changeVector = new double[1,1];
            int changeIndex = 0;
            inverse_matrix_B = inverse_matrix_B != null ? MoiuLib.Algorithm(matrix_B, inverse_matrix_B, changeVector, changeIndex ) : matrix_B.Inverse();

            double[,] Cn = c.GetRow(0).Where((d, i) => non_basis_indexes.Contains(i)).ToList().Make2DArray();
            double[,] Cb = c.GetRow(0).Where((d, i) => basis_indexes.Contains(i)).ToList().Make2DArray();
            var div = FindDiv(Cn, Cb, inverse_matrix_B, matrix_N);

            Console.WriteLine("div");
            MoiuLib.PrintMatrix(div);
            var c_indexes_more_zero = div.GetRow(0).Select(((d, i) => new {d, i}))
                .Where((arg => arg.d > 0))
                .Select((arg => arg.i))
                .ToList();
            
            while (c_indexes_more_zero.Count > 0)
            {
                var c_index = c_indexes_more_zero[0];
            }  

        }

        public static double[,] FindDiv(
            double[,] Cn,
            double[,] Cb,
            double[,] Ab_inv,
            double[,] An)
        {
            var step1 = MoiuLib.MultiplyMatrix(Cb, Ab_inv);
            var step2 = MoiuLib.MultiplyMatrix(step1, An);
            var step3 = MoiuLib.MultiplyMatrixAndScalar(step2, -1);
            var result = MoiuLib.SumMatrix(Cn, step3);
            return result;
        }

        private static double[,] GetSubMatrixByCols(double[,] matrix, int[] basis)
        {
            var cols = basis.Length;
            var rows = matrix.GetLength(0);
            var result = new double[rows, cols];
            for (var i = 0; i < cols; i++)
            {
                var index = basis[i];
                for (var j = 0; j < rows; j++)
                {
                    result[j, i] = matrix[j, index];
                }
            }
            return result;
        }   
    }
}