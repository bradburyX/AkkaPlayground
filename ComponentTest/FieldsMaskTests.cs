using System.Collections.Generic;
using AkkaPlayground.Proto.Data.Masking;
using NUnit.Framework;

namespace ComponentTest
{
    public class FieldMaskTests
    {
        [Test]
        public void FieldMaskCreatesOne()
        {
            var fields1 = new FieldMask(new List<FieldName>{ FieldName.Status });
            Assert.AreEqual(1, fields1.Mask[0]);
        }
        [Test]
        public void FieldMaskCreatesSplit()
        {
            var fields1 = new FieldMask(new List<FieldName> { FieldName.Name, FieldName.Status});
            Assert.AreEqual((1 << 15) + 1, fields1.Mask[0]);
        }
        [Test]
        public void FieldMaskRollsOver()
        {
            var fields1 = new FieldMask(new List<FieldName> { FieldName.Smoker});
            Assert.AreEqual(1 << 15, fields1.Mask[1]);
        }
        [Test]
        public void FieldMaskCreatesTwo()
        {
            var fields3 = new FieldMask(new List<FieldName> { FieldName.StarSign, FieldName.Status});
            Assert.AreEqual(3, fields3.Mask[0]);
        }
        [Test]
        public void FieldMaskCreatesThree()
        {
            var fields3 = new FieldMask(new List<FieldName> { FieldName.StarSign, FieldName.Status});
            Assert.AreEqual(3, fields3.Mask[0]);
        }
        [Test]
        public void FieldMaskIsMatch()
        {
            var fields = new FieldMask(new List<FieldName> { FieldName.StarSign, FieldName.Status});
            var pattern = new FieldMask(new List<FieldName> { FieldName.StarSign});
            Assert.IsTrue(fields.IsMatch(pattern));
        }
        [Test]
        public void FieldMaskIsMatchRollover()
        {
            var fields = new FieldMask(new List<FieldName> { FieldName.StarSign, FieldName.Status, FieldName.Smoker});
            var pattern = new FieldMask(new List<FieldName> { FieldName.Smoker});
            Assert.IsTrue(fields.IsMatch(pattern));
        }

        [Test]
        public void FindsProperIntersection()
        {
            var left = new FieldMask(new List<FieldName> { FieldName.StarSign, FieldName.Status, FieldName.Smoker});
            var right = new FieldMask(new List<FieldName> { FieldName.Smoker, FieldName.Status, FieldName.Birthdate});
            CollectionAssert.AreEquivalent(new[] { FieldName.Smoker, FieldName.Status }, left.Intersect(right));
        }

    }
}