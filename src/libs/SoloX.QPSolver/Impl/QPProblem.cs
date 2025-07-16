// ----------------------------------------------------------------------
// <copyright file="QPProblem.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;
using SoloX.QPSolver.Exceptions;

namespace SoloX.QPSolver.Impl
{
    /// <summary>
    /// Implement Quadratic Programming Problem definition.
    /// </summary>
    internal sealed class QPProblem : IQPProblem
    {
        private QPProblem(
            int size,
            Matrix<double> matrixQ,
            Vector<double> vectorC,
            Matrix<double> matrixAEquality,
            Vector<double> vectorBEquality,
            Matrix<double> matrixAInequality,
            Vector<double> vectorBInequality,
            Vector<double>? lowerBounds,
            Vector<double>? upperBounds)
        {
            Size = size;
            MatrixQ = matrixQ;
            VectorC = vectorC;
            MatrixAEquality = matrixAEquality;
            VectorBEquality = vectorBEquality;
            MatrixAInequality = matrixAInequality;
            VectorBInequality = vectorBInequality;
            LowerBounds = lowerBounds;
            UpperBounds = upperBounds;
        }

        /// <inheritdoc/>
        public int Size { get; }

        /// <inheritdoc/>
        public Matrix<double> MatrixQ { get; }

        /// <inheritdoc/>
        public Vector<double> VectorC { get; }

        /// <inheritdoc/>
        public Matrix<double> MatrixAEquality { get; }

        /// <inheritdoc/>
        public Vector<double> VectorBEquality { get; }

        /// <inheritdoc/>
        public Matrix<double> MatrixAInequality { get; }

        /// <inheritdoc/>
        public Vector<double> VectorBInequality { get; }

        /// <inheritdoc/>
        public Vector<double>? LowerBounds { get; }

        /// <inheritdoc/>
        public Vector<double>? UpperBounds { get; }

        /// <summary>
        /// Create and configure a Quadratic Programming problem.
        /// </summary>
        /// <param name="configure">Configuration handler.</param>
        /// <returns>The configured problem.</returns>
        /// <exception cref="QPEngineException"></exception>
        public static IQPProblem Create(Action<IQPProblemConfiguration> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var validator = new QPProblemValidator();
            configure(validator);

            if (validator.Size == null)
            {
                throw new QPEngineException("Unable to evaluate QP problem size.");
            }

            var size = validator.Size.Value;

            var initializer = new QPProblemInitializer();
            configure(initializer);

            var matrixQ = initializer.MatrixQ ?? Matrix<double>.Build.Sparse(size, size, 0.0);
            var vectorC = initializer.VectorC ?? Vector<double>.Build.Sparse(size, 0.0);

            var matrixAEquality = initializer.MatrixAEquality ?? Matrix<double>.Build.Sparse(0, size);
            var vectorBEquality = initializer.VectorBEquality ?? Vector<double>.Build.Sparse(0);

            // Prepare inequality matrices
            var matrixAInequality = initializer.MatrixAInequality ?? Matrix<double>.Build.Sparse(0, size);
            var vectorBInequality = initializer.VectorBInequality ?? Vector<double>.Build.Sparse(0);

            var lowerBounds = initializer.LowerBounds;
            var upperBounds = initializer.UpperBounds;

            return new QPProblem(
                size,
                matrixQ,
                vectorC,
                matrixAEquality,
                vectorBEquality,
                matrixAInequality,
                vectorBInequality,
                lowerBounds,
                upperBounds);
        }
    }
}
