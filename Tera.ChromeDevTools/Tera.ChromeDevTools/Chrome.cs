using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Tera.ChromeDevTools.Protocol;

namespace Tera.ChromeDevTools
{
    /// <summary>
    /// Represents a Chrome instance.
    /// Might not work if you try to open several instances on the same port.
    /// 
    /// </summary>
    public class Chrome : IDisposable
    {
        private Process chromeProcess;
        private string directoryInfo;
        private int remoteDebuggingPort;
        private Dictionary<string, ChromeSession> aliveSessions;
        /// <summary>
        /// Initializes an instance of Chrome
        /// </summary>
        /// <param name="remoteDebuggingPort">The port provided for the remote debugger</param>
        /// <param name="headless">indicates if the window should be hidden</param>
        public Chrome(int remoteDebuggingPort = 9222, bool headless = true)
        {
            directoryInfo = ChromeUtils.CreateTempFolder();
            this.remoteDebuggingPort = remoteDebuggingPort;
            var remoteDebuggingArg = $"--remote-debugging-port={remoteDebuggingPort}";
            var userDirectoryArg = $"--user-data-dir=\"{directoryInfo}\"";
            var chromeProcessArgs = $"{remoteDebuggingArg} {userDirectoryArg} --bwsi --no-first-run";

            if (headless == true)
            {
                chromeProcessArgs += " --headless";
            }

            chromeProcess = Process.Start(ChromeUtils.GetChromePath(), chromeProcessArgs);
            System.Threading.Thread.Sleep(100);
            aliveSessions = new Dictionary<string, ChromeSession>();

        }

         
        /// <summary>
        /// Ennumerates the available sessions(tabs) 
        /// </summary>
        /// <returns>A collection of ChromeSessions</returns>
        public async Task<IEnumerable<ChromeSession>> GetActiveSessions()
        {
            using (var webClient = GetDebuggerClient())
            {
                var remoteSessions = await webClient.GetStringAsync("/json");
                var validSessions = new List<string>();
                foreach (var item in JsonConvert.DeserializeObject<ICollection<ChromeSessionInfo>>(remoteSessions)
                    .Where(s=>s.Type=="page")
                    )
                {
                    validSessions.Add(item.Id);
                    if (!aliveSessions.ContainsKey(item.Id))
                    {
                        addSession(item);
                    }

                }
                foreach (var invalidKey in aliveSessions.Keys.Except(validSessions))
                {
                    aliveSessions[invalidKey].Dispose();
                }

                return aliveSessions.Values.ToArray();
            }
        }
        private ChromeSession addSession(ChromeSessionInfo info)
        {
            var session = new ChromeSession(info, this);
            aliveSessions.Add(session.Id, session);
            return session;
        }
        /// <summary>
        /// Creates and returns a new Session (Tab)
        /// </summary>
        /// <returns>The newly created Session</returns>
        public async Task<ChromeSession> CreateNewSession()
        {
            using (var webClient = GetDebuggerClient())
            {
                var result = await webClient.PostAsync("/json/new", null);
                var contents = await result.Content.ReadAsStringAsync();
                return addSession(JsonConvert.DeserializeObject<ChromeSessionInfo>(contents));
            }
        }
        /// <summary>
        /// Closes the provided Session(Tab)
        /// </summary>
        /// <param name="session">Session to be closed</param>
        /// <returns></returns>
        public async Task CloseSession(ChromeSession session)
        {
            //TODO(Tera):if i close all the sessions, chrome closes itself! i can use this to gracefully close this stuff, OR maybe i should prevent it?
            using (var webClient = GetDebuggerClient())
            {
                var result = await webClient.PostAsync("/json/close/" + session.Id, null);
                var contents = await result.Content.ReadAsStringAsync();
                //Assert contents == "Target is closing" 
                if (aliveSessions.ContainsKey(session.Id))
                    aliveSessions.Remove(session.Id);
                session.innerDispose();
            }
        }
        private HttpClient GetDebuggerClient()
        {
            var chromeHttpClient = new HttpClient()
            {
                BaseAddress = new Uri($"http://localhost:{remoteDebuggingPort}")
            };

            return chromeHttpClient;
        }





        #region Closing & cleaning
        private void closeProcess()
        {
            if (chromeProcess != null)
            {
                chromeProcess.CloseMainWindow();
                if (!chromeProcess.WaitForExit(1000))
                {
                    chromeProcess.Kill();
                }
                chromeProcess.Dispose();
                chromeProcess = null;
            }
        }
        private void closeSessions()
        {
            foreach (var item in aliveSessions.Values.ToArray())
            {
                this.CloseSession(item).GetAwaiter().GetResult();

            }

        }
        ~Chrome()
        {
            closeSessions();
            closeProcess();
        }

        /// <summary>
        /// Closes the active sessions & finalizes the process created by this instance
        /// </summary>
        public void Dispose()
        {
            closeSessions();
            closeProcess();
        }
        #endregion
    }
}
