// ----------------------------------------------------------------------
// <copyright file="QPEngineTest.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.Options;
using SoloX.QPSolver.Impl;

namespace SoloX.QPSolver.UTests
{
    public class QPEngineTest
    {
        [Fact]
        public void ItShouldSolveQPProblem()
        {
            var size = 6;
            var tolerance = 1e-6;

            var mQ = Matrix<double>.Build.RandomPositiveDefinite(size);
            var c = Vector<double>.Build.Random(size);

            var lower = Vector<double>.Build.Dense(size, 0.0);
            var upper = Vector<double>.Build.Dense(size, 10.0);

            var mAeq = Matrix<double>.Build.Dense(1, size, 1.0);
            var beq = Vector<double>.Build.Dense(1, 5.0);

            var mAineq = Matrix<double>.Build.Dense(1, size, 1.0);
            var bineq = Vector<double>.Build.Dense(1, 15.0);

            var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()
            {
                Tolerance = tolerance,
            }));

            var qpProblem = qpEngine.CreateProblem(configure =>
                configure
                    .WithInequality(mAineq, bineq)
                    .WithEquality(mAeq, beq)
                    .WithLowerBounds(lower)
                    .WithUpperBounds(upper)
                    .Minimizing(mQ, c)
            );

            var solution = qpEngine.Solve(qpProblem);

            var norm = (mAeq * solution.VectorX - beq).L2Norm();

            norm.Should().BeLessThan(tolerance);

            var fineq = mAineq * solution.VectorX - bineq;

            foreach (var fi in fineq)
            {
                fi.Should().BeLessThan(tolerance);
            }

            for (var i = 0; i < size; i++)
            {
                var xi = solution.VectorX[i];
                xi.Should().BeLessThanOrEqualTo(upper[i]);
                xi.Should().BeGreaterThanOrEqualTo(lower[i]);
            }
        }

        [Fact]
        public void ItShouldResolveAxMinusBEqualsZero()
        {
            var mQ = Matrix<double>.Build.Dense(5, 5, 0.0);
            var c = Vector<double>.Build.Dense(5, 0.0);

            var mA = Matrix<double>.Build.DenseOfRows([
                [ 0.20, 0.30, 0.00, 0.05, 0.45 ],
                [ 0.20, 0.20, 0.25, 0.50, 0.30 ],
                [ 0.20, 0.20, 0.30, 0.25, 0.25 ],
                [ 0.20, 0.60, 0.65, 0.40, 0.10 ],
            ]);

            var b = Vector<double>.Build.DenseOfArray([0.05, 0.15, 0.25, 0.50]);

            var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

            var qpProblem = qpEngine.CreateProblem(configure =>
                configure
                    .WithEquality(mA, b)
                    .Minimizing(mQ, c)
            );

            var solution = qpEngine.Solve(qpProblem);

            var x = solution.VectorX;

            var norm = (mA * x - b).L2Norm();

            norm.Should().BeLessThan(1e-6);

            solution.IterationCount.Should().Be(1);
        }

        [Fact]
        public void ItShouldSolveSimpleQP()
        {
            // Minimize 1/2 * xT * Q * x + cT * x
            // with xT = x transposed.
            // and cT = c transposed.

            var mQ = Matrix<double>.Build.DenseOfRows([
                [ 2, 0 ],
                [ 0, 2 ]
            ]);
            var c = Vector<double>.Build.Dense([-4.0, -6.0]);

            var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

            var qpProblem = qpEngine.CreateProblem(configure =>
                configure.Minimizing(mQ, c)
            );

            var solution = qpEngine.Solve(qpProblem);

            var x = solution.VectorX;

            var expected = Vector<double>.Build.Dense([2.0, 3.0]);
            (x - expected).L2Norm().Should().BeLessThan(1e-6);

            solution.IterationCount.Should().Be(2);

            var fx = qpEngine.Fx(qpProblem, x);

            var xOther = Vector<double>.Build.Dense([3.0, 10.0]);

            var fxOther = qpEngine.Fx(qpProblem, xOther);

            fxOther.Should().BeGreaterThan(fx);
        }

        [Theory]
        [InlineData(4, 1000)]
        [InlineData(10, 100)]
        [InlineData(40, 10)]
        [InlineData(60, 10)]
        public void ItShouldMinimizeQcFunctionWithBounds(int size, int testCount)
        {
            var minimumIssue = 0;
            var invalidIssue = 0;

            for (var testIter = 0; testIter < testCount; testIter++)
            {

                // Minimize 1/2 * xT * Q * x + cT * x
                // with xT = x transposed.
                // and cT = c transposed.

                var mQ = Matrix<double>.Build.RandomPositiveDefinite(size);
                var c = Vector<double>.Build.Random(size);

                var lower = Vector<double>.Build.Dense(size, -10.0);
                var upper = Vector<double>.Build.Dense(size, 10.0);

                try
                {
                    var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

                    var qpProblem = qpEngine.CreateProblem(configure =>
                        configure
                            .WithLowerBounds(lower)
                            .WithUpperBounds(upper)
                            .Minimizing(mQ, c)
                    );

                    var solution = qpEngine.Solve(qpProblem);

                    var x = solution.VectorX;

                    // Make sure lower[i] <= x(i) <= upper[i] 
                    for (var i = 0; i < x.Count; i++)
                    {
                        x[i].Should().BeGreaterThanOrEqualTo(lower[i])
                            .And.BeLessThanOrEqualTo(upper[i]);
                    }

                    var fx = qpEngine.Fx(qpProblem, x);

                    var x1 = Vector<double>.Build.Dense(x.Count);
                    var tryCount = 10000;

                    var alreadySet = false;

                    for (var nbTry = 0; nbTry < tryCount; nbTry++)
                    {
                        // Compute x1 has different of x.
                        for (var i = 0; i < x.Count; i++)
                        {
                            do
                            {
#pragma warning disable CA5394 // Do not use insecure randomness
                                x1[i] = Random.Shared.Next((int)lower[i] * 100, (int)upper[i] * 100) / 100.0;
#pragma warning restore CA5394 // Do not use insecure randomness
                            }
                            while (x[i] == x1[i]);
                        }

                        // Fx1 must be greater that Fx.
                        var fx1 = qpEngine.Fx(qpProblem, x1);

                        if (fx1 < fx && !alreadySet)
                        {
                            alreadySet = true;
                            minimumIssue++;
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    invalidIssue++;
                }


            }

            invalidIssue.Should().Be(0);
            minimumIssue.Should().Be(0);
        }

        [Theory]
        [InlineData(4, 1000)]
        [InlineData(10, 100)]
        [InlineData(40, 10)]
        [InlineData(60, 10)]
        public void ItShouldMinimizeQcFunctionWithEquality(int size, int testCount)
        {
            var minimumIssue = 0;
            var invalidIssue = 0;

            for (var testIter = 0; testIter < testCount; testIter++)
            {

                // Minimize 1/2 * xT * Q * x + cT * x
                // with xT = x transposed.
                // and cT = c transposed.

                var mQ = Matrix<double>.Build.RandomPositiveDefinite(size);
                var c = Vector<double>.Build.Random(size);

                var mAeq = Matrix<double>.Build.Dense(1, size, 1.0);
                var beq = Vector<double>.Build.Dense(1, 1.0);

                try
                {
                    var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

                    var qpProblem = qpEngine.CreateProblem(configure =>
                        configure
                            .WithEquality(mAeq, beq)
                            .Minimizing(mQ, c)
                    );

                    var solution = qpEngine.Solve(qpProblem);

                    var x = solution.VectorX;

                    // Make sure Equality Aeq * x = beq
                    (mAeq * x - beq).L2Norm().Should().BeLessThan(1e-6);

                    var fx = qpEngine.Fx(qpProblem, x);

                    var x1 = Vector<double>.Build.Dense(x.Count);
                    var tryCount = 10000;

                    var alreadySet = false;

                    for (var nbTry = 0; nbTry < tryCount; nbTry++)
                    {
                        do
                        {
                            // Compute x1 has different of x and sum equals one.
                            for (var i = 0; i < x.Count; i++)
                            {
#pragma warning disable CA5394 // Do not use insecure randomness
                                var v = Random.Shared.Next(-1000, 1000) / 100.0;
#pragma warning restore CA5394 // Do not use insecure randomness
                                x1[i] = v;
                            }

                            var toOne = 1.0 / x1.Sum();
                            x1 = x1 * toOne;
                        } while (x[0] == x1[0]);

                        // Fx1 must be greater that Fx.
                        var fx1 = qpEngine.Fx(qpProblem, x1);

                        if (fx1 < fx && !alreadySet)
                        {
                            alreadySet = true;
                            minimumIssue++;
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    invalidIssue++;
                }


            }

            invalidIssue.Should().Be(0);
            minimumIssue.Should().Be(0);
        }

        [Theory]
        [InlineData(4, 1000)]
        [InlineData(10, 100)]
        [InlineData(40, 10)]
        [InlineData(60, 10)]
        public void ItShouldMinimizeQcFunctionWithInequality(int size, int testCount)
        {
            var minimumIssue = 0;
            var invalidIssue = 0;

            for (var testIter = 0; testIter < testCount; testIter++)
            {

                // Minimize 1/2 * xT * Q * x + cT * x
                // with xT = x transposed.
                // and cT = c transposed.

                var mQ = Matrix<double>.Build.RandomPositiveDefinite(size);
                var c = Vector<double>.Build.Random(size);

                var mAineq = Matrix<double>.Build.Dense(1, size, 1.0);
                var bineq = Vector<double>.Build.Dense(1, 1.0);

                try
                {
                    var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

                    var qpProblem = qpEngine.CreateProblem(configure =>
                        configure
                            .WithInequality(mAineq, bineq)
                            .Minimizing(mQ, c)
                    );

                    var solution = qpEngine.Solve(qpProblem);

                    var x = solution.VectorX;

                    // Make sure Equality Aeq * x = beq
                    foreach (var v in (mAineq * x - bineq))
                    {
                        v.Should().BeLessThan(1e-6);
                    }

                    var fx = qpEngine.Fx(qpProblem, x);

                    var x1 = Vector<double>.Build.Dense(x.Count);
                    var tryCount = 10000;

                    var alreadySet = false;

                    for (var nbTry = 0; nbTry < tryCount; nbTry++)
                    {
                        // Compute x1 has different of x and sum lower than one.
                        do
                        {
#pragma warning disable CA5394 // Do not use insecure randomness
                            // Compute x1 has different of x and sum equals one.
                            for (var i = 0; i < x.Count; i++)
                            {
                                var v = Random.Shared.Next(-1000, 1000) / 100.0;
                                x1[i] = v;
                            }

                            var toOne = (Random.Shared.Next(-100000000, 100) / 100.0) / x1.Sum();
                            x1 = x1 * toOne;
#pragma warning restore CA5394 // Do not use insecure randomness
                        } while (x[0] == x1[0]);

                        // Fx1 must be greater that Fx.
                        var fx1 = qpEngine.Fx(qpProblem, x1);

                        if (fx1 < fx && !alreadySet)
                        {
                            alreadySet = true;
                            minimumIssue++;
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    invalidIssue++;
                }
            }

            invalidIssue.Should().Be(0);
            minimumIssue.Should().Be(0);
        }

        [Fact]
        public void ItShouldHandleInequalityConstraints()
        {
            // Minimize 1/2 * xT * Q * x + cT * x
            // with xT = x transposed.
            // and cT = c transposed.

            var mQ = Matrix<double>.Build.DenseIdentity(2);
            var c = Vector<double>.Build.Dense(2);

            var mAineq = Matrix<double>.Build.DenseOfRows([
                [1.0, 1.0]
            ]);
            var bineq = Vector<double>.Build.Dense([1.0]);

            var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

            var qpProblem = qpEngine.CreateProblem(configure =>
                configure
                    .WithInequality(mAineq, bineq)
                    .Minimizing(mQ, c)
            );

            var solution = qpEngine.Solve(qpProblem);

            var x = solution.VectorX;

            (x[0] + x[1]).Should().BeLessThan(1.0 + 1e-6);
        }

        [Fact]
        public void ItShouldHandleEqualityConstraints()
        {
            // Minimize 1/2 * xT * Q * x + cT * x
            // with xT = x transposed.
            // and cT = c transposed.

            var mQ = Matrix<double>.Build.DenseIdentity(2);
            var c = Vector<double>.Build.Dense(2);

            var mAeq = Matrix<double>.Build.DenseOfRows([
                [1.0, 1.0]
            ]);
            var beq = Vector<double>.Build.Dense([1.0]);

            var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

            var qpProblem = qpEngine.CreateProblem(configure =>
                configure
                    .WithEquality(mAeq, beq)
                    .Minimizing(mQ, c)
            );

            var solution = qpEngine.Solve(qpProblem);

            var x = solution.VectorX;

            (x[0] + x[1] - 1.0).Should().BeLessThan(1e-6);
        }

        [Fact]
        public void ItShouldThrowsOnInfeasibleSystem()
        {
            var mAeq = Matrix<double>.Build.DenseOfRows([
                [1, 0],
                [0, 1]
            ]);
            var beq = Vector<double>.Build.Dense([1.0, 1.0]);

            // Infeasible: x = 1, but lower bound is 2
            var lower = Vector<double>.Build.Dense([2.0, 2.0]);

            var mQ = Matrix<double>.Build.DenseIdentity(2);
            var c = Vector<double>.Build.Dense(2);

            var qpEngine = new QPEngine(Options.Create(new QPEngineOptions()));

            var qpProblem = qpEngine.CreateProblem(configure =>
                configure
                    .WithEquality(mAeq, beq)
                    .WithLowerBounds(lower)
                    .Minimizing(mQ, c)
            );

            Assert.Throws<InvalidOperationException>(() =>
                qpEngine.Solve(qpProblem)
            );
        }
    }
}
