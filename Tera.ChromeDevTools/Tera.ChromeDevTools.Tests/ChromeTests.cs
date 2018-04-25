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
            Chrome c = new Chrome(remoteDebuggingPort: 9991);
            var post = Process.GetProcessesByName("chrome");
            Assert.IsTrue(post.Length > prev.Length, "No chrome processes were created on the initialization of a Chrome instance");
            c.Dispose();
        }

        [TestMethod]
        public async Task SessionCreationTest()
        {
            Chrome c = new Chrome(remoteDebuggingPort: 9992, headless: false);

            var currentSessions = await c.GetActiveSessions();
            var s = await c.CreateNewSession();
            var newSessions = await c.GetActiveSessions();
            Assert.IsTrue(currentSessions.Count() + 1 == newSessions.Count(), "The number of sessions before creation + 1 was not equal to the sessions found later");
            c.Dispose();

        }
        [TestMethod]
        public async Task SessionClosingTest()
        {
            Chrome c = new Chrome(remoteDebuggingPort: 9993, headless: false);

            await c.CreateNewSession();
            var currentSessions = await c.GetActiveSessions();

            await c.CloseSession(currentSessions.First());
            var newSessions = await c.GetActiveSessions();
            Assert.IsTrue(currentSessions.Count() - 1 == newSessions.Count(), "The number of sessions before closing - 1 was not equal to the sessions found later");
            c.Dispose();
        }


        [TestMethod]
        public async Task NavigateTest()
        {
            using (Chrome c = new Chrome(remoteDebuggingPort: 9994, headless: false))
            {

                bool reached = false;
                var session = await c.CreateNewSession();
                session.PageLoaded += (s, e) => { reached = true; };
                await session.Navigate("http://www.google.com");
                await Task.Delay(5000);
                //if by now i do not have the result, something went wrong
                Assert.IsTrue(reached, "The webpage was not reached during the time waited");
            }

        }
    }

}
