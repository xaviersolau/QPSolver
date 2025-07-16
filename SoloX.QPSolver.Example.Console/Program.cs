using MathNet.Numerics.LinearAlgebra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SoloX.QPSolver;
using SoloX.QPSolver.Impl;

var services = new ServiceCollection();

services.AddQuadraticProgrammingEngine();

using var serviceProvider = services.BuildServiceProvider();

var A = Matrix<double>.Build.DenseOfArray(new double[,] {
            { 0.20, 0.30, 0.00, 0.05, 0.45 },
            { 0.20, 0.20, 0.25, 0.50, 0.30 },
            { 0.20, 0.20, 0.30, 0.25, 0.25 },
            { 0.20, 0.60, 0.65, 0.40, 0.10 },
        });
var b = Vector<double>.Build.DenseOfArray(new double[] { 0.05, 0.15, 0.25, 0.50, });


var E = Matrix<double>.Build.Dense(1, A.ColumnCount, 1.0);
var f = Vector<double>.Build.DenseOfArray(new double[] { 1 });


var lower = Vector<double>.Build.Dense(A.ColumnCount, 0.0);
var upper = Vector<double>.Build.Dense(A.ColumnCount, 1.0);

var qpEngine = serviceProvider.GetRequiredService<IQPEngine>();

var qpProblem = qpEngine.CreateProblem(configure =>
    configure
        .WithEquality(E, f)
        .WithLowerBounds(lower)
        .WithUpperBounds(upper)
        .MinimizingLeastSquares(A, b)
    );

var solution = qpEngine.Solve(qpProblem);

var x = solution.VectorX;

Console.WriteLine("Solution x = " + x);

Console.WriteLine("Solution Ax = " + A * x);

Console.WriteLine("Solution b = " + b);

Console.WriteLine("Eq E x = " + E * x);

Console.WriteLine("Eq f = " + f);
