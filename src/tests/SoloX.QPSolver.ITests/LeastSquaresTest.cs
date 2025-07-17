// ----------------------------------------------------------------------
// <copyright file="LeastSquaresTest.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.DependencyInjection;

namespace SoloX.QPSolver.ITests
{
    public class LeastSquaresTest
    {
        [Fact]
        public void ItShouldSolveLeastSquaresProblem()
        {
            var services = new ServiceCollection();

            services.AddQuadraticProgrammingEngine();

            using var provider = services.BuildServiceProvider();

            var qpEngine = provider.GetRequiredService<IQPEngine>();

            var mA = Matrix<double>.Build.DenseOfRows([
                [0.20, 0.30, 0.00, 0.05, 0.45],
                [0.20, 0.20, 0.25, 0.50, 0.30],
                [0.20, 0.20, 0.30, 0.25, 0.25],
                [0.20, 0.60, 0.65, 0.40, 0.10],
            ]);

            var b = Vector<double>.Build.DenseOfArray([0.05, 0.15, 0.25, 0.50]);

            var mAeq = Matrix<double>.Build.Dense(1, mA.ColumnCount, 1.0);
            var beq = Vector<double>.Build.DenseOfArray([1.0]);

            var lower = Vector<double>.Build.Dense(mA.ColumnCount, 0.0);
            var upper = Vector<double>.Build.Dense(mA.ColumnCount, 1.0);

            var leastSquaresProblem = qpEngine.CreateProblem(configure =>
                configure
                    .WithEquality(mAeq, beq)
                    .WithLowerBounds(lower)
                    .WithUpperBounds(upper)
                    .MinimizingLeastSquares(mA, b));

            var solution = qpEngine.Solve(leastSquaresProblem);

            solution.Should().NotBeNull();

            var x = solution.VectorX;

            qpEngine.CheckBoundsConstraints(leastSquaresProblem, x).Should().BeTrue();
            qpEngine.CheckEqualityConstraints(leastSquaresProblem, x).Should().BeTrue();

            var vRes = mA * x - b;
            var leastSquaresResult = vRes.L2Norm();

            var x1 = Vector<double>.Build.Dense(x.Count);

            var tryCount = 10000;
            var minimumIssue = 0;

            for (var nbTry = 0; nbTry < tryCount; nbTry++)
            {
                // Compute x1 has different of x.
                for (var i = 0; i < x.Count; i++)
                {
                    do
                    {
#pragma warning disable CA5394 // Do not use insecure randomness
                        x1[i] = Random.Shared.Next(0, 1000) / 1000.0;
#pragma warning restore CA5394 // Do not use insecure randomness
                    }
                    while (x[i] == x1[i]);
                }

                // Make sure sum is one
                var toOne = 1.0 / x1.Sum();
                x1 = x1 * toOne;

                var vRes1 = mA * x1 - b;
                var leastSquaresResult1 = vRes1.L2Norm();

                if (leastSquaresResult1 < leastSquaresResult)
                {
                    minimumIssue++;
                }

            }
            minimumIssue.Should().Be(0);
        }
    }
}
