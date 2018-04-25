using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Tera.ChromeDevTools
{
    public class Chrome
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

            Process chromeProcess = Process.Start(ChromeUtils.GetChromePath(), chromeProcessArgs);


        }


    }
}
