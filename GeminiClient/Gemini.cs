using System;
using System.Text;
using System.Net.Http;
using System.IO;
using System.Xml.Serialization;
using Gemini.Contracts;

namespace Gemini
{
    /// <summary>
    /// XML schema for wallet file
    /// </summary>
    [Serializable]
    public class GeminiWallet
    {
        /// <summary>
        /// API Url
        /// </summary>
        public string Url;
        /// <summary>
        /// API Key
        /// </summary>
        public string Key;
        /// <summary>
        /// API Secret Key
        /// </summary>
        public string Secret;
        /// <summary>
        /// Keypair associated nonce value
        /// </summary>
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

        /// <summary>
        /// Create a new wallet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="url"></param>
        /// <param name="nonce"></param>
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
        /// Increase the Nonce of the wallet.
        /// </summary>
        /// <param name="increase"></param>
        public void IncreaseNonce(int increase)
        {
            auth.Nonce += Math.Abs(increase);
        }

        /// <summary>
        /// Return the public API Key for the current Wallet profile
        /// </summary>
        /// <returns></returns>
        public string Key()
        {
            if (auth == null)
                return "No API Keys loaded";
            return auth.Key;
        }

        /// <summary>
        /// Return the base API Url
        /// </summary>
        /// <returns></returns>
        public string Url()
        {
            if (auth == null)
                return "https://api.gemini.com";
            return auth.Url;
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

        /// <summary>
        /// Container for API Keys, and functions for saving/loading key files
        /// </summary>
        public static Wallet Wallet = new Wallet();
        private GeminiClient() { }

        /// <summary>
        /// Return an instance of the GeminiClient
        /// </summary>
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

        /// <summary>
        /// This delegate will be called when the GeminiClient encounters a server side error,
        /// to allow for error handling by the calling application
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="message"></param>
        public delegate void ErrorHandler(string reason, string message);
        private static ErrorHandler Handler;

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
            Requests re = new Requests(Wallet.Url() + "/v1/pubticker/" + currency.ToLower());
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
            Requests re = new Requests(Wallet.Url() + "/v1/symbols");
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
            Wallet.Authenticate(re, new PrivateRequest { Nonce = 2, Request = "/v1/balances" });
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
            Wallet.Authenticate(re, new DepositAddressRequest { Label = label, Request = request });

            var result = re.Post().Result;
            if (result.IsSuccessStatusCode)
                return result.Json<DepositAddress>().Address;
            HandleError(result);
            return null;
        }

        /// <summary>
        /// Withdraw cryptocurrency funds to a whitelisted address
        /// </summary>
        /// <param name="currency">"btc", "eth"</param>
        /// <param name="address">Cryptocurrency address</param>
        /// <param name="amount">Amount to withdraw</param>
        /// <returns></returns>
        public static string Withdraw(string currency, string address, string amount)
        {
            Requests re = new Requests();
            Wallet.Authenticate(re, new WithdrawalRequest
            {
                Request = String.Format("/v1/withdraw/{0}", currency),
                Address = address,
                Amount = amount,
            });

            var result = re.Post().Result;
            if (result.IsSuccessStatusCode)
                return result.Json<WithdrawalResponse>().TxHash;
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
            Wallet.Authenticate(re, order);

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
            Wallet.Authenticate(re, new OrderStatusRequest() { Request = "/v1/order/status", OrderID = order_id });

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
        public static OrderStatus[] GetActiveOrders()
        {
            Requests re = new Requests();
            Wallet.Authenticate(re, new OrderStatusRequest() { Request = "/v1/orders" });

            var status = re.Post().Result;
            if (status.IsSuccessStatusCode)
                return status.Json<OrderStatus[]>();
            HandleError(status);
            return null;
        }

        /// <summary>
        /// Retrieve trade history. See the Gemini API docs for more information on using
        /// this function.
        /// </summary>
        /// <param name="symbol">The symbol to retrieve trades for</param>
        /// <param name="number">Optional. The maximum number of trades to return. Default is 50, max is 500.</param>
        /// <param name="timestamp">Optional. Only return trades after this timestamp.</param>
        /// <returns></returns>
        public static PastTrade[] GetPastTrades(string symbol, int number = 50, long timestamp = 0)
        {
            Requests re = new Requests();
            Wallet.Authenticate(re, new PastTradeRequest
            {
                Request = "/v1/mytrades",
                Symbol = symbol,
                LimitTrades = number,
                Timestamp = timestamp,
            });

            var status = re.Post().Result;
            if (status.IsSuccessStatusCode)
                return status.Json<PastTrade[]>();
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
            Wallet.Authenticate(re, new OrderStatusRequest() { Request = "/v1/order/cancel", OrderID = order_id });

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
            Wallet.Authenticate(re, new PrivateRequest { Request = "/v1/order/cancel/session" });
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
            Wallet.Authenticate(re, new PrivateRequest { Request = "/v1/order/cancel/all" });

            var res = re.Post().Result;
            if (res.IsSuccessStatusCode)
                return true;

            HandleError(res);
            return false;
        }
    }
}
