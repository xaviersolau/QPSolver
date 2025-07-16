// ----------------------------------------------------------------------
// <copyright file="QPProblemInitializer.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver.Impl
{
    internal sealed class QPProblemInitializer : IQPProblemConfiguration
    {
        public Matrix<double>? MatrixQ { get; set; }

        public Vector<double>? VectorC { get; set; }

        public Matrix<double>? MatrixAEquality { get; set; }

        public Vector<double>? VectorBEquality { get; set; }

        public Matrix<double>? MatrixAInequality { get; set; }

        public Vector<double>? VectorBInequality { get; set; }

        public Vector<double>? LowerBounds { get; set; }

        public Vector<double>? UpperBounds { get; set; }

        public IQPProblemConfiguration Minimizing(Matrix<double> matrixQ, Vector<double> vectorC)
        {
            MatrixQ = matrixQ;
            VectorC = vectorC;

            return this;
        }

        public IQPProblemConfiguration WithEquality(Matrix<double> matrixAEquality, Vector<double> vectorBEquality)
        {
            if (MatrixAEquality == null)
            {
                MatrixAEquality = matrixAEquality;
                VectorBEquality = vectorBEquality;
            }
            else
            {
                MatrixAEquality = MatrixAEquality.Stack(matrixAEquality);
                VectorBEquality = Vector<double>.Build.DenseOfEnumerable(VectorBEquality!.Concat(vectorBEquality));
            }

            return this;
        }

        public IQPProblemConfiguration WithInequality(Matrix<double> matrixAInequality, Vector<double> vectorBInequality)
        {
            if (MatrixAInequality == null)
            {
                MatrixAInequality = matrixAInequality;
                VectorBInequality = vectorBInequality;
            }
            else
            {
                MatrixAInequality = MatrixAInequality.Stack(matrixAInequality);
                VectorBInequality = Vector<double>.Build.DenseOfEnumerable(VectorBInequality!.Concat(vectorBInequality));
            }

            return this;
        }

        public IQPProblemConfiguration WithLowerBounds(Vector<double> lowerBounds)
        {
            LowerBounds = lowerBounds;

            return this;
        }

        public IQPProblemConfiguration WithUpperBounds(Vector<double> upperBounds)
        {
            UpperBounds = upperBounds;

            return this;
        }
    }
}
