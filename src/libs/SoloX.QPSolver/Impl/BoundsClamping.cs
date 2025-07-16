// ----------------------------------------------------------------------
// <copyright file="BoundsClamping.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;

namespace SoloX.QPSolver.Impl
{
    internal sealed class BoundsClamping
    {
        private readonly Vector<double>? lowerBounds;
        private readonly Vector<double>? upperBounds;
        private readonly double tolerance;

        internal BoundsClamping(Vector<double>? lowerBounds, Vector<double>? upperBounds, double tolerance)
        {
            this.lowerBounds = lowerBounds;
            this.upperBounds = upperBounds;
            this.tolerance = tolerance;
        }

        public void ApplyTo(Vector<double> x)
        {
            if (this.lowerBounds != null && this.upperBounds != null)
            {
                LowerUpperClamping(x);
            }
            else if (this.lowerBounds != null)
            {
                LowerClamping(x);
            }
            else if (this.upperBounds != null)
            {
                UpperClamping(x);
            }
        }

        private void LowerUpperClamping(Vector<double> x)
        {
            var lowerBounds = this.lowerBounds!;
            var upperBounds = this.upperBounds!;

            for (var i = 0; i < x.Count; i++)
            {
                var x_i = x[i];
                var lowerBounds_i = lowerBounds[i];
                var upperBounds_i = upperBounds[i];

                if (x_i < lowerBounds_i)
                {
                    if (lowerBounds_i - x_i > this.tolerance)
                    {
                        throw new InvalidOperationException("Couldn't match lower bounds.");
                    }

                    x[i] = lowerBounds_i;
                }

                if (x_i > upperBounds_i)
                {
                    if (x_i - upperBounds_i > this.tolerance)
                    {
                        throw new InvalidOperationException("Couldn't match upper bounds.");
                    }
                    x[i] = upperBounds_i;
                }
            }
        }

        private void LowerClamping(Vector<double> x)
        {
            var lowerBounds = this.lowerBounds!;

            for (var i = 0; i < x.Count; i++)
            {
                var x_i = x[i];
                var lowerBounds_i = lowerBounds[i];

                if (x_i < lowerBounds_i)
                {
                    if (lowerBounds_i - x_i > this.tolerance)
                    {
                        throw new InvalidOperationException("Couldn't match lower bounds.");
                    }

                    x[i] = lowerBounds_i;
                }
            }
        }
        private void UpperClamping(Vector<double> x)
        {
            var upperBounds = this.upperBounds!;

            for (var i = 0; i < x.Count; i++)
            {
                var x_i = x[i];
                var upperBounds_i = upperBounds[i];

                if (x_i > upperBounds_i)
                {
                    if (x_i - upperBounds_i > this.tolerance)
                    {
                        throw new InvalidOperationException("Couldn't match upper bounds.");
                    }
                    x[i] = upperBounds_i;
                }
            }
        }
    }
}
