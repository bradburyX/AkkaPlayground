using System;
using System.Collections.Generic;
using System.Text;
using AkkaPlayground.Proto.Data;
using NUnit.Framework;

namespace ComponentTest
{
    public class FieldBitmaskAtIndexTests
    {
        [Test]
        public void CreatesMin()
        {
            Assert.AreEqual(1, new FieldsMask.FieldBitmaskAtIndex(15).Mask);
        }
        [Test]
        public void CreatesMax()
        {
            Assert.AreEqual(1 << 15, new FieldsMask.FieldBitmaskAtIndex(0).Mask);
        }
        [Test]
        public void CreatesIndex()
        {
            Assert.AreEqual(0, new FieldsMask.FieldBitmaskAtIndex(5).Index);
        }
        [Test]
        public void CreatesMaxOverflow()
        {
            Assert.AreEqual(1 << 15, new FieldsMask.FieldBitmaskAtIndex(16).Mask);
        }
        [Test]
        public void CreatesIndexOverflow()
        {
            Assert.AreEqual(1, new FieldsMask.FieldBitmaskAtIndex(17).Index);
        }
    }
}
