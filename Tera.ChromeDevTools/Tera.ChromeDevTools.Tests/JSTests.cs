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
                int result = await s.EvalValue<int>("7");
                Assert.AreEqual(7, result, "The received result did not match the expected result");
            }
        }
        [TestMethod]
        public async Task BoolEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                var s = await c.CreateNewSession();
                bool result = await s.EvalValue<bool>("true");
                Assert.AreEqual(true, result, "The received result did not match the expected result");
            }
        }
        [TestMethod]
        public async Task DoubleEvalTest()
        {
            using (Chrome c = new Chrome())
            {
                var s = await c.CreateNewSession();
                double result = await s.EvalValue<double>("7.3543");
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
                IEnumerable<int> result = await s.EvalEnumerable<int>("[1, 2, 3, 4, 5]");
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
                IEnumerable<object> result = await s.EvalEnumerable<object>("[1, true, 'asd']");
                Assert.IsTrue(expected.SequenceEqual(result), "The received result did not match the expected result");
            }
        }
        [TestMethod]
        public async Task ObjectLiteralEvalTest()
        {
            using (Chrome c = new Chrome(headless: false))
            {
                Point expected = new Point(10, 3);
                var s = await c.CreateNewSession();
                var result = await s.EvalObject("var a ={ 'X' : 10, 'Y' : 3}; a;");
                Assert.AreEqual(expected.X, result.X, "The received result did not match the expected result");
                Assert.AreEqual(expected.Y, result.Y, "The received result did not match the expected result");
            }

        }
       
    }
}
