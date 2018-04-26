using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tera.ChromeDevTools.Tests
{
    [TestClass]
    public class JSTests
    {
        [TestMethod]
        public async Task IntEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                var s = await c.CreateNewSession();
                int result = await s.Eval<int>("7");
                Assert.AreEqual(7, result, "The received result did not match the expected result");
            }
        }
        [TestMethod]
        public async Task BoolEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                var s = await c.CreateNewSession();
                bool result = await s.Eval<bool>("true");
                Assert.AreEqual(true, result, "The received result did not match the expected result");
            }
        }
        [TestMethod]
        public async Task DoubleEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                var s = await c.CreateNewSession();
                double result = await s.Eval<double>("7.3543");
                Assert.AreEqual(7.3543, result, "The received result did not match the expected result");
            }
        }

        [TestMethod]
        public async Task ArrayEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                int[] expected = new int[] { 1, 2, 3, 4, 5 };
                var s = await c.CreateNewSession();
                var result = await s.Eval<IEnumerable<int>>("[1, 2, 3, 4, 5]");
                Assert.IsTrue(expected.SequenceEqual(result), "The received result did not match the expected result");
            }

        }
        [TestMethod]
        public async Task DynamicArrayEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                object[] expected = new object[] { 1, true, "asd" };
                var s = await c.CreateNewSession();
                var result = await s.Eval<IEnumerable<object>>("[1, true, 'asd']");
                Assert.IsTrue(expected.SequenceEqual(result), "The received result did not match the expected result");
            }
        }
        [TestMethod]
        public async Task literalObjectEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                Point expected = new Point(10, 3);
                var s = await c.CreateNewSession();
                var result = await s.Eval<Point>("{'x':10,'y':3}");
                Assert.AreEqual(expected,result, "The received result did not match the expected result");
            }

        }

    }
}
