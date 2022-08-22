using System;
using NFluent;
using NFluent.Extensibility;
using NFluent.Kernel;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    public static class NFluentHelper
    {
        public static ICheckLink<ICheck<Vector3F>> Check(this ICheck<Vector3F> check, (float x, float y, float z) expected)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Vector3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (runCheck.Value != new Vector3F(expected.x, expected.y, expected.z))
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }

        public static ICheckLink<ICheck<Vector3F>> IsCloseTo(this ICheck<Vector3F> check, (float x, float y, float z) expected, float within=1e-8f)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Vector3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (MathF.Abs(runCheck.Value.X - expected.x) >  within
                || MathF.Abs(runCheck.Value.Y - expected.y) >  within
                || MathF.Abs(runCheck.Value.Z - expected.z) >  within)
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }
        
        public static ICheckLink<ICheck<Vector3F>> Check(this ICheck<Vector3F> check, (int x, int y, int z) expected)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Vector3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (runCheck.Value != new Vector3F(expected.x, expected.y, expected.z))
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }
        
        public static ICheckLink<ICheck<Point3F>> Check(this ICheck<Point3F> check, (float x, float y, float z) expected)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Point3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (runCheck.Value != new Point3F(expected.x, expected.y, expected.z))
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }
        public static ICheckLink<ICheck<Point3F>> Check(this ICheck<Point3F> check, (int x, int y, int z) expected)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Point3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (runCheck.Value != new Point3F(expected.x, expected.y, expected.z))
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }
        
        public static ICheckLink<ICheck<Normal3F>> Check(this ICheck<Normal3F> check, (float x, float y, float z) expected)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Normal3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (runCheck.Value != new Normal3F(expected.x, expected.y, expected.z))
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }
        
        public static ICheckLink<ICheck<Normal3F>> Check(this ICheck<Normal3F> check, (int x, int y, int z) expected)
        {
            var runCheck = ExtensibilityHelper.ExtractChecker<Normal3F>(check); 
            return runCheck.ExecuteCheck(() =>
            {
                if (runCheck.Value != new Normal3F(expected.x, expected.y, expected.z))
                {
                    throw new FluentCheckException($"{runCheck.Value} != {expected}");
                }
            }, "failed");
        }
    }
}