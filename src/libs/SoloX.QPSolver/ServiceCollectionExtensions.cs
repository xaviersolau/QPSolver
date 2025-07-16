// ----------------------------------------------------------------------
// <copyright file="ServiceCollectionExtensions.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using Microsoft.Extensions.DependencyInjection;
using SoloX.QPSolver.Impl;

namespace SoloX.QPSolver
{
    /// <summary>
    /// Extension methods to setup the Quadratic Programming Engine
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add Quadratic Programming Engine into the service collection.
        /// </summary>
        /// <param name="services">The service collection to initialize.</param>
        /// <returns>The given services.</returns>
        public static IServiceCollection AddQuadraticProgrammingEngine(this IServiceCollection services)
            => AddQuadraticProgrammingEngine(services, options => { });

        /// <summary>
        /// Add Quadratic Programming Engine into the service collection.
        /// </summary>
        /// <param name="services">The service collection to initialize.</param>
        /// <param name="configure">Configuration handler.</param>
        /// <returns>The given services.</returns>
        public static IServiceCollection AddQuadraticProgrammingEngine(
            this IServiceCollection services,
            Action<QPEngineOptions> configure)
        {
            services.AddSingleton<IQPEngine, QPEngine>();

            services.Configure<QPEngineOptions>(configure);

            return services;
        }
    }
}
