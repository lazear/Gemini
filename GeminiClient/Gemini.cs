using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
using Gemini.Contracts;

namespace Gemini
{
    [Serializable]
    public class GeminiWallet
    {
        public string Url;
        public string Key;
        public string Secret;
        public int Nonce;
    }

    /// <summary>
    /// Class to handle storage and retrieval of Gemini API Key pair
    /// </summary>
    public class Wallet
    {

        public delegate void WalletEvent(Wallet sender, EventArgs e);
        /// <summary>
        /// Triggered when a new Wallet is loaded
        /// </summary>
        /// <param name="w"></param>
        /// <param name="e"></param>
        public event WalletEvent OnChange;

        /// <summary>
        /// In-memory representation of the Wallet file
        /// </summary>
        private static GeminiWallet auth = null;

        /// <summary>
        /// Current Wallet file
        /// </summary>
        private static string _filename = "";

        /// <summary>
        /// Current Wallet password. Used for saving new information on application close
        /// </summary>
        private static string _password = "";

       
        public Wallet(string key = "", string secret = "", string url = "", int nonce = 1)
        {

            if (key != String.Empty && secret != String.Empty)
            {
                auth = new GeminiWallet { Key = key, Secret = secret, Url = url, Nonce = nonce };
                if (OnChange != null)
                    OnChange(this, null);
            }
        }

        /// <summary>
        /// Open an AES-256 encrypted Wallet file containing API Key and Secret
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="password"></param>
        public void Open(string filename, string password)
        {
            Close();
            Stream file = Cryptography.AesDecryption(filename, password);
            XmlSerializer xml = new XmlSerializer(typeof(GeminiWallet));
            auth = xml.Deserialize(file) as GeminiWallet;
            file.Close();
            _filename = filename;
            _password = password;
            if (OnChange != null)
                OnChange(this, null);
        }

        /// <summary>
        /// Create a new Wallet file, storing the API Key and Secret as AES-256 encrypted XML
        /// </summary>
        /// <param name="filename">Filename for Wallet file</param>
        /// <param name="password">Password to use for AES-256 encryption</param>
        public void Create(GeminiWallet keys, string filename, string password)
        {
            using (var stream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(GeminiWallet));
                xml.Serialize(stream, keys);
                stream.Seek(0, SeekOrigin.Begin);
                Cryptography.AesEncryption(stream, filename, password);
                _filename = filename;
                if (OnChange != null)
                    OnChange(this, null);
            }
        }

        /// <summary>
        /// Save the currently loaded API Key and Secret to their original file
        /// </summary>
        public void Close()
        {
            if (auth == null)
                return;
            using (var s = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(GeminiWallet));
                xml.Serialize(s, auth);
                s.Seek(0, SeekOrigin.Begin);
                Cryptography.AesEncryption(s, _filename, _password);

                auth = null;
            }
        }

        /// <summary>
        /// Return the public API Key for the current Wallet profile
        /// </summary>
        /// <returns></returns>
        public string Key()
        {
            if (auth == null)
                return String.Empty;
            return auth.Key;
        }

        /// <summary>
        /// Sign and complete a pending API Request
        /// </summary>
        /// <typeparam name="T">DataContract serializable type that is a PrivateRequest</typeparam>
        /// <param name="message">Requests object to sign</param>
        /// <param name="data">Data to be JSON encoded</param>
        /// <returns>Request, with signed payload and proper headers and url</returns>
        public Requests Authenticate<T>(Requests message, T data) where T : PrivateRequest
        {
            if (message == null || data == null)
                throw new NullReferenceException();
            if (auth == null)
                throw new NullReferenceException("No wallet keys loaded");

            data.Nonce = auth.Nonce;
            auth.Nonce += 1;

            var serial = new Serializer<T>();
            string plain = serial.Write(data);

            string base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(plain));
            string signature = Cryptography.SHA384Sign(base64, auth.Secret);

            message.Url = auth.Url + data.Request;
            message.Headers["X-GEMINI-APIKEY"] = auth.Key;
            message.Headers["X-GEMINI-PAYLOAD"] = base64;
            message.Headers["X-GEMINI-SIGNATURE"] = signature;

            return message;
        }
    }

    /// <summary>
    /// Singleton class that provides access to Gemini Private API functions
    /// </summary>
    public class GeminiClient
    {
        private static GeminiClient instance;
        public static Wallet Wallet = new Wallet();

        private GeminiClient() { }

        public static GeminiClient Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GeminiClient();
                }
                return instance;
            }
        }

        public delegate void ErrorHandler(string reason, string message);
        public static ErrorHandler Handler;

        /// <summary>
        /// Delegate function to be called on API errors
        /// </summary>
        /// <param name="eh">Delegate function</param>
        public static void InstallErrorHandler(ErrorHandler eh)
        {
            Handler += eh;
        }


        private static void HandleError(HttpResponseMessage message)
        {
            ErrorCode error = new Serializer<ErrorCode>().Read(message.Content.ReadAsStreamAsync().Result);
            Handler(error.Reason, error.Message);
        }

        /* 
         * BEGIN PUBLIC API FUNCTIONS
         */

        /// <summary>
        /// Get the last ticker price for a cryptocurrency
        /// </summary>
        /// <param name="currency">Valid strings can be queryed by /v1/symbols API</param>
        /// <returns>Last price as a decimal</returns>
        public static decimal GetLastPrice(string currency)
        {
            Requests re = new Requests("https://api.gemini.com/v1/pubticker/" + currency.ToLower());
            var result = re.Get().Result;
            if (result.IsSuccessStatusCode)
                return result.Json<Ticker>().Last;
            HandleError(result);
            return 0.0M;
        }

        /// <summary>
        /// Get array of valid exchange symbols
        /// </summary>
        /// <returns></returns>
        public string[] GetSymbols()
        {
            Requests re = new Requests("https://api.gemini.com/v1/symbols");
            var result = re.Get().Result;
            if (result.IsSuccessStatusCode)
                return result.Json<string[]>();
            HandleError(result);
            return null;
        }

        /*
         * BEGIN PRIVATE API FUNCTIONS
         */

        /// <summary>
        /// Request account balances
        /// </summary>
        /// <returns></returns>
        public static BalanceRequest[] GetBalances()
        {
            Requests re = new Requests();
            Wallet.Authenticate(re, new BalanceRequest { Nonce = 2, Request = "/v1/balances" });
            var result = re.Post().Result;
            if (result.IsSuccessStatusCode)
                return result.Json<BalanceRequest[]>();
            HandleError(result);
            return null;
        }

        /// <summary>
        /// Generate a new cryptocurrency deposit address
        /// </summary>
        /// <param name="currency">Valid currencies: "eth", "btc"</param>
        /// <param name="label">Optional label for the address</param>
        /// <returns>New deposit address, encoded as a string</returns>
        public static string GetDepositAddress(string currency, string label="")
        {
            string request = String.Format("/v1/deposit/{0}/newAddress", currency);
            Requests re = new Requests();
            Wallet.Authenticate<DepositAddressRequest>(re, new DepositAddressRequest { Label = label, Request = request });

            var result = re.Post().Result;
            if (result.IsSuccessStatusCode)
                return result.Json<DepositAddress>().Address;
            HandleError(result);
            return null;
        }

        /// <summary>
        /// Request a new order
        /// </summary>
        /// <param name="order">NewOrderRequest to send to the server</param>
        /// <returns>OrderStatus object</returns>
        public static OrderStatus PlaceOrder(NewOrderRequest order)
        {
            Requests re = new Requests();
            order.Request = "/v1/order/new";
            Wallet.Authenticate<NewOrderRequest>(re, order);

            var status = re.Post().Result;
            if (status.IsSuccessStatusCode)
                return status.Json<OrderStatus>();

            HandleError(status);
            return null;
        }

        /// <summary>
        /// Retrieve information on an order by it's order ID
        /// </summary>
        /// <param name="order_id">Server-side order identifier</param>
        /// <returns></returns>
        public static OrderStatus GetOrder(int order_id)
        {
            Requests re = new Requests();
            Wallet.Authenticate<OrderStatusRequest>(re, new OrderStatusRequest() { Request = "/v1/order/status", OrderID = order_id });

            var status = re.Post().Result;
            if (status.IsSuccessStatusCode)
                return status.Json<OrderStatus>();
            HandleError(status);
            return null;
        }

        /// <summary>
        /// Get open orders that have been placed with this API Key
        /// </summary>
        /// <returns>Array of OrderStatus objects, or NULL on failure</returns>
        public static OrderStatus[] GetOrders()
        {
            Requests re = new Requests();
            Wallet.Authenticate<OrderStatusRequest>(re, new OrderStatusRequest() { Request = "/v1/orders" });

            var status = re.Post().Result;
            if (status.IsSuccessStatusCode)
                return status.Json<OrderStatus[]>();
            HandleError(status);
            return null;
        }

        /// <summary>
        /// Cancel a specific order by order ID
        /// </summary>
        /// <param name="order_id">Server-side order identifier</param>
        /// <returns></returns>
        public static OrderStatus CancelOrder(int order_id)
        {
            Requests re = new Requests();
            Wallet.Authenticate<OrderStatusRequest>(re, new OrderStatusRequest() { Request = "/v1/order/cancel", OrderID = order_id });

            var status = re.Post().Result;
            if (status.IsSuccessStatusCode)
                return status.Json<OrderStatus>();
            HandleError(status);
            return null;
        }

        /// <summary>
        /// Cancel all orders created this session
        /// </summary>
        /// <returns>Boolean representing command status</returns>
        public static bool CancelSession()
        {
            Requests re = new Requests();
            Wallet.Authenticate<BoolResponse>(re, new BoolResponse { Request = "/v1/order/cancel/session" });
            var res = re.Post().Result;
            if (res.IsSuccessStatusCode)
                return true;

            HandleError(res);
            return false;
        }

        /// <summary>
        /// Cancel all outstanding orders
        /// </summary>
        /// <returns>Boolean representing command status</returns>
        public static bool CancelAll()
        {
            Requests re = new Requests();
            Wallet.Authenticate<BoolResponse>(re, new BoolResponse { Request = "/v1/order/cancel/all" });

            var res = re.Post().Result;
            if (res.IsSuccessStatusCode)
                return true;

            HandleError(res);
            return false;
        }
    }
}
