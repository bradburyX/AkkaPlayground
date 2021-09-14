using AkkaPlayground.Proto.Data.Masking;
using NUnit.Framework;

namespace ComponentTest
{
    public class FieldBitmaskAtIndexTests
    {
        [Test]
        public void CreatesMin()
        {
            Assert.AreEqual(1, new FieldMask.FieldBitmaskAtIndex(15).Mask);
        }
        [Test]
        public void CreatesMax()
        {
            Assert.AreEqual(1 << 15, new FieldMask.FieldBitmaskAtIndex(0).Mask);
        }
        [Test]
        public void CreatesIndex()
        {
            Assert.AreEqual(0, new FieldMask.FieldBitmaskAtIndex(5).Index);
        }
        [Test]
        public void CreatesMaxOverflow()
        {
            Assert.AreEqual(1 << 15, new FieldMask.FieldBitmaskAtIndex(16).Mask);
        }
        [Test]
        public void CreatesIndexOverflow()
        {
            Assert.AreEqual(1, new FieldMask.FieldBitmaskAtIndex(17).Index);
        }
    }
}
