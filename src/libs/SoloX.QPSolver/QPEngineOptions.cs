// ----------------------------------------------------------------------
// <copyright file="QPEngineOptions.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

namespace SoloX.QPSolver
{
    /// <summary>
    /// Quadratic Programming Engine options.
    /// </summary>
    public class QPEngineOptions
    {
        /// <summary>
        /// Engine tolerance.
        /// </summary>
        public double Tolerance { get; set; } = 1e-8;

        /// <summary>
        /// Max Iterations to solve the problem.
        /// </summary>
        public int MaxIterations { get; set; } = 100;

    }
}
