using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace Gemini.Contracts
{
    /// <summary>
    /// Private API request that doesn't use additional parameters
    /// </summary>
    [DataContract]
    public class PrivateRequest
    {
        [DataMember(Name = "request")]
        public string Request;
        [DataMember(Name = "nonce")]
        public int Nonce;
    }

    /// <summary>
    /// Trades that have executed since timestamp.
    /// </summary>
    [DataContract]
    public class TradeHistory
    {
        [DataMember(Name = "timestamp")]
        public long Timestamp;
        [DataMember(Name = "timestampms")]
        public long TimestampMs;
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


    /// <summary>
    /// Retrieve data for each supported currency
    /// </summary>
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

    /// <summary>
    /// Represents an individual order in the order book
    /// </summary>
    [DataContract]
    public class OrderBookEntry
    {
        [DataMember(Name = "price")]
        public float Price;
        [DataMember(Name = "amount")]
        public float Amount;
    }

    /// <summary>
    /// Represents the current order book
    /// </summary>
    [DataContract]
    public class OrderBook
    {
        [DataMember(Name = "bids")]
        public OrderBookEntry[] Bids;
        [DataMember(Name = "asks")]
        public OrderBookEntry[] Asks;
    }

    /// <summary>
    /// Request the available balances of the account
    /// </summary>
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

    /// <summary>
    /// Withdraw crypto funds to a whitelisted address
    /// </summary>
    [DataContract]
    public class WithdrawalRequest : PrivateRequest
    {
        /// <summary>
        /// Standard string format of a whitelisted cryptocurrency address
        /// </summary>
        [DataMember(Name = "address")]
        public string Address;

        /// <summary>
        /// Quoted decimal amount to withdraw
        /// </summary>
        [DataMember(Name = "amount")]
        public string Amount;
    }
    
    /// <summary>
    /// Request a cryptocurrency withdrawal to a whitelisted address
    /// </summary>
    [DataContract]
    public class WithdrawalResponse
    {
        /// <summary>
        /// Standard string format of the withdrawal destination address
        /// </summary>
        [DataMember(Name = "destination")]
        public string Destination;
        /// <summary>
        /// The withdrawal amount
        /// </summary>
        [DataMember(Name = "amount")]
        public string Amount;
        /// <summary>
        /// Standard string format of the transaction hash
        /// </summary>
        [DataMember(Name = "txHash")]
        public string TxHash;
    }

    /// <summary>
    /// Create a new cryptocurrency deposit address
    /// </summary>
    [DataContract]
    public class DepositAddressRequest : PrivateRequest
    {
        /// <summary>
        /// Optional label 
        /// </summary>
        [DataMember(Name = "label")]
        public string Label;
    }

    /// <summary>
    /// Response containing new cryptocurrency deposit address
    /// </summary>
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
        /// Client defined order id. Optional, recommended
        /// </summary>
        [DataMember(Name = "client_order_id")]
        public string ClientOrderID;
        /// <summary>
        /// Symbol for the new order
        /// </summary>
        [DataMember(Name = "symbol")]
        public string Symbol;
        /// <summary>
        /// Quoted decimal amount to purchase
        /// </summary>
        [DataMember(Name = "amount")]
        public string Amount;
        /// <summary>
        /// Quoted decimal amount to spend per unit
        /// </summary>
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

    /// <summary>
    /// Get the status for an order
    /// </summary>
    [DataContract]
    public class OrderStatusRequest : PrivateRequest
    {
        [DataMember(Name = "order_id")]
        public int OrderID;
    }

    /// <summary>
    /// Status for an order
    /// </summary>
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
        /// <summary>
        /// Symbol of the order
        /// </summary>
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
        public string[] Options;
        /// <summary>
        /// Timestamp for when the order was submitted, seconds
        /// </summary>
        [DataMember(Name = "timestamp")]
        public string Timestamp;
        /// <summary>
        /// Timestamp, in milliseconds
        /// </summary>
        [DataMember(Name = "timestampms")]
        public long TimestampMs;
        /// <summary>
        /// True is the order is active on the book (remaining quantity, not cancelled)
        /// </summary>
        [DataMember(Name = "is_live")]
        public bool IsLive;
        /// <summary>
        /// True is the order has been cancelled
        /// </summary>
        [DataMember(Name = "is_cancelled")]
        public bool IsCancelled;
        /// <summary>
        /// Will always be false
        /// </summary>
        [DataMember(Name = "was_forced")]
        public bool WasForced;
        /// <summary>
        /// The amount of the order that has been filled
        /// </summary>
        [DataMember(Name = "executed_amount")]
        public decimal ExecutedAmount;
        /// <summary>
        /// The amount of the order that has not been filled
        /// </summary>
        [DataMember(Name = "remaining_amount")]
        public decimal RemainingAmount;
        /// <summary>
        /// The originally submitted amount of the order
        /// </summary>
        [DataMember(Name = "original_amount")]
        public decimal OriginalAmount;
    }

    /// <summary>
    /// Retrieve trade history
    /// </summary>
    [DataContract]
    public class PastTradeRequest : PrivateRequest
    {
        /// <summary>
        /// The symbol to retrieve trades for
        /// </summary>
        [DataMember(Name = "symbol")]
        public string Symbol;
        /// <summary>
        /// The maximum number of trades to return.
        /// Default is 50, Maximum is 500
        /// </summary>
        [DataMember(Name = "limit_trades")]
        public int LimitTrades;
        /// <summary>
        /// Only return trades after this timestamp
        /// </summary>
        [DataMember(Name = "timestamp")]
        public long Timestamp;
    }

    /// <summary>
    /// Information on past trades
    /// </summary>
    [DataContract]
    public class PastTrade
    {
        /// <summary>
        /// Execution price
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price;
        /// <summary>
        /// The quantity executed
        /// </summary>
        [DataMember(Name = "amount")]
        public decimal Amount;
        /// <summary>
        /// Time in epoch seconds
        /// </summary>
        [DataMember(Name = "timestamp")]
        public long Timestamp;
        /// <summary>
        /// Time in epoch milliseconds
        /// </summary>
        [DataMember(Name = "timestampms")]
        public long TimestampMs;
        /// <summary>
        /// Side of the order: "Buy" or "Sell"
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;
        /// <summary>
        /// Whether the order was the taker in the trade
        /// </summary>
        [DataMember(Name = "aggressor")]
        public bool Agressor;
        /// <summary>
        /// Currency that the fee was paid in
        /// </summary>
        [DataMember(Name = "fee_currency")]
        public string FeeCurrency;
        /// <summary>
        /// The amount charged
        /// </summary>
        [DataMember(Name = "fee_amount")]
        public decimal FeeAmount;
        /// <summary>
        /// Uniqued identifier for the trade
        /// </summary>
        [DataMember(Name = "tid")]
        public int TradeId;
        /// <summary>
        /// The order that this trade executed against
        /// </summary>
        [DataMember(Name = "order_id")]
        public string OrderID;
        /// <summary>
        /// client defined order id.
        /// </summary>
        [DataMember(Name = "client_order_id")]
        public string ClientOrderID;
        /// <summary>
        /// Was the trade filled at an auction
        /// </summary>
        [DataMember(Name = "is_auction_fill")]
        public bool Auction;
        /// <summary>
        /// True is the trade is broken
        /// </summary>
        [DataMember(Name = "break", IsRequired = false)]
        public string Broken;
    }


    /// <summary>
    /// Response on error
    /// </summary>
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