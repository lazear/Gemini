using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.WebSockets;

namespace Gemini
{
	/// <summary>
	/// Websocket wrapper class
	/// </summary>
	public class Websocket : IDisposable
	{
		/// <summary>
		/// 16kB buffer
		/// </summary>
		private int BufferSize = 16384;
		private ClientWebSocket ws;
		private string url;

		/// <summary>
		/// Delegate for receiving data from the websocket
		/// </summary>
		/// <param name="data"></param>
		/// <param name="state"></param>
		public delegate void ReceiveCallback(string data, object state);
		private ReceiveCallback rc;
		private object state;

		/// <summary>
		/// Create a new websocket pointing to a server.
		/// </summary>
		/// <param name="url">Server URL</param>
		/// <param name="callback">Delegate to call when data is received</param>
		/// <param name="state">Optional object to pass to delegate</param>
		public Websocket(string url, ReceiveCallback callback, object state = null)
		{
			ws = new ClientWebSocket();
			ws.Options.SetBuffer(BufferSize, BufferSize);
			this.url = url;
			this.rc += callback;
			this.state = state;
		}

		/// <summary>
		/// Connect to the websocket and start receiving data
		/// </summary>
		/// <returns></returns>
		public async Task Connect()
		{
			try
			{
				await ws.ConnectAsync(new Uri(url), CancellationToken.None);
				await Receive();
			}
			catch (Exception e)
			{
				rc("Exception", e);
			}
			finally
			{
				ws.Dispose();
			}
		}

		private async Task Receive()
		{
			string overflow = "";
			while (ws.State == WebSocketState.Open)
			{
				var buffer = new byte[BufferSize];
				var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				while (!result.EndOfMessage)
				{
					overflow += Encoding.UTF8.GetString(buffer.TakeWhile((x) => x != 0).ToArray());
					result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				}
				overflow += Encoding.UTF8.GetString(buffer.TakeWhile((x) => x != 0).ToArray());

				rc?.Invoke(overflow, state);
				overflow = "";
			}
            rc?.Invoke("Disconnected", null);
		}

		/// <summary>
		/// Add a header to the Websocket HTTP request
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void AddHeader(string key, string value)
		{
			ws.Options.SetRequestHeader(key, value);
		}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    
                    ws.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Websocket() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
