// ----------------------------------------------------------------------
// <copyright file="BoundsClampingTest.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------


using FluentAssertions;
using MathNet.Numerics.LinearAlgebra;
using SoloX.QPSolver.Impl;

namespace SoloX.QPSolver.UTests
{
    public class BoundsClampingTest
    {
        [Fact]
        public void IsShouldClampVectorBetweenUpperLowerBoundsGivenTheTolerance()
        {
            var lowerBounds = Vector<double>.Build.Dense(2, 0.0);
            var upperBounds = Vector<double>.Build.Dense(2, 1.0);
            var tolerance = 0.01;
            var boundsClamping = new BoundsClamping(lowerBounds, upperBounds, tolerance);

            var x = Vector<double>.Build.DenseOfArray([-0.001, 1.001]);

            boundsClamping.ApplyTo(x);

            x[0].Should().Be(0.0);
            x[1].Should().Be(1.0);
        }

        [Fact]
        public void IsShouldClampVectorOnLowerBoundsGivenTheTolerance()
        {
            var lowerBounds = Vector<double>.Build.Dense(2, 0.0);
            var tolerance = 0.01;
            var boundsClamping = new BoundsClamping(lowerBounds, null, tolerance);

            var x = Vector<double>.Build.DenseOfArray([-0.001, 1.001]);

            boundsClamping.ApplyTo(x);

            x[0].Should().Be(0.0);
            x[1].Should().Be(1.001);
        }

        [Fact]
        public void IsShouldClampVectorOnUpperBoundsGivenTheTolerance()
        {
            var upperBounds = Vector<double>.Build.Dense(2, 1.0);
            var tolerance = 0.01;
            var boundsClamping = new BoundsClamping(null, upperBounds, tolerance);

            var x = Vector<double>.Build.DenseOfArray([-0.001, 1.001]);

            boundsClamping.ApplyTo(x);

            x[0].Should().Be(-0.001);
            x[1].Should().Be(1.0);
        }
    }
}
