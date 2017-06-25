using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Gemini.Contracts
{
    [DataContract]
    public class TradeHistory
    {
        [DataMember(Name = "timestamp")]
        public string Timestamp;
        [DataMember(Name = "timestampms")]
        public string TimestampMs;
        [DataMember(Name = "tid")]
        public int TradeId;
        [DataMember(Name = "price")]
        public decimal Price;
        [DataMember(Name = "exchange")]
        public string Exchange;
        [DataMember(Name = "type")]
        public string Type;
        [DataMember(Name = "broken")]
        public string Broken;
    }

    [DataContract]
    public class Volume
    {
        [DataMember(Order = 0)]
        public decimal Currency;
        [DataMember(Name = "USD")]
        public decimal USD;
        [DataMember(Name = "timestamp")]
        public string timestamp;
    }


    [DataContract]
    public class Ticker
    {
        [DataMember(Name = "bid")]
        public decimal Bid;
        [DataMember(Name = "ask")]
        public decimal Ask;
        [DataMember(Name = "last")]
        public decimal Last;
        [DataMember(Name = "volume")]
        public Volume Volume;

    }

    [DataContract]
    public class Order
    {
        [DataMember(Name = "price")]
        public float Price;
        [DataMember(Name = "amount")]
        public float Amount;
    }

    [DataContract]
    public class OrderBook
    {
        [DataMember(Name = "bids")]
        public Order[] Bids;
        [DataMember(Name = "asks")]
        public Order[] Asks;
    }

    [DataContract]
    public abstract class PrivateRequest
    {
        [DataMember(Name = "request")]
        public string Request;
        [DataMember(Name = "nonce")]
        public int Nonce;
    }

    [DataContract]
    public class BalanceRequest : PrivateRequest
    {
        [DataMember(Name = "currency")]
        public string Currency;
        [DataMember(Name = "amount")]
        public decimal Amount;
        [DataMember(Name = "available")]
        public decimal Available;
        [DataMember(Name = "availableForWithdrawal")]
        public decimal AvailableForWithdrawal;
    }

    /* request: /v1/deposit/$CURRENCY/newAddress */
    [DataContract]
    public class DepositAddressRequest : PrivateRequest
    {
        /* Optional Label for deposit address */
        [DataMember(Name = "label")]
        public string Label;
    }

    /* response */
    [DataContract]
    public class DepositAddress
    {
        [DataMember(Name = "currency")]
        public string Currency;
        [DataMember(Name = "address")]
        public string Address;
        [DataMember(Name = "label")]
        public string Label;
    }

    [DataContract]
    public class NewOrderRequest : PrivateRequest
    {
        /// <summary>
        /// client defined order id.
        /// </summary>
        [DataMember(Name = "client_order_id")]
        public string ClientOrderID;
        [DataMember(Name = "symbol")]
        public string Symbol;
        [DataMember(Name = "amount")]
        public string Amount;
        [DataMember(Name = "price")]
        public string Price;

        /// <summary>
        /// "buy" or "sell"
        /// </summary>
        [DataMember(Name = "side")]
        public string Side;

        /// <summary>
        /// only "exchange limit" is supported
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;

        /// <summary>
        /// Array including any of the following:
        /// "maker-or-cancel"
        /// "immediate-or-cancel"
        /// "auction-only"
        /// </summary>
        [DataMember(Name = "options", IsRequired = false)]
        public string[] Options;
    }

    [DataContract]
    public class OrderStatusRequest : PrivateRequest
    {
        [DataMember(Name = "order_id")]
        public int OrderID;
    }

    [DataContract]
    public class OrderStatus
    {
        /// <summary>
        /// server defined order id.
        /// </summary>
        [DataMember(Name = "order_id")]
        public string OrderID;

        /// <summary>
        /// client defined order id.
        /// </summary>
        [DataMember(Name = "client_order_id")]
        public string ClientOrderID;

        [DataMember(Name = "symbol")]
        public string Symbol;
        /// <summary>
        /// "gemini"
        /// </summary>
        [DataMember(Name = "exchange")]
        public string Exchange;

        /// <summary>
        /// Price the order was issued at
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price;

        /// <summary>
        /// Average price at which the order has been executed so far
        /// </summary>
        [DataMember(Name = "avg_execution_price")]
        public decimal AvgExecutionPrice;

        /// <summary>
        /// "buy" or "sell"
        /// </summary>
        [DataMember(Name = "side")]
        public string Side;

        /// <summary>
        /// "exchange limit", "auction-only", "market buy", "market sell"
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;

        /// <summary>
        /// Array including at most one:
        /// "maker-or-cancel"
        /// "immediate-or-cancel"
        /// "auction-only"
        /// </summary>
        [DataMember(Name = "options")]
        public string Options;

        [DataMember(Name = "timestamp")]
        public string Timestamp;
        [DataMember(Name = "timestampms")]
        public string TimestampMs;
        [DataMember(Name = "is_live")]
        public bool IsLive;
        [DataMember(Name = "is_cancelled")]
        public bool IsCancelled;
        [DataMember(Name = "was_forced")]
        public bool WasForced;
        [DataMember(Name = "executed_amount")]
        public decimal ExecutedAmount;
        [DataMember(Name = "remaining_amount")]
        public decimal RemainingAmount;
        [DataMember(Name = "original_amount")]
        public decimal OriginalAmount;
    }

    [DataContract]
    public class BoolResponse : PrivateRequest
    {
        [DataMember(Name = "result")]
        public bool Result;
    }


    [DataContract]
    public class ErrorCode
    {
        [DataMember(Name = "result")]
        public string Result;
        [DataMember(Name = "reason")]
        public string Reason;
        [DataMember(Name = "message")]
        public string Message;
    }

    public class Serializer<T> where T : class
    {
        private DataContractJsonSerializer json;

        public Serializer()
        {
            if (!typeof(T).IsDefined(typeof(DataContractAttribute), true))
                throw new Exception("Not a valid Data Contract");
            json = new DataContractJsonSerializer(typeof(T));
        }

        public string Write(T obj)
        {
            MemoryStream stream = new MemoryStream();
            json.WriteObject(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }

        public T Read(Stream s)
        {
            return json.ReadObject(s) as T;
        }
    }

}