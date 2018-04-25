﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tera.ChromeDevTools.Protocol;

namespace Tera.ChromeDevTools
{
    public class ChromeSession : IDisposable
    {
        public string Id { get; }
        public string Title { get; private set; }

        public delegate void PageLoadedEventHandler(ChromeSession session, double Timestamp);
        public event PageLoadedEventHandler PageLoaded;
        private BaristaLabs.ChromeDevTools.Runtime.ChromeSession internalSession;
        private readonly Chrome chrome;

        internal ChromeSession(ChromeSessionInfo chromeSessionInfo, Chrome chrome)
        {
            this.Id = chromeSessionInfo.Id;
            internalSession = new BaristaLabs.ChromeDevTools.Runtime.ChromeSession(chromeSessionInfo.WebSocketDebuggerUrl);
            this.chrome = chrome;
            InitializePage();
        }

        private void InitializePage()
        {
            //enables events.
            internalSession.Page.Enable();
            internalSession.Page.SubscribeToLoadEventFiredEvent((evt) =>
            {
                //this should trigger when a page loads.
                PageLoaded?.Invoke(this, evt.Timestamp);
            });
        }

        public async Task Navigate(string Url) {
           var result =   await internalSession.Page.Navigate(new BaristaLabs.ChromeDevTools.Runtime.Page.NavigateCommand() { Url = Url });
            //todo(Tera): Maybe return something here? like the body, update the title, or whatever?
        }
        #region cleanup
        ~ChromeSession()
        {
            if (internalSession != null)
                internalSession.Dispose();
        }
        internal void innerDispose()
        {
            internalSession.Dispose();
            internalSession = null;

        }
        public void Dispose()
        {
            innerDispose();
            chrome.CloseSession(this).GetAwaiter().GetResult();
        }
        #endregion
    }
}