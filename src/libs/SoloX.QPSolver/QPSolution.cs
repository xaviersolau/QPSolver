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
        public QPSolution(Vector<double> vectorX)
        {
            this.VectorX = vectorX;
        }

        /// <summary>
        /// Vector x solving the problem.
        /// </summary>
        public Vector<double> VectorX { get; }
    }
}
