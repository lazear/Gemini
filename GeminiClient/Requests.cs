using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization.Json;

namespace Gemini
{
    class Time
    {
        private static long Timestamp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
        }

        private static long TimestampMs()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }
    }

    static class RequestsExtensions
    {
        public static T Json<T>(this HttpResponseMessage message)
        {
            var stream = message.Content.ReadAsStreamAsync();
            return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(stream.Result);
        }
    }

    public class Requests
    {
        public string Url { get; set; }
        public string ContentType { get; set; }
        public string Data { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        private static HttpClient client = new HttpClient();

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

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return client.SendAsync(request);
        }

        public Task<HttpResponseMessage> Get()
        {
            return SendAsync(GenerateRequest(HttpMethod.Get));
        }

        public Task<HttpResponseMessage> Post()
        {
            return SendAsync(GenerateRequest(HttpMethod.Post));
        }

        /* static methods for calling without class instance */
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
            if (data != String.Empty)
                request.Content = new StringContent(data, Encoding.UTF8);
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> p in headers)
                    request.Headers.Add(p.Key, p.Value);
            }

            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            return client.SendAsync(request);
        }


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
