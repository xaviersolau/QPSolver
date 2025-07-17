// ----------------------------------------------------------------------
// <copyright file="QPSolution.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver
{
    /// <summary>
    /// Quadratic Programming problem solution.
    /// </summary>
    public class QPSolution
    {
        /// <summary>
        /// Setup instance.
        /// </summary>
        /// <param name="vectorX">Vector x solving the problem.</param>
        /// <param name="iterationCount">Iteration Count to solve the problem.</param>
        public QPSolution(Vector<double> vectorX, int iterationCount)
        {
            this.VectorX = vectorX;
            IterationCount = iterationCount;
        }

        /// <summary>
        /// Vector x solving the problem.
        /// </summary>
        public Vector<double> VectorX { get; }

        /// <summary>
        /// Iteration Count to solve the problem.
        /// </summary>
        public int IterationCount { get; }
    }
}
