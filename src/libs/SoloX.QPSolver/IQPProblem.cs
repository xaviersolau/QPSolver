// ----------------------------------------------------------------------
// <copyright file="IQPProblem.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver
{
    /// <summary>
    /// Quadratic Programming Problem definition interface.
    /// Finding x minimizing F(x)
    /// With 
    /// F(x) = 1/2 * xT * Q * x + cT * x
    /// and
    /// Aeq * x = beq
    /// and
    /// Aineq * x ≤ bIneq
    /// And if bounds is specified:
    /// LowerBounds ≤ x ≤ UpperBounds
    /// </summary>
    public interface IQPProblem
    {
        /// <summary>
        /// Problem size.
        /// </summary>
        int Size { get; }

        /// <summary>
        /// Matrix Q.
        /// </summary>
        Matrix<double> MatrixQ { get; }

        /// <summary>
        /// Vector c.
        /// </summary>
        Vector<double> VectorC { get; }

        /// <summary>
        /// Matrix A Equality.
        /// </summary>
        Matrix<double> MatrixAEquality { get; }

        /// <summary>
        /// Vector b Equality.
        /// </summary>
        Vector<double> VectorBEquality { get; }

        /// <summary>
        /// Matrix A Inequality.
        /// </summary>
        Matrix<double> MatrixAInequality { get; }

        /// <summary>
        /// Vector b Inequality.
        /// </summary>
        Vector<double> VectorBInequality { get; }

        /// <summary>
        /// Lower bounds Vector.
        /// </summary>
        Vector<double>? LowerBounds { get; }

        /// <summary>
        /// Upper bounds Vector.
        /// </summary>
        Vector<double>? UpperBounds { get; }
    }
}
