using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Tera.ChromeDevTools.Tests
{
    [TestClass]
    public class ChromeTests
    {
        [TestMethod]
        public void ChromeOpenTest()
        {
            var prev = Process.GetProcessesByName("chrome");
            Chrome c = new Chrome();
            var post = Process.GetProcessesByName("chrome");
            Assert.IsTrue(post.Length > prev.Length,"No chrome processes were created on the initialization of a Chrome instance");          

        }
    }

}
