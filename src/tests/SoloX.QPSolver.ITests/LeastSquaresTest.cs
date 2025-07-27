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

        private static readonly IEnumerable<IEnumerable<double>> MatrixA3Columns = [
                [0.007216290468957373   ,   -0.011083577336876722    , 0.00013617288906265092],
                [0.0055840790763347775  ,  0.03431689311881424        , 0.00013638742129018912],
                [-0.02165494847294276   ,    -0.0007592796959843442 , 0.00013630697545064375],
                [-0.006907213753249833  ,   0.01474252076624703        , 0.0001367092003151701],
                [-0.020383401821213745  ,   -0.03863315983033734      , 0.00013636059350597275],
                [-0.00900193720316729   ,    -0.017154250058479387    , 0.00013665557111742078],
                [0.03143190354995966    ,     0.02469539951023945        , 0.00013654831143672798],
                [0.005394734469880744   ,   0.01839890147142163        , 0.0001366823862337704],
                [0.005008215741800479   ,   0.002739359978038075     , 0.00013654829901977163],
                [-0.041416404742583415  ,   -0.042233123790100593    , 0.00013603880631485733],
                [0.033986427138253436   ,   0.03113406622291202        , 0.000136601941663432],
                [0.0026143636745229876  ,  0.007867121305130214     , 0.00011649362299615408],
                [0.013502489584632002   ,   0.0157330101313584           , 8.975472928563345E-05],
                [0.002740298369131519   ,   -0.004881215522412958    , 8.943218048107905E-05],
                [0.016195528812397095   ,   0.011825597508017687     , 8.970097426275604E-05],
                [0.012785418453693895   ,   0.017296535857663314     , 8.967409558391782E-05],
                [-0.0119303939152436    ,      -0.0265312984347096         , 6.984097606788499E-05],
                [-0.01908761097227514   ,    -0.008552516022746665    , 4.250028240060789E-05],
                [0.045560300851449506   ,   0.06014316347645153        , 4.241946651973162E-05],
                [-0.004893182811190291  ,   -0.014201481145742174    , 4.255417741838986E-05],
                [0.03055738096937881    ,     0.045742561344858754     , 4.303913215720069E-05],
                [-0.0038929953861740058 , -0.0014869308155305463 , 4.295830526313406E-05],
                [0.008773910139896188   ,   -0.005600881044618217    , 4.2688888596824305E-05],
                [-0.005124596894678327  ,   -0.015580417384111329    , 4.274277213312384E-05],
                [-0.01388600274789116   ,    -0.028302180501814673    , 2.286272258478629E-05],
                [0.0030412367651650313  ,  0.000549299573662437     , -4.893966858095277E-06],
                [0.0071366620870089575  ,  0.01671681912112664        , -4.623921435183186E-06],
                [-0.017654544024226018  ,   -0.025556774561597235    , -3.7867113268322695E-06],
                [0.026457267264396497   ,   0.03355397144793691        , -3.6786908946776376E-06],
                [0.002604526145582417   ,   -0.0008812672412947978 , -3.3546313656887972E-06],
                [0.0009817301749922985  ,  0.004413017892330711     , -3.381646520912761E-06],
                [-0.0007831631156679092 , -0.001121187682877184    , -2.455148167245934E-05],
                [0.006378500238653093   ,   -0.004633191401955079    , -5.161663135655058E-05],
                [-0.01419460985162753   ,    -0.024439603227162116    , -5.153541980817554E-05],
                [-0.005912785774916121  ,   -0.009766387809793348    , -5.1751990340127884E-05],
                [-0.058477419189317106  ,   -0.05581769147094463      , -5.2158055313107675E-05],
                [-0.02384959224305612   ,    -0.015754010756671617    , -7.221500492160114E-05],
                [0.011172527155716525   ,   0.013235642227382254     , -9.877520958345297E-05],
                [-0.012805678479182972  ,   -0.01019097404467319      , -9.86938052204248E-05],
                [-0.10751556773181643   ,    -0.10882739379255392      , -9.88023466653679E-05],
                [0.015961775779130757   ,   -0.002588018652526797    , -9.910083094332865E-05],
                [-0.0011013632218293684 , 0.02128377064923524        , -9.880234481129545E-05],
                [0.04036254382324993    ,     0.028430212538364846     , -0.00011906946891692837],
                [0.029481226925282142   ,   0.03104077312015539        , -0.00014686218190084897],
                [0.0037156876614840434  ,  0.015469759410886113     , -0.0001460189550911387],
                [0.045555184909569155   ,   0.036050436337018196     , -0.00014561096582543375],
                [-0.025919546472862723  ,   -0.024966450546206182    , -0.00014558376680106765],
                [0.011472633696812215   ,   0.006820908021703636     , -0.0001462909664785531],
                [0.005700966944001948   ,   0.011479531879838394     , -0.0001458285843470811],
                [-0.01349234872547507   ,    -0.016182428937091115    , -0.00016554475719476468],
                [-0.006390709135187647  ,   1.2459475291323914E-06 , -0.00019283019672943912],
                [0.014902205814441169   ,   0.012045172850489543     , -0.00019231218883930122],
            ];

        private static readonly IEnumerable<double> VectorB = [
                -0.012479483864734282,
                0.02391705994729089,
                0.003834855061313662,
                0.015691498843786858,
                -0.03172130546225925,
                -0.02465698597293607,
                0.02623386747950686,
                0.0010460150834077516,
                0.017321441284277497,
                -0.02462971018221448,
                0.010827654718494688,
                0.008539315445999856,
                0.026042124051750784,
                -0.003273232468059091,
                0.004257518443344004,
                0.016901372019643382,
                -0.02848379790379839,
                -0.009384432515796888,
                0.0489208131562482,
                -0.007787157910961058,
                0.031916646918571004,
                0.000748750768663002,
                0.002716682999918578,
                -0.01591457359044389,
                -0.03153767438485221,
                0.005727808550015883,
                0.002697711925041772,
                -0.012457124569214757,
                0.024554572575841607,
                -0.002564822453212086,
                0.009058467926551088,
                0.0076324187516236975,
                -0.011191682939556398,
                -0.014489484496221212,
                -0.032088014374208926,
                -0.04331500697372369,
                -0.021226254315122803,
                0.020016894879951744,
                -0.009770552302011832,
                -0.09752497068943154,
                -0.025297430217862224,
                0.028514972236221472,
                0.031644481356519996,
                0.03265956894068457,
                0.012008256809240574,
                0.04280578715166407,
                -0.018823450521798,
                0.00579167627263401,
                0.015739071861040413,
                -0.010860695001236082,
                0.0032840716926183663,
                0.008426465957788858,
                ];

        private const string XiFixedWithBounds = nameof(XiFixedWithBounds);

        private const string XiFixedWithEquality = nameof(XiFixedWithEquality);

        [Theory]
        [InlineData(XiFixedWithBounds)]
        [InlineData(XiFixedWithEquality)]
        public void ItShouldSolveLeastSquaresProblemOn3ColumnsMatrixAndOneXiFixed(string mode)
        {
            var services = new ServiceCollection();

            services.AddQuadraticProgrammingEngine();

            using var provider = services.BuildServiceProvider();

            var qpEngine = provider.GetRequiredService<IQPEngine>();

            var mA = Matrix<double>.Build.DenseOfRows(MatrixA3Columns);

            var b = Vector<double>.Build.DenseOfEnumerable(VectorB);

            var leastSquaresProblem = CreateLeastSquaresProblem(qpEngine, mA, b, mode);

            var solution = qpEngine.Solve(leastSquaresProblem);

            solution.Should().NotBeNull();

            var x = solution.VectorX;

            qpEngine.CheckBoundsConstraints(leastSquaresProblem, x).Should().BeTrue();
            qpEngine.CheckEqualityConstraints(leastSquaresProblem, x).Should().BeTrue();

            var vRes = mA * x - b;
            var leastSquaresResult = vRes.L2Norm();

            var x1 = Vector<double>.Build.DenseOfArray([
                    0.0182,
                    0.8952,
                    0.0866
                ]);

            var vRes1 = mA * x1 - b;
            var leastSquaresResult1 = vRes1.L2Norm();

            Math.Abs(leastSquaresResult - leastSquaresResult1).Should().BeLessThanOrEqualTo(1e-4);
        }

        private static IQPProblem CreateLeastSquaresProblem(IQPEngine qpEngine, Matrix<double> mA, Vector<double> b, string mode)
        {
            if (mode == XiFixedWithBounds)
            {
                var mAeq = Matrix<double>.Build.Dense(1, mA.ColumnCount, 1.0);
                var beq = Vector<double>.Build.DenseOfArray([1.0]);

                var lower = Vector<double>.Build.DenseOfArray([
                    0,
                    0,
                    0.08655991680229141
                ]);
                var upper = Vector<double>.Build.DenseOfArray([
                    1,
                    1,
                    0.08655991680229141
                ]);

                var leastSquaresProblem = qpEngine.CreateProblem(configure =>
                    configure
                        .WithEquality(mAeq, beq)
                        .WithLowerBounds(lower)
                        .WithUpperBounds(upper)
                        .MinimizingLeastSquares(mA, b));

                return leastSquaresProblem;
            }
            else if (mode == XiFixedWithEquality)
            {
                var mAeq = Matrix<double>.Build.DenseOfColumns([
                    [1.0, 0.0],
                    [1.0, 0.0],
                    [1.0, 1.0],
                ]);
                var beq = Vector<double>.Build.DenseOfArray([
                    1.0,
                    0.08655991680229141,
                ]);

                var lower = Vector<double>.Build.DenseOfArray([
                    0,
                    0,
                    0,
                ]);
                var upper = Vector<double>.Build.DenseOfArray([
                    1,
                    1,
                    1,
                ]);

                var leastSquaresProblem = qpEngine.CreateProblem(configure =>
                    configure
                        .WithEquality(mAeq, beq)
                        .WithLowerBounds(lower)
                        .WithUpperBounds(upper)
                        .MinimizingLeastSquares(mA, b));

                return leastSquaresProblem;
            }
            else
            {
                throw new NotImplementedException($"Unknown problem mode: {mode}");
            }
        }
    }
}
