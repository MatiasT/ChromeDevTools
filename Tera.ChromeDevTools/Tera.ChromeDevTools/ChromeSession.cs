using System;
using System.Collections.Generic;
using System.Text;
using Tera.ChromeDevTools.Protocol;

namespace Tera.ChromeDevTools
{
    public class ChromeSession:IDisposable
    {
        public string Id { get; }
        public string Title { get; private set; }

        private BaristaLabs.ChromeDevTools.Runtime.ChromeSession internalSession;
        private readonly Chrome chrome;

        internal ChromeSession(ChromeSessionInfo chromeSessionInfo, Chrome chrome)
        {
            this.Id = chromeSessionInfo.Id;
            internalSession = new BaristaLabs.ChromeDevTools.Runtime.ChromeSession(chromeSessionInfo.WebSocketDebuggerUrl);
            this.chrome = chrome;
        }
        ~ChromeSession()
        {
            if (internalSession != null)
            internalSession.Dispose();
        }
        internal void innerDispose() {
            internalSession.Dispose();
            internalSession = null;

        }
        public void Dispose()
        {
            innerDispose();
            chrome.CloseSession(this).GetAwaiter().GetResult();
        }
    }
}
