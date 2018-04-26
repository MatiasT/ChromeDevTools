using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
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
    }
}
