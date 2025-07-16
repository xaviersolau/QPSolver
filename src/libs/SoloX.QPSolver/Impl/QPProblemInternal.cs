// ----------------------------------------------------------------------
// <copyright file="QPProblemInternal.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver.Impl
{
    internal sealed class QPProblemInternal
    {
        public IQPProblem QPProblem { get; }

        public QPProblemInternal(
            IQPProblem quadraticProgrammingProblem,
            Matrix<double> matrixAInequality,
            Vector<double> vectorBInequality)
        {
            QPProblem = quadraticProgrammingProblem;

            this.MatrixAInequality = matrixAInequality;
            this.VectorBInequality = vectorBInequality;

            VectorCTransposed = quadraticProgrammingProblem.VectorC.ToColumnMatrix().Transpose();
        }

        public Matrix<double> MatrixAInequality { get; }

        public Vector<double> VectorBInequality { get; }

        public Matrix<double> VectorCTransposed { get; }
    }
}
