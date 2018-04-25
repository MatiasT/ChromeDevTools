using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Assert.IsTrue(post.Length > prev.Length, "No chrome processes were created on the initialization of a Chrome instance");
            c.Dispose();
        }

        [TestMethod]
        public void SessionCreationTest()
        {
            Task.Run(async () =>
            {

                Chrome c = new Chrome(headless: false);

                var currentSessions = await c.GetActiveSessions();
                var s = await c.CreateNewSession();
                var newSessions = await c.GetActiveSessions();
                Assert.IsTrue(currentSessions.Count() + 1 == newSessions.Count(), "The number of sessions before creation + 1 was not equal to the sessions found later");
                c.Dispose();
            }).GetAwaiter().GetResult();
        }
        [TestMethod]
        public void SessionClosingTest()
        {
            Task.Run(async () =>
            {
                Chrome c = new Chrome(headless: false);
                var currentSessions = await c.GetActiveSessions();
                if (currentSessions.Count() == 0)
                {
                    await c.CreateNewSession();
                    currentSessions = await c.GetActiveSessions();
                }
                await c.CloseSession(currentSessions.First());
                var newSessions = await c.GetActiveSessions();
                Assert.IsTrue(currentSessions.Count() - 1 == newSessions.Count(), "The number of sessions before closing - 1 was not equal to the sessions found later");
                c.Dispose();
            }).GetAwaiter().GetResult();
        }
    }

}
