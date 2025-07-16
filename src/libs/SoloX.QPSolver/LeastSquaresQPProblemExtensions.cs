// ----------------------------------------------------------------------
// <copyright file="LeastSquaresQPProblemExtensions.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver
{
    /// <summary>
    /// LeastSquares QPProblem Configuration Extensions
    /// </summary>
    public static class LeastSquaresQPProblemExtensions
    {
        /// <summary>
        /// Configure QP problem to minimizing LeastSquares || A x - b ||²
        /// </summary>
        /// <param name="configuration">The configuration handler.</param>
        /// <param name="matrixA">Matrix A.</param>
        /// <param name="vectorB">Vector b.</param>
        /// <returns>Self.</returns>
        public static IQPProblemConfiguration MinimizingLeastSquares(
            this IQPProblemConfiguration configuration,
            Matrix<double> matrixA,
            Vector<double> vectorB)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(matrixA);

            var matrixATransposed = matrixA.Transpose();

            var matrixQ = 2.0 * matrixATransposed * matrixA;
            var vectorC = -2.0 * matrixATransposed * vectorB;

            return configuration.Minimizing(matrixQ, vectorC);
        }
    }
}
