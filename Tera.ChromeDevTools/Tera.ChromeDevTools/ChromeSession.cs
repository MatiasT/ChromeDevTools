using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
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

        public async Task<T> EvalValue<T>(string v) where T : struct
        {
            var result = await internalSession.Runtime.Evaluate(new BaristaLabs.ChromeDevTools.Runtime.Runtime.EvaluateCommand() { Expression = v });
            if (result.ExceptionDetails != null)
            {
                //TODO(Tera):do something with the error
                throw new NotImplementedException();
            }
            return (T)Convert.ChangeType(result.Result.Value, typeof(T));
        }
        public async Task<dynamic> EvalObject(string v)
        {
            var result = await internalSession.Runtime.Evaluate(new BaristaLabs.ChromeDevTools.Runtime.Runtime.EvaluateCommand() { Expression = v });
            if (result.ExceptionDetails != null)
            {
                //TODO(Tera):do something with the error
                throw new NotImplementedException();
            }
            return DynamicObjectResult.Get(result, this);
        }

        public Task<IEnumerable<T>> EvalEnumerable<T>(string v)
        {
            throw new NotImplementedException();
        }
        private BaristaLabs.ChromeDevTools.Runtime.ChromeSession internalSession;
        private readonly Chrome chrome;

        internal ChromeSession(ChromeSessionInfo chromeSessionInfo, Chrome chrome)
        {
            this.Id = chromeSessionInfo.Id;
            internalSession = new BaristaLabs.ChromeDevTools.Runtime.ChromeSession(chromeSessionInfo.WebSocketDebuggerUrl);
            this.chrome = chrome;
            Task.WaitAll(InitializePage());
        }

        private async Task InitializePage()
        {
            //enables events.

            await internalSession.Page.Enable();
            await Task.Delay(100);
            internalSession.Page.SubscribeToLoadEventFiredEvent((evt) =>
            {
                //this should trigger when a page loads.
                PageLoaded?.Invoke(this, evt.Timestamp);
            });
        }

        internal async Task RemoveObject(string objectId)
        {
            await internalSession.Runtime.ReleaseObject(new BaristaLabs.ChromeDevTools.Runtime.Runtime.ReleaseObjectCommand() { ObjectId = objectId });
        }

        public async Task Navigate(string Url)
        {
            var result = await internalSession.Page.Navigate(new BaristaLabs.ChromeDevTools.Runtime.Page.NavigateCommand() { Url = Url });
            //todo(Tera): Maybe return something here? like the body, update the title, or whatever?
        }

        internal async Task<BaristaLabs.ChromeDevTools.Runtime.Runtime.GetPropertiesCommandResponse> InspectObject(string ObjectId)
        {
            return await internalSession.Runtime.GetProperties(new BaristaLabs.ChromeDevTools.Runtime.Runtime.GetPropertiesCommand() { ObjectId = ObjectId });
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
