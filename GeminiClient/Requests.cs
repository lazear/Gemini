/*
MIT License

Copyright (c) Michael Lazear 2017 

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace Gemini
{
    /// <summary>
    /// Wrapper class for current timestamp
    /// </summary>
	public class Time
	{
        /// <summary>
        /// Epoch timestamp in seconds of current time
        /// </summary>
        /// <returns></returns>
        public static long Timestamp()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}

		/// <summary>
		/// Epoch timestamp in milliseconds of current time
		/// </summary>
		/// <returns></returns>
		public static long TimestampMs()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
		}

	}

	/// <summary>
	/// Extensions to HttpResponseMessage
	/// </summary>
	public static class Extensions
	{
		/// <summary>
		/// Deserialize an HttpResponseMessage from JSON to <typeparamref name="T"/>
		/// </summary>
		/// <typeparam name="T">Class with [DataContract] attribute</typeparam>
		/// <param name="message">A successfull HttpResponseMessage object</param>
		/// <returns></returns>
		public static T Json<T>(this HttpResponseMessage message)
		{
			var stream = message.Content.ReadAsStreamAsync();
			return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream.Result);
		}

        /// <summary>
        /// Deserialize a string from JSON to <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <returns></returns>
		public static T Json<T>(this string message)
		{
			var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(message));

			return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream);
		}

		/// <summary>
		/// Converts a DateTime object to epoch time
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static long ToTimestamp(this DateTime dt)
		{
			return (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		}

		/// <summary>
		/// Converts a DateTime object to epoch time
		/// </summary>
		/// <param name="dt"></param>
		/// <returns></returns>
		public static long ToTimestampMs(this DateTime dt)
		{
			return (long)(dt - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
		}
	}

	/// <summary>
	/// HttpClient wrapper class for ease-of-use
	/// </summary>
	public class Requests
	{
        /// <summary>
        /// Base URL
        /// </summary>
		public string Url { get; set; }
        /// <summary>
        /// By default, "text/plain"
        /// </summary>
		public string ContentType { get; set; }
        /// <summary>
        /// Data to send with HTTP Request
        /// </summary>
		public string Data { get; set; }
        /// <summary>
        /// HTTP Request headers
        /// </summary>
		public Dictionary<string, string> Headers { get; set; }
        /// <summary>
        /// Parameters to encode in URL request
        /// </summary>
		public Dictionary<string, string> Parameters { get; set; }

		private static HttpClient client = new HttpClient();

        /// <summary>
        /// Initialize a Requests instance
        /// </summary>
        /// <param name="url"></param>
		public Requests(string url = "")
		{
			Headers = new Dictionary<string, string>();
			Parameters = new Dictionary<string, string>();
			this.Url = url;
			this.Data = "";
			this.ContentType = "text/plain";
		}

		private Uri GenerateUri()
		{
			return new Uri(this.Url +
				this.Parameters.Aggregate(
					new StringBuilder(),
					(sb, pair) => sb.AppendFormat("{0}={1}&", pair.Key, pair.Value),
					(sb) => sb.ToString()));
		}

		private HttpRequestMessage GenerateRequest(HttpMethod method)
		{
			HttpRequestMessage request = new HttpRequestMessage(method, GenerateUri());
			foreach (KeyValuePair<string, string> p in this.Headers)
				request.Headers.Add(p.Key, p.Value);
			if (this.Data != String.Empty)
				request.Content = new StringContent(this.Data, Encoding.UTF8);
			request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(this.ContentType));
			return request;
		}

		/// <summary>
		/// Send an HttpRequestMessage that has a valid HttpMethod set
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
		{
			return client.SendAsync(request);
		}

		/// <summary>
		/// Send request as an HTTP GET
		/// </summary>
		/// <returns></returns>
		public Task<HttpResponseMessage> Get()
		{
			return SendAsync(GenerateRequest(HttpMethod.Get));
		}

		/// <summary>
		/// Send request as an HTTP POST
		/// </summary>
		/// <returns></returns>
		public Task<HttpResponseMessage> Post()
		{
			return SendAsync(GenerateRequest(HttpMethod.Post));
		}

		/* static methods for calling without class instance */
        /// <summary>
        /// Send an HTTP GET request
        /// </summary>
        /// <param name="url">base url</param>
        /// <param name="data">data to send in request</param>
        /// <param name="headers">HTTP Request headers</param>
        /// <param name="parameters">Url parameters to encode</param>
        /// <returns></returns>
		public static Task<HttpResponseMessage> Get(string url, string data = "", Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
		{
			if (parameters != null)
			{
				url += parameters.Aggregate(
				 new StringBuilder(),
				 (sb, pair) => sb.AppendFormat("{0}={1}&", pair.Key, pair.Value),
				 (sb) => sb.ToString());
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
			if (data != null && data != String.Empty)
				request.Content = new StringContent(data, Encoding.UTF8);
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> p in headers)
					request.Headers.Add(p.Key, p.Value);
			}

			request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
			return client.SendAsync(request);
		}

        /// <summary>
        /// Send an HTTP POST request
        /// </summary>
        /// <param name="url">base url</param>
        /// <param name="data">data to send in request</param>
        /// <param name="headers">HTTP Request headers</param>
        /// <param name="parameters">Url parameters to encode</param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> Post(string url, string data = "", Dictionary<string, string> headers = null, Dictionary<string, string> parameters = null)
		{
			if (parameters != null)
			{
				url += parameters.Aggregate(
				 new StringBuilder(),
				 (sb, pair) => sb.AppendFormat("{0}={1}&", pair.Key, pair.Value),
				 (sb) => sb.ToString());
			}

			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
			if (data != String.Empty)
				request.Content = new StringContent(data, Encoding.UTF8);
			foreach (KeyValuePair<string, string> p in headers)
				request.Headers.Add(p.Key, p.Value);
			request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
			return client.SendAsync(request);
		}
	}
}
