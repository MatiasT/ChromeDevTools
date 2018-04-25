using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Tera.ChromeDevTools
{
    internal static class ChromeUtils
    {
        public static string GetChromePath() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "google-chrome";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
            }
            else
            {
                throw new InvalidOperationException("Unknown or unsupported platform.");
            }
        }
        public static string CreateTempFolder() {
            string path = Path.GetRandomFileName();
            return Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), path)).FullName;
        }

    }
}
