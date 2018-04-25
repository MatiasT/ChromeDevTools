using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Tera.ChromeDevTools
{
    public class Chrome:IDisposable
    {
        private Process chromeProcess;
        private string directoryInfo;
        private int remoteDebuggingPort;


        public Chrome(int remoteDebuggingPort = 9222, bool headless = true)
        {
            directoryInfo = ChromeUtils.CreateTempFolder();
            var remoteDebuggingArg = $"--remote-debugging-port={remoteDebuggingPort}";
            var userDirectoryArg = $"--user-data-dir=\"{directoryInfo}\"";
            var chromeProcessArgs = $"{remoteDebuggingArg} {userDirectoryArg} --bwsi --no-first-run";

            if (headless == true)
            {
                chromeProcessArgs += " --headless";
            }

            chromeProcess = Process.Start(ChromeUtils.GetChromePath(), chromeProcessArgs);


        }
        ~Chrome()
        {
            if (chromeProcess != null) { 
                chromeProcess.Kill();
                chromeProcess = null;
            }
        }
        public void Dispose()
        {
            chromeProcess.Dispose();
            chromeProcess = null;
        }
    }
}
