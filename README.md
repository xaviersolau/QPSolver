# QPSolver

This project provides an optimization solver using Quadratic Programing (QP). It is using
[Math.NET Numerics](https://numerics.mathdotnet.com/) for all matrix computation.

Don't hesitate to post issues, pull requests on the project or to fork and improve the project.

## Project dashboard

[![Build - CI](https://github.com/xaviersolau/QPSolver/actions/workflows/build-ci.yml/badge.svg)](https://github.com/xaviersolau/QPSolver/actions/workflows/build-ci.yml)
[![Coverage Status](https://coveralls.io/repos/github/xaviersolau/QPSolver/badge.svg?branch=main)](https://coveralls.io/github/xaviersolau/QPSolver?branch=main)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

| Package                                    | Nuget.org | Pre-release |
|--------------------------------------------|-----------|-----------|
|**SoloX.QPSolver**            |[![NuGet Beta](https://img.shields.io/nuget/v/SoloX.QPSolver.svg)](https://www.nuget.org/packages/SoloX.QPSolver)|[![NuGet Beta](https://img.shields.io/nuget/vpre/SoloX.QPSolver.svg)](https://www.nuget.org/packages/SoloX.QPSolver)|

## License and credits

QPSolver project is written by Xavier Solau. It's licensed under the MIT license.

 * * *

## Features

* Quadratic programming optimization solver.
* Constrained least squares problem solver (QP).

## Formal Definition

### Quadratic Programming

Quadratic Programming is a type of mathematical optimization problem characterized by a quadratic objective function and linear constraints.
It's widely used in fields like finance, control systems, machine learning (e.g., SVMs), and operations research.

Standard Form:
Minimize:

_f(x)=1/2 x ^T^ Q x + 𝑐 ^T^ x_

Subject to:

* _Aeq_ = _beq_
* _Aineq_ _x_ ≤ _bineq_
* _lower_ ≤ _x_ ≤ _upper_

Where:

* _x_ ∈ 𝑅^𝑛^ is the variable vector.
* _Q_ ∈ 𝑅^𝑛×𝑛^ is symmetric and (usually) positive semi-definite.
* _c_ ∈ 𝑅^𝑛^
* _Aeq_, _beq_ are equality constraints.
* _Aineq_, _bineq_ are inequality constraints.
* _lower_, _upper_ are lower and upper bounds (box constraints).

## Least-Squares problem

Least-Squares problem minimize:

_|| A _x_ - b ||²_

or

_(A _x_ - b)^T^(A _x_ - b)_

This is equivalent to a QP problem minimizing:

_f(x)=1/2 x ^T^ Q x + 𝑐 ^T^ x_

With:

* _Q = 2 A^T^ A_
* _c = -2 A^T^ b_


## Installation

You can checkout this Github repository or you can use the NuGet packages:

**Install using the command line from the Package Manager:**
```bash
Install-Package SoloX.QPSolver -version 1.0.0-alpha.2
```

**Install using the .Net CLI:**
```bash
dotnet add package SoloX.QPSolver --version 1.0.0-alpha.2
```

**Install editing your project file (csproj):**
```xml
<PackageReference Include="SoloX.QPSolver" Version="1.0.0-alpha.2" />
```

## How to use it

Note that you can find code examples in this repository at this location: `src/examples`.

### Set up the dependency injection

A few lines of code are actually needed to setup the QPSolver services. You just need to use the
name space SoloX.QPSolver to get access to the right extension methods and to add the services in
your ServiceCollection :

```csharp
// Add QPSolver services.
builder.Services.AddQuadraticProgrammingEngine();
```

Once it is added, you can inject and use the IQPEngine implementation.

### Configure your problem

You can use the injected IQPEngine to create the problem:

```csharp
var mQ = Matrix<double>.Build.DenseOfRows([
    [ 2, 0 ],
    [ 0, 2 ]
]);

var c = Vector<double>.Build.Dense([
    -4.0,
    -6.0
]);

var qpProblem = qpEngine.CreateProblem(configure =>
    configure.Minimizing(mQ, c)
);
```

The full QP problem configuration can look like this:

```csharp
var qpProblem = qpEngine.CreateProblem(configure =>
    configure
        .WithEquality(mAeq, beq)
        .WithInequality(mAineq, bineq)
        .WithLowerBounds(lower)
        .WithUpperBounds(upper)
        .Minimizing(mQ, c)
);
```

And the full Least-Squares QP problem configuration can look like this:

```csharp
var qpProblem = qpEngine.CreateProblem(configure =>
    configure
        .WithEquality(mAeq, beq)
        .WithInequality(mAineq, bineq)
        .WithLowerBounds(lower)
        .WithUpperBounds(upper)
        .MinimizingLeastSquares(mA, b)
);
```

### Solve the problem

You can use the injected IQPEngine to solve your problem:

```csharp
var solution = qpEngine.Solve(qpProblem);
```
