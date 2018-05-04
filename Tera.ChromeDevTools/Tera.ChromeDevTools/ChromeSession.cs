using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tera.ChromeDevTools.Protocol;

namespace Tera.ChromeDevTools
{
    /// <summary>
    /// Represents a single Chrome Session (Tab)
    /// </summary>
    public class ChromeSession : IDisposable
    {
        /// <summary>
        /// The unique Id of this Session
        /// </summary>
        public string Id { get; }

        //TODO(Tera): finish this.
        /// <summary>
        /// The title in the Session (Not working yet?)
        /// </summary>
        public string Title { get; private set; }
        
        /// <summary>
        /// A handler that can carry the active session and the current timestamp
        /// </summary>
        /// <param name="session">The session that triggered the event</param>
        /// <param name="Timestamp">The timestamp of this event</param>
        public delegate void PageLoadedEventHandler(ChromeSession session, double Timestamp);
        /// <summary>
        /// Event that triggers when a page is loaded
        /// </summary>
        public event PageLoadedEventHandler PageLoaded;
        /// <summary>
        /// Evaluates a given javascript, returning a value
        /// </summary>
        /// <typeparam name="T">The type of the provided value</typeparam>
        /// <param name="v">The javascript code to be evaluated</param>
        /// <returns>Returns the value resulting from the evaluation of the given code</returns>
        public async Task<T> EvalValue<T>(string v) where T : struct
        {
            var result = await internalSession.Runtime.Evaluate(new BaristaLabs.ChromeDevTools.Runtime.Runtime.EvaluateCommand() { Expression = v });
            if (result.ExceptionDetails != null)
            {
                throw new ChromeRemoteException(result.ExceptionDetails);
            }
            return (T)Convert.ChangeType(result.Result.Value, typeof(T));
        }
        /// <summary>
        /// Evaluates a given javascript, returning an object
        /// </summary>
        /// <param name="v">The javascript code to be evaluated</param>
        /// <returns>A dynamic object that can be explored</returns>
        public async Task<dynamic> EvalObject(string v)
        {
            var result = await internalSession.Runtime.Evaluate(new BaristaLabs.ChromeDevTools.Runtime.Runtime.EvaluateCommand() { Expression = v });
            if (result.ExceptionDetails != null)
            {
                throw new ChromeRemoteException(result.ExceptionDetails);
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
            await internalSession?.Runtime.ReleaseObject(new BaristaLabs.ChromeDevTools.Runtime.Runtime.ReleaseObjectCommand() { ObjectId = objectId });
        }
        /// <summary>
        /// Redirects the current session to the given url
        /// </summary>
        /// <param name="Url">The desired url</param>
        /// <returns></returns>
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
