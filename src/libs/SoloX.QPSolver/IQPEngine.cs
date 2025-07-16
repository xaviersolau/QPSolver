// ----------------------------------------------------------------------
// <copyright file="IQPEngine.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using MathNet.Numerics.LinearAlgebra;
using SoloX.QPSolver.Exceptions;

namespace SoloX.QPSolver
{
    /// <summary>
    /// Quadratic Programming Engine interface.
    /// </summary>
    public interface IQPEngine
    {
        /// <summary>
        /// Create and configure a Quadratic Programming problem.
        /// </summary>
        /// <param name="configure">Configuration handler.</param>
        /// <returns>The configured problem.</returns>
        /// <exception cref="QPEngineException"></exception>
        IQPProblem CreateProblem(Action<IQPProblemConfiguration> configure);

        /// <summary>
        /// Solve the given Quadratic Programming Problem.
        /// </summary>
        /// <param name="quadraticProgrammingProblem">The problem to solve.</param>
        /// <returns>The Quadratic Programming Problem solution.</returns>
        QPSolution Solve(IQPProblem quadraticProgrammingProblem);

        /// <summary>
        /// Compute F(x) = 1/2 * xT * Q * x + cT * x with Q c from the given Quadratic Programming Problem definition.
        /// </summary>
        /// <param name="quadraticProgrammingProblem">The problem to get the Q and c parameters from.</param>
        /// <param name="x">The variable x to process.</param>
        /// <returns>F(x) result.</returns>
        double Fx(IQPProblem quadraticProgrammingProblem, Vector<double> x);

        /// <summary>
        /// Check inequality constraints of x and the given programing problem.
        /// </summary>
        /// <param name="quadraticProgrammingProblem">The problem to get the constraints from.</param>
        /// <param name="x">The variable x to process.</param>
        /// <returns>True if the constraints are satisfied.</returns>
        bool CheckInequalityConstraints(IQPProblem quadraticProgrammingProblem, Vector<double> x);

        /// <summary>
        /// Check equality constraints of x and the given programing problem.
        /// </summary>
        /// <param name="quadraticProgrammingProblem">The problem to get the constraints from.</param>
        /// <param name="x">The variable x to process.</param>
        /// <returns>True if the constraints are satisfied.</returns>
        bool CheckEqualityConstraints(IQPProblem quadraticProgrammingProblem, Vector<double> x);

        /// <summary>
        /// Check bounds constraints of x and the given programing problem.
        /// </summary>
        /// <param name="quadraticProgrammingProblem">The problem to get the bounds from.</param>
        /// <param name="x">The variable x to process.</param>
        /// <returns>True if the bounds constraints are satisfied.</returns>
        bool CheckBoundsConstraints(IQPProblem quadraticProgrammingProblem, Vector<double> x);
    }
}
