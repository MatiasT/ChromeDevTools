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
        ~Chrome()
        {
            closeProcess();
        }
        public void Dispose()
        {
            closeProcess();
        }
    }
}
