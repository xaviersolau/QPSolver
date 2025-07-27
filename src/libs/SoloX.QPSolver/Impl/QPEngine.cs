// ----------------------------------------------------------------------
// <copyright file="QPEngine.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Microsoft.Extensions.Options;

namespace SoloX.QPSolver.Impl
{
    /// <summary>
    /// Quadratic Programming Engine implementation.
    /// </summary>
    public class QPEngine : IQPEngine
    {
        private readonly QPEngineOptions options;

        /// <summary>
        /// Setup instance with the given options.
        /// </summary>
        /// <param name="options"></param>
        public QPEngine(IOptions<QPEngineOptions> options)
        {
            this.options = options?.Value ?? new QPEngineOptions();
        }

        /// <inheritdoc/>
        public IQPProblem CreateProblem(Action<IQPProblemConfiguration> configure)
        {
            return QPProblem.Create(configure);
        }

        /// <inheritdoc/>
        public QPSolution Solve(IQPProblem quadraticProgrammingProblem)
        {
            ArgumentNullException.ThrowIfNull(quadraticProgrammingProblem);

            var size = quadraticProgrammingProblem.Size;
            var lowerBounds = quadraticProgrammingProblem.LowerBounds;
            var upperBounds = quadraticProgrammingProblem.UpperBounds;
            var matrixAInequalityFinal = quadraticProgrammingProblem.MatrixAInequality;
            var vectorBInequality = quadraticProgrammingProblem.VectorBInequality;

            if (lowerBounds != null)
            {
                var lowerA = Matrix<double>.Build.Sparse(size, size, (i, j) => i == j ? -1 : 0);
                var lowerB = -lowerBounds;
                matrixAInequalityFinal = matrixAInequalityFinal.Stack(lowerA);
                vectorBInequality = Vector<double>.Build.DenseOfEnumerable(vectorBInequality.Concat(lowerB));
            }

            if (upperBounds != null)
            {
                var upperA = Matrix<double>.Build.Sparse(size, size, (i, j) => i == j ? 1 : 0);
                var upperB = upperBounds;
                matrixAInequalityFinal = matrixAInequalityFinal.Stack(upperA);
                vectorBInequality = Vector<double>.Build.DenseOfEnumerable(vectorBInequality.Concat(upperB));
            }

            var problem = new QPProblemInternal(quadraticProgrammingProblem, matrixAInequalityFinal, vectorBInequality);

            var solution = Solve(problem);

            var clamping = new BoundsClamping(lowerBounds, upperBounds, this.options.Tolerance);

            clamping.ApplyTo(solution.VectorX);

            return solution;
        }

        /// <inheritdoc/>
        public double Fx(IQPProblem quadraticProgrammingProblem, Vector<double> x)
        {
            ArgumentNullException.ThrowIfNull(quadraticProgrammingProblem);
            ArgumentNullException.ThrowIfNull(x);

            var matrixQ = quadraticProgrammingProblem.MatrixQ;
            var vectorC = quadraticProgrammingProblem.VectorC;

            var fx = 1.0 / 2.0 * x.ToColumnMatrix().TransposeThisAndMultiply(matrixQ) * x + vectorC.ToColumnMatrix().TransposeThisAndMultiply(x);

            return fx[0];
        }

        /// <inheritdoc/>
        public bool CheckInequalityConstraints(IQPProblem quadraticProgrammingProblem, Vector<double> x)
        {
            ArgumentNullException.ThrowIfNull(quadraticProgrammingProblem);
            ArgumentNullException.ThrowIfNull(x);

            return CheckInequalityConstraints(quadraticProgrammingProblem.MatrixAInequality, quadraticProgrammingProblem.VectorBInequality, x);
        }

        /// <inheritdoc/>
        public bool CheckEqualityConstraints(IQPProblem quadraticProgrammingProblem, Vector<double> x)
        {
            ArgumentNullException.ThrowIfNull(quadraticProgrammingProblem);
            ArgumentNullException.ThrowIfNull(x);

            return CheckEqualityConstraints(quadraticProgrammingProblem.MatrixAEquality, quadraticProgrammingProblem.VectorBEquality, x);
        }

        /// <inheritdoc/>
        public bool CheckBoundsConstraints(IQPProblem quadraticProgrammingProblem, Vector<double> x)
        {
            ArgumentNullException.ThrowIfNull(quadraticProgrammingProblem);
            ArgumentNullException.ThrowIfNull(x);

            var lowerBounds = quadraticProgrammingProblem.LowerBounds;
            var upperBounds = quadraticProgrammingProblem.UpperBounds;

            for (var i = 0; i < x.Count; i++)
            {
                var xi = x[i];
                if ((lowerBounds?[i] ?? double.NegativeInfinity) > xi || xi > (upperBounds?[i] ?? double.PositiveInfinity))
                {
                    return false;
                }
            }

            return true;
        }

        private QPSolution Solve(QPProblemInternal problem)
        {
            var matrixQ = problem.QPProblem.MatrixQ;
            var vectorC = problem.QPProblem.VectorC;
            var matrixAEquality = problem.QPProblem.MatrixAEquality;
            var vectorBEquality = problem.QPProblem.VectorBEquality;
            var matrixAInequality = problem.MatrixAInequality;
            var vectorBInequality = problem.VectorBInequality;

            var maxIter = this.options.MaxIterations;
            var tolerance = this.options.Tolerance;

            var n = matrixQ.RowCount;
            var x = Vector<double>.Build.Dense(n, 0);

            // Initialize active set with equality constraints
            var activeSet = new List<int>();

            var previousActiveSetCount = -1;

            var originalEqualityCount = matrixAEquality.RowCount;

            // Project to equality constraints
            if (matrixAEquality.RowCount > 0)
            {
                var matrixAEqualityTransposed = matrixAEquality.Transpose();
                var matrixAAT = matrixAEquality * matrixAEqualityTransposed;
                var lambdaEq = matrixAAT.Solve(vectorBEquality - matrixAEquality * x);
                x += matrixAEqualityTransposed * lambdaEq;
            }

            Matrix<double> kktMatrix = null!;
            Vector<double> rhs = null!;
            Matrix<double> matrixAActive = null!;
            Vector<double> vectorBActive = null!;
            var m = 0;

            var bestFxMin = double.PositiveInfinity;

            Vector<double>? bestX = null;

            for (var iter = 1; iter <= maxIter; iter++)
            {
                // Setup new KKT matrix if activeSet changed
                if (previousActiveSetCount != activeSet.Count)
                {
                    // activeSet changed so we need a new KKT matrix.
                    previousActiveSetCount = activeSet.Count;

                    // Build active constraint matrix A and vector b
                    matrixAActive = BuildActiveMatrix(matrixAEquality, matrixAInequality, activeSet);
                    vectorBActive = BuildActiveVector(vectorBEquality, vectorBInequality, activeSet);

                    m = matrixAActive.RowCount;

                    kktMatrix = Matrix<double>.Build.Dense(n + m, n + m);
                    kktMatrix.SetSubMatrix(0, n, 0, n, matrixQ);
                    kktMatrix.SetSubMatrix(0, n, n, m, matrixAActive.Transpose());
                    kktMatrix.SetSubMatrix(n, m, 0, n, matrixAActive);

                    rhs = Vector<double>.Build.Dense(n + m, 0);
                }

                // Solve KKT system
                rhs.SetSubVector(0, n, -(matrixQ * x + vectorC));
                rhs.SetSubVector(n, m, vectorBActive - matrixAActive * x);

                var sol = kktMatrix.Solve(rhs);

                if (sol.Take(n).All(double.IsNaN))
                {
                    var pi = kktMatrix.PseudoInverse();

                    sol = pi * rhs;
                }

                var dx = sol.SubVector(0, n);
                var lambda = sol.SubVector(n, m);

                var dxNorm = dx.L2Norm();

                // Check for optimality
                if (dxNorm < tolerance)
                {
                    // Check Lagrange multipliers for inequality constraints
                    var removedOne = false;

                    for (var i = 0; i < activeSet.Count; i++)
                    {
                        if (lambda[i + originalEqualityCount] < -tolerance)
                        {
                            removedOne = true;
                            activeSet.RemoveAt(i);
                            break;
                        }
                    }

                    if (!removedOne)
                    {
                        return new QPSolution(x, iter);
                    }
                }
                else
                {
                    // Line search to maximum feasible step
                    var alpha = 1.0;
                    var blockingConstraint = -1;

                    for (var i = 0; i < matrixAInequality.RowCount; i++)
                    {
                        if (activeSet.Contains(i))
                        {
                            continue;
                        }

                        var ai = matrixAInequality.Row(i);
                        var bi = vectorBInequality[i];

                        var aidx = ai * dx;

                        if (aidx > tolerance)
                        {
                            var aix = ai * x;

                            var step = (bi - aix) / aidx;
                            if (step < alpha)
                            {
                                alpha = step;
                                blockingConstraint = i;
                            }
                        }
                    }

                    x += alpha * dx;

                    if (blockingConstraint >= 0)
                    {
                        activeSet.Add(blockingConstraint);
                    }
                    else
                    {
                        // Search for a new blocking Inequality Constraint.
                        for (var i = 0; i < matrixAInequality.RowCount; i++)
                        {
                            if (activeSet.Contains(i))
                            {
                                continue;
                            }

                            var ai = matrixAInequality.Row(i);

                            var aix = ai * x;
                            var bi = vectorBInequality[i];

                            if (aix - bi > tolerance)
                            {
                                activeSet.Add(i);
                                blockingConstraint = i;
                                break;
                            }
                        }
                    }

                    // Register best result if no new blocking Constraint.
                    if (blockingConstraint < 0
                        && CheckEqualityConstraints(matrixAEquality, vectorBEquality, x)
                        && CheckInequalityConstraints(matrixAInequality, vectorBInequality, x))
                    {
                        var fx = Fx(problem, x);

                        if (fx < bestFxMin)
                        {
                            bestX = x;
                            bestFxMin = fx;
                        }
                    }
                }
            }

            if (bestX != null)
            {
                // Return the best X we have.
                return new QPSolution(bestX, maxIter);
            }

            throw new InvalidOperationException("Max iterations reached without convergence.");

        }

        private bool CheckInequalityConstraints(Matrix<double> matrixAInequality, Vector<double> vectorBInequality, Vector<double> x)
        {
            var tolerance = this.options.Tolerance;

            // check that Aineq * x <= bineq
            // or Aineq * x - bineq <= [0]

            var fineq = matrixAInequality * x - vectorBInequality;

            foreach (var fi in fineq)
            {
                if (fi > tolerance)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckEqualityConstraints(Matrix<double> matrixAEquality, Vector<double> vectorBEquality, Vector<double> x)
        {
            var l2Norm = (matrixAEquality * x - vectorBEquality).L2Norm();

            return l2Norm < this.options.Tolerance;
        }

        private static double Fx(
            QPProblemInternal problem,
            Vector<double> x)
        {
            var vectorCTransposed = problem.VectorCTransposed;
            var matrixQ = problem.QPProblem.MatrixQ;

            var fx = 1.0 / 2.0 * x.ToColumnMatrix().TransposeThisAndMultiply(matrixQ) * x + vectorCTransposed * x;

            return fx[0];
        }

        private static Matrix<double> BuildActiveMatrix(
            Matrix<double> matrixAEquality,
            Matrix<double> matrixAInequality,
            List<int> activeSet)
        {
            var list = new List<Vector<double>>();
            for (var i = 0; i < matrixAEquality.RowCount; i++)
            {
                list.Add(matrixAEquality.Row(i));
            }

            foreach (var c in activeSet)
            {
                list.Add(matrixAInequality.Row(c));
            }

            if (list.Count == 0)
            {
                return DenseMatrix.Build.Dense(0, matrixAEquality.ColumnCount);
            }
            else
            {
                return DenseMatrix.OfRowVectors(list);
            }
        }

        private static Vector<double> BuildActiveVector(
            Vector<double> vectorBEquality,
            Vector<double> vectorBInequality,
            List<int> activeSet)
        {
            var list = new List<double>();

            for (var i = 0; i < vectorBEquality.Count; i++)
            {
                list.Add(vectorBEquality[i]);
            }

            foreach (var c in activeSet)
            {
                list.Add(vectorBInequality[c]);
            }

            return DenseVector.OfEnumerable(list);
        }
    }
}
