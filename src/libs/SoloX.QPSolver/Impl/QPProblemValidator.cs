// ----------------------------------------------------------------------
// <copyright file="QPProblemValidator.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;
using SoloX.QPSolver.Exceptions;

namespace SoloX.QPSolver.Impl
{
    internal sealed class QPProblemValidator : IQPProblemConfiguration
    {
        public int? Size { get; private set; }

        private bool minimizingConfigured;
        private bool lowerBoundsConfigured;
        private bool upperBoundsConfigured;

        public IQPProblemConfiguration Minimizing(Matrix<double> matrixQ, Vector<double> vectorC)
        {
            if (matrixQ.ColumnCount != matrixQ.RowCount)
            {
                throw new QPEngineException("The matrix Q must be a square matrix.");
            }

            if (matrixQ.ColumnCount != vectorC.Count)
            {
                throw new QPEngineException("The vector c size must be equal to matrix Q columns/rows count.");
            }

            if (Size == null)
            {
                Size = vectorC.Count;
            }
            else if (Size != vectorC.Count)
            {
                throw new QPEngineException($"The vector c size must be {Size} and the matrix Q must be {Size}x{Size}.");
            }

            if (this.minimizingConfigured)
            {
                throw new QPEngineException($"Minimizing configuration method already called.");
            }

            this.minimizingConfigured = true;

            return this;
        }

        public IQPProblemConfiguration WithEquality(Matrix<double> matrixAEquality, Vector<double> vectorBEquality)
        {
            if (matrixAEquality.RowCount != vectorBEquality.Count)
            {
                throw new QPEngineException($"The matrix Aeq row count must be equal to the vector beq size.");
            }

            if (Size == null)
            {
                Size = matrixAEquality.ColumnCount;
            }
            else if (Size != matrixAEquality.ColumnCount)
            {
                throw new QPEngineException($"The matrix Aeq must have {Size} columns.");
            }

            return this;
        }

        public IQPProblemConfiguration WithInequality(Matrix<double> matrixAInequality, Vector<double> vectorBInequality)
        {
            if (matrixAInequality.RowCount != vectorBInequality.Count)
            {
                throw new QPEngineException($"The matrix Aineq row count must be equal to the vector bineq size.");
            }

            if (Size == null)
            {
                Size = matrixAInequality.ColumnCount;
            }
            else if (Size != matrixAInequality.ColumnCount)
            {
                throw new QPEngineException($"The matrix Aineq must have {Size} columns.");
            }

            return this;
        }

        public IQPProblemConfiguration WithLowerBounds(Vector<double> lowerBounds)
        {
            if (Size == null)
            {
                Size = lowerBounds.Count;
            }
            else if (Size != lowerBounds.Count)
            {
                throw new QPEngineException($"The lower bound must have {Size} size.");
            }

            if (this.lowerBoundsConfigured)
            {
                throw new QPEngineException($"WithLowerBounds configuration method already called.");
            }

            this.lowerBoundsConfigured = true;

            return this;
        }

        public IQPProblemConfiguration WithUpperBounds(Vector<double> upperBounds)
        {
            if (Size == null)
            {
                Size = upperBounds.Count;
            }
            else if (Size != upperBounds.Count)
            {
                throw new QPEngineException($"The upper bound must have {Size} size.");
            }

            if (this.upperBoundsConfigured)
            {
                throw new QPEngineException($"WithUpperBounds configuration method already called.");
            }

            this.upperBoundsConfigured = true;

            return this;
        }
    }
}
