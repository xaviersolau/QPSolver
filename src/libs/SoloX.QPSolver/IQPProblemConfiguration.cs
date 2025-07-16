// ----------------------------------------------------------------------
// <copyright file="IQPProblemConfiguration.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver
{
    /// <summary>
    /// Quadratic Programming Problem configuration interface.
    /// </summary>
    public interface IQPProblemConfiguration
    {
        /// <summary>
        /// Configure QP problem to minimizing F(x) = 1/2 * xT * Q * x + cT * x
        /// with:
        ///  xT as x transposed,
        ///  cT as c transposed,
        ///  Q as a symmetric matrix (often positive semidefinite).
        /// </summary>
        /// <param name="matrixQ">Matrix Q symmetric matrix.</param>
        /// <param name="vectorC">Vector c.</param>
        /// <returns>Self.</returns>
        IQPProblemConfiguration Minimizing(Matrix<double> matrixQ, Vector<double> vectorC);

        /// <summary>
        /// Subject to equality constraints: Aeq * x = beq
        /// </summary>
        /// <param name="matrixAEquality">Matrix Aeq.</param>
        /// <param name="vectorBEquality">Vector beq.</param>
        /// <returns>Self.</returns>
        IQPProblemConfiguration WithEquality(Matrix<double> matrixAEquality, Vector<double> vectorBEquality);

        /// <summary>
        /// Subject to inequality constraints: Aineq * x ≤ bineq
        /// </summary>
        /// <param name="matrixAInequality">Matrix Aineq.</param>
        /// <param name="vectorBInequality">Vector bineq.</param>
        /// <returns>Self.</returns>
        IQPProblemConfiguration WithInequality(Matrix<double> matrixAInequality, Vector<double> vectorBInequality);

        /// <summary>
        /// Subject to lower bounds constraints: lower ≤ x
        /// </summary>
        /// <param name="lowerBounds">Lower bounds.</param>
        /// <returns>Self.</returns>
        IQPProblemConfiguration WithLowerBounds(Vector<double> lowerBounds);

        /// <summary>
        /// Subject to upper bounds constraints: x ≤ upper
        /// </summary>
        /// <param name="upperBounds">Upper bounds.</param>
        /// <returns>Self.</returns>
        IQPProblemConfiguration WithUpperBounds(Vector<double> upperBounds);
    }
}
