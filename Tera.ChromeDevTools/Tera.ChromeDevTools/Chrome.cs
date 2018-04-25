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
    public class Chrome : IDisposable
    {
        private Process chromeProcess;
        private string directoryInfo;
        private int remoteDebuggingPort;
        private Dictionary<string, ChromeSession> aliveSessions;

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
            aliveSessions = new Dictionary<string, ChromeSession>();

        }

         

        public async Task<IEnumerable<ChromeSession>> GetActiveSessions()
        {
            using (var webClient = GetDebuggerClient())
            {
                var remoteSessions = await webClient.GetStringAsync("/json");
                var validSessions = new List<string>();
                foreach (var item in JsonConvert.DeserializeObject<ICollection<ChromeSessionInfo>>(remoteSessions))
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
        public async Task<ChromeSession> CreateNewSession()
        {
            using (var webClient = GetDebuggerClient())
            {
                var result = await webClient.PostAsync("/json/new", null);
                var contents = await result.Content.ReadAsStringAsync();
                return addSession(JsonConvert.DeserializeObject<ChromeSessionInfo>(contents));
            }
        }

        public async Task CloseSession(ChromeSession session)
        {
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
        public void Dispose()
        {
            closeSessions();
            closeProcess();
        }
        #endregion
    }
}
