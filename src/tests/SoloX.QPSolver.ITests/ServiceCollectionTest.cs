// ----------------------------------------------------------------------
// <copyright file="ServiceCollectionTest.cs" company="Xavier Solau">
// Copyright © 2025 Xavier Solau.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>
// ----------------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace SoloX.QPSolver.ITests
{
    public class ServiceCollectionTest
    {
        [Fact]
        public void ItShouldBeAvailableThroughDependencyInjection()
        {
            var services = new ServiceCollection();

            services.AddQuadraticProgrammingEngine();

            using var provider = services.BuildServiceProvider();

            var qpEngine = provider.GetRequiredService<IQPEngine>();

            qpEngine.Should().NotBeNull();
        }
    }
}
