using System;
using System.Collections.Generic;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class PartitionUtilsTests
    {
        [Test]
        public void StdPartitionTest()
        {
            List<int> values = new List<int> { 9, 7, 5, 3, 1, 8, 6, 4, 2, 0 };
            int m = values.StdPartition(0, values.Count, i => i % 2 == 0);

            Check.That(m).IsEqualTo(5);
            Check.That(values).ContainsExactly(8, 6, 4, 2, 0, 9, 7, 5, 3, 1);
        }

        [Test]
        public void StdPartition_2_Test()
        {
            List<int> values = new List<int> { 9, 7, 5, 3, 1, 8, 6, 4, 2, 0 };
            int m = values.StdPartition(0, values.Count, i => i < 3);

            Check.That(m).IsEqualTo(3);
            Check.That(values).ContainsExactly(1, 2, 0, 3, 9, 8, 6, 4, 7, 5);
        }

        [Test]
        public void StdPartition_3_Test()
        {
            List<int> values = new List<int> { -5, -3, 9, 7, 5, 3, 1, 8, 6, 4, 2, 0 };
            int m = values.StdPartition(0, values.Count, i => i < 3);

            Check.That(m).IsEqualTo(5);
            Check.That(values).ContainsExactly(-5, -3, 1, 2, 0, 3, 9, 8, 6, 4, 7, 5);
        }

        [Test]
        public void StdPartition_4_Test()
        {
            List<int> values = new List<int> { 5, 3, 9, 7, 5, 3 };
            int m = values.StdPartition(0, values.Count, i => i < 3);

            Check.That(m).IsEqualTo(0);
            Check.That(values).ContainsExactly(5, 3, 9, 7, 5, 3);
        }

        [Test]
        public void StdPartition_5_Test()
        {
            List<int> values = new List<int> { 7, 8, 9, -1, 0, 1, 2, 3, 4, 5, 6 };
            int m = values.StdPartition(3, 3, null);

            Check.That(m).IsEqualTo(3);
            Check.That(values).ContainsExactly(7, 8, 9, -1, 0, 1, 2, 3, 4, 5, 6);
        }

        [Test]
        public void StdPartition_6_Test()
        {
            List<int> values = new List<int> { 7, 8, 9, -1, 0, 1, 2, 3, 4, 5, 6 };
            int m = values.StdPartition(2, 4, i => i < 3);

            Check.That(m).IsEqualTo(3);
            Check.That(values).ContainsExactly(7, 8, -1, 9, 0, 1, 2, 3, 4, 5, 6);
        }
    }
}