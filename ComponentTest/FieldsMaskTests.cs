using System.Collections.Generic;
using AkkaPlayground.Proto.Data;
using Google.Protobuf.WellKnownTypes;
using NUnit.Framework;

namespace ComponentTest
{
    public class FieldMaskTests
    {
        [Test]
        public void FieldMaskCreatesOne()
        {
            var fields1 = new FieldsMask(new List<Fields>{ Fields.Status });
            Assert.AreEqual(1, fields1.Mask[0]);
        }
        [Test]
        public void FieldMaskCreatesSplit()
        {
            var fields1 = new FieldsMask(new List<Fields> { Fields.Name, Fields.Status});
            Assert.AreEqual((1 << 15) + 1, fields1.Mask[0]);
        }
        [Test]
        public void FieldMaskRollsOver()
        {
            var fields1 = new FieldsMask(new List<Fields> { Fields.Smoker});
            Assert.AreEqual(1 << 15, fields1.Mask[1]);
        }
        [Test]
        public void FieldMaskCreatesTwo()
        {
            var fields3 = new FieldsMask(new List<Fields> { Fields.StarSign, Fields.Status});
            Assert.AreEqual(3, fields3.Mask[0]);
        }
        [Test]
        public void FieldMaskCreatesThree()
        {
            var fields3 = new FieldsMask(new List<Fields> { Fields.StarSign, Fields.Status});
            Assert.AreEqual(3, fields3.Mask[0]);
        }
        [Test]
        public void FieldMaskIsMatch()
        {
            var fields = new FieldsMask(new List<Fields> { Fields.StarSign, Fields.Status});
            var pattern = new FieldsMask(new List<Fields> { Fields.StarSign});
            Assert.IsTrue(fields.IsMatch(pattern));
        }
        [Test]
        public void FieldMaskIsMatchRollover()
        {
            var fields = new FieldsMask(new List<Fields> { Fields.StarSign, Fields.Status, Fields.Smoker});
            var pattern = new FieldsMask(new List<Fields> { Fields.Smoker});
            Assert.IsTrue(fields.IsMatch(pattern));
        }

        [Test]
        public void FindsProperIntersection()
        {
            var left = new FieldsMask(new List<Fields> { Fields.StarSign, Fields.Status, Fields.Smoker});
            var right = new FieldsMask(new List<Fields> { Fields.Smoker, Fields.Status, Fields.Birthdate});
            CollectionAssert.AreEquivalent(new[] { Fields.Smoker, Fields.Status }, left.Intersect(right));
        }

    }
}