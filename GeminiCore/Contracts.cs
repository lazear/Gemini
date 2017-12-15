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
        /// <summary>
        /// API endpoint string
        /// </summary>
		[DataMember(Name = "request")]
        public string Request;
        /// <summary>
        /// Integer value that must increase between API calls
        /// </summary>
		[DataMember(Name = "nonce")]
        public long Nonce;
    }

    /// <summary>
    /// Executed trade on the exchange.
    /// </summary>
    [DataContract]
    public class TradeHistory
    {
        /// <summary>
        /// Time that the trade was executed
        /// </summary>
        [DataMember(Name = "timestamp")]
        public long Timestamp;
        /// <summary>
        /// The time that the trade was executed in milliseconds
        /// </summary>
        [DataMember(Name = "timestampms")]
        public long TimestampMs;
        /// <summary>
        /// The trade ID number
        /// </summary>
        [DataMember(Name = "tid")]
        public long TradeId;
        /// <summary>
        /// The price the trade was executed at
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price;
        /// <summary>
        /// The amount that was traded
        /// </summary>
        [DataMember(Name = "amount")]
        public decimal Amount;
        /// <summary>
        /// Will always be "gemini"
        /// </summary>
        [DataMember(Name = "exchange")]
        public string Exchange;
        /// <summary>
        /// "buy": an ask was removed by an incoming buy order
        /// "sell": a bid was removed by an incoming sell order
        /// "auction": bulk trade
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;
        /// <summary>
        /// Whether the trade was broken or not. Broken trades will not be displayed by default; use the include_breaks to display them.
        /// </summary>
        [DataMember(Name = "broken")]
        public string Broken;
    }

    /// <summary>
    /// Volume object 
    /// </summary>
	[DataContract]
    public class Volume
    {
        /// <summary>
        /// Supported currency
        /// </summary>
		[DataMember(Order = 0)]
        public decimal Currency1;
        /// <summary>
        /// Value in USD
        /// </summary>
		[DataMember(Order = 1)]
        public decimal Currency2;
        /// <summary>
        /// Epoch time in seconds
        /// </summary>
		[DataMember(Name = "timestamp")]
        public string timestamp;
    }


    /// <summary>
    /// Retrieve data for each supported currency
    /// </summary>
    [DataContract]
    public class Ticker
    {
        /// <summary>
        /// Highest bid price
        /// </summary>
		[DataMember(Name = "bid")]
        public decimal Bid;
        /// <summary>
        /// Lowest ask price
        /// </summary>
		[DataMember(Name = "ask")]
        public decimal Ask;
        /// <summary>
        /// Price of last trade
        /// </summary>
		[DataMember(Name = "last")]
        public decimal Last;
        /// <summary>
        /// 24hr volume
        /// </summary>
		[DataMember(Name = "volume")]
        public Volume Volume;

    }

    /// <summary>
    /// Represents an individual order in the order book
    /// </summary>
    [DataContract]
    public class OrderBookEntry
    {
        /// <summary>
        /// Price
        /// </summary>
		[DataMember(Name = "price")]
        public float Price;
        /// <summary>
        /// Amount
        /// </summary>
		[DataMember(Name = "amount")]
        public float Amount;
    }

    /// <summary>
    /// Represents the current order book
    /// </summary>
    [DataContract]
    public class OrderBook
    {
        /// <summary>
        /// Array of all bids on the order book
        /// </summary>
		[DataMember(Name = "bids")]
        public OrderBookEntry[] Bids;
        /// <summary>
        /// Array of all asks on the order book
        /// </summary>
		[DataMember(Name = "asks")]
        public OrderBookEntry[] Asks;
    }

    /// <summary>
    /// Request the available balances of the account
    /// </summary>
    [DataContract]
    public class BalanceResponse
    {
        /// <summary>
        /// Currency: "BTC", "ETH", "USD"
        /// </summary>
		[DataMember(Name = "currency")]
        public string Currency;
        /// <summary>
        /// Total quantity of the currency held
        /// </summary>
		[DataMember(Name = "amount")]
        public decimal Amount;
        /// <summary>
        /// Quantity available for trading
        /// </summary>
		[DataMember(Name = "available")]
        public decimal Available;
        /// <summary>
        /// Quantity available for withdrawal
        /// </summary>
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
        /// <summary>
        /// Cryptocurrency code: "eth", "btc"
        /// </summary>
        [DataMember(Name = "currency")]
        public string Currency;
        /// <summary>
        /// Cryptocurrency address
        /// </summary>
        [DataMember(Name = "address")]
        public string Address;
        /// <summary>
        /// Optional label for the address
        /// </summary>
        [DataMember(Name = "label")]
        public string Label;
    }

    /// <summary>
    /// Format for placing an order request
    /// </summary>
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
        /// <summary>
        /// Server side order ID
        /// </summary>
		[DataMember(Name = "order_id")]
        public long OrderID;
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
        public long TradeId;
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
    /// Websocket OrderEvent response
    /// </summary>
    [DataContract]
    public class OrderEvent
    {
        /// <summary>
        /// An order event type, e.g. accepted, booked, fill, cancelled
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;

        /// <summary>
        /// The order id that Gemini has assigned to this order, first provided in the accepted or rejected message. All further events (fill, booked, cancelled, closed) will refer to this order_id
        /// </summary>
        [DataMember(Name = "order_id")]
        public string OrderID;

        /// <summary>
        /// The event id associated with this specific order event. event_id is supplied for every order event type except the initial events that supply the active orders when you initially subscribe.
        /// </summary>
        [DataMember(Name = "event_id")]
        public string EventID;

        /// <summary>
        /// The API session key associated with this order.
        /// </summary>
        [DataMember(Name = "api_session")]
        public string ApiSession;

        /// <summary>
        /// The optional client-specified order id
        /// </summary>
        [DataMember(Name = "client_order_id")]
        public string ClientOrderID;

        /// <summary>
        /// The symbol of the order
        /// </summary>
        [DataMember(Name = "symbol")]
        public string Symbol;

        /// <summary>
        /// Either buy or sell
        /// </summary>
        [DataMember(Name = "side")]
        public string Side;

        /// <summary>
        /// When limit orders are placed with immediate-or-cancel or maker-or-cancel behavior, this field will indicate the order behavior.
        /// </summary>
        [DataMember(Name = "behavior")]
        public string Behavior;

        /// <summary>
        /// Description of the order
        /// </summary>
        [DataMember(Name = "order_type")]
        public string OrderType;

        /// <summary>
        /// The timestamp the order was submitted. Note that for compatibility reasons, this is returned as a string. See the Difference From Bitfinex section below. We recommend using the timestampms field instead.
        /// </summary>
        [DataMember(Name = "timestamp")]
        public string Timestamp;

        /// <summary>
        /// The timestamp the order was submitted in milliseconds.
        /// </summary>
        [DataMember(Name = "timestampms")]
        public long TimestampMs;

        /// <summary>
        /// true if the order is active on the book (has remaining quantity and has not been canceled)
        /// </summary>
        [DataMember(Name = "is_live")]
        public bool IsLive;

        /// <summary>
        /// true if the order has been canceled. Note the spelling, "cancelled" instead of "canceled". This is for compatibility reasons.
        /// </summary>
        [DataMember(Name = "is_cancelled")]
        public bool IsCancelled;

        /// <summary>
        /// true if the order is active but not visible (for instance, an auction-only order)
        /// </summary>
        [DataMember(Name = "is_hidden")]
        public bool IsHidden;

        /// <summary>
        /// The average price at which this order as been executed so far. 0 if the order has not been executed at all.
        /// </summary>
        [DataMember(Name = "avg_execution_price")]
        public decimal AvgExecutionPrice;

        /// <summary>
        /// The amount of the order that has been filled.
        /// </summary>
        [DataMember(Name = "executed_amount")]
        public decimal ExecutedAmount;

        /// <summary>
        /// The amount of the order that has not been filled. Present for limit and market sell orders; absent for market buy orders.
        /// </summary>
        [DataMember(Name = "remaining_amount")]
        public decimal RemainingAmount;

        /// <summary>
        /// For limit orders and market sells, the quantity the order was placed for. Not present for market buys.
        /// </summary>
        [DataMember(Name = "original_amount")]
        public decimal OriginalAmount;

        /// <summary>
        /// For limit orders, the price the order was placed for. Not present for market buys and sells.
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price;

        /// <summary>
        /// For market buys, the total spend (fee-inclusive notional value) the order was placed for. Not present for limit orders and market sells.
        /// </summary>
        [DataMember(Name = "total_spend")]
        public decimal TotalSpend;
    }

    /// <summary>
    /// Websocket subscription acknowledgement
    /// </summary>
    [DataContract]
    public class OrderEventSubscriptionAck
    {
        /// <summary>
        /// subscription_ack
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;

        /// <summary>
        /// The account id associated with the API session key you supplied in your X-GEMINI-APIKEY header. See Private API Invocation for more details.
        /// </summary>
        [DataMember(Name = "accountId")]
        public long AccountID;

        /// <summary>
        /// The id associated with this websocket subscription; the component after the last dash is a request trace id that will be echoed back in the heartbeat traceId field.
        /// </summary>
        [DataMember(Name = "subscriptionId")]
        public string SubscriptionID;

        /// <summary>
        /// An array of zero or more supported symbols. An empty array means your subscription is not filtered by symbol.
        /// </summary>
        [DataMember(Name = "symbolFilter")]
        public string[] SymbolFilter;

        /// <summary>
        /// An array of zero or more API session keys associated with your account. The string "UI" means you want to see orders placed by your website users. An empty array means you want to see all orders on your account, regardless of whether they were placed via the API or the website.
        /// </summary>
        [DataMember(Name = "apiSessionFilter")]
        public string[] ApiSessionFilter;

        /// <summary>
        /// An array of zero or more order event types. An empty array means your subscription is not filtered by event type.
        /// </summary>
        [DataMember(Name = "eventTypeFilter")]
        public string[] EventTypeFilter;
    }

    /// <summary>
    /// Websocket heartbeat
    /// </summary>
    [DataContract]
    public class Heartbeat
    {
        /// <summary>
        /// heartbeat
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;
        /// <summary>
        /// Gemini adds a timestamp so if you get disconnected, you may contact Gemini support with the timestamp of the last heartbeat you received.
        /// </summary>
        [DataMember(Name = "timestampms")]
        public long TimestampMs;
        /// <summary>
        /// Gemini adds a monotonically incrementing sequence to make it easy to tell if you've missed a heartbeat.
        /// </summary>
        [DataMember(Name = "sequence")]
        public long Sequence;
        /// <summary>
        /// Gemini adds a trace id to each WebSocket request to make troubleshooting
        /// </summary>
        [DataMember(Name = "trace_id")]
        public string TraceID;
    }

    /// <summary>
    /// Fill data
    /// </summary>
    [DataContract]
    public class OrderEventFillData
    {
        /// <summary>
        /// the event id the order was filled at
        /// </summary>
        [DataMember(Name = "trade_id")]
        public string TradeID;
        /// <summary>
        /// whether this side of the trade represents Maker, Taker, or Auction liquidity
        /// </summary>
        [DataMember(Name = "liquidity")]
        public string Liquidity;
        /// <summary>
        /// the price the trade filled at
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price;
        /// <summary>
        /// the amount of the trade fill
        /// </summary>
        [DataMember(Name = "amount")]
        public decimal Amount;
        /// <summary>
        /// the fee associated with this side of the trade
        /// </summary>
        [DataMember(Name = "fee")]
        public decimal Fee;
        /// <summary>
        /// the three-letter code of the currency associated with the fee
        /// </summary>
        [DataMember(Name = "fee_currency")]
        public string FeeCurrency;
    }

    /// <summary>
    /// Response when an order is filled. type = "fill"
    /// </summary>
    [DataContract]
    public class OrderEventFilled : OrderEvent
    {
        /// <summary>
        /// Data about the filled order
        /// </summary>
        [DataMember(Name = "fill", IsRequired = false)]
        public OrderEventFillData Fill;
    }

    /// <summary>
    /// Response when an order is cancelled. type = "cancelled"
    /// </summary>
    [DataContract]
    public class OrderEventCancelled : OrderEvent
    {
        /// <summary>
        /// The event id of the command to cancel your order.
        /// </summary>
        [DataMember(Name = "cancel_command_id", IsRequired = false)]
        public string CancelCommandID;
        /// <summary>
        /// When possible, Gemini will supply the reason your order was cancelled.
        /// </summary>
        [DataMember(Name = "reason", IsRequired = false)]
        public string Reason;
    }

    /// <summary>
    /// Trade event
    /// </summary>
    [DataContract]
    public class MarketDataEvent
    {
        /// <summary>
        /// trade, change, or auction.
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;
        /// <summary>
        /// The price this trade executed at.
        /// </summary>
        [DataMember(Name = "price")]
        public decimal Price;
        /// <summary>
        /// The amount traded.
        /// </summary>
        [DataMember(Name = "amount")]
        public decimal Amount;
        /// <summary>
        /// The side of the book the maker of the trade placed their order on, of if the trade occurred in an auction. Either bid, ask, or auction.
        /// </summary>
        [DataMember(Name = "makerSide")]
        public string MakerSide;
        /* For CHANGE events */
        /// <summary>
        /// Either bid or ask.
        /// </summary>
        [DataMember(Name = "side")]
        public string Side;
        /// <summary>
        /// Either place, trade, cancel, or initial, to indicate why the change has occurred. initial is for the initial response message, which will show the entire existing state of the order book.
        /// </summary>
        [DataMember(Name = "reason")]
        public string Reason;
        /// <summary>
        /// The quantity remaining at that price level after this change occurred. May be zero if all orders at this price level have been filled or canceled.
        /// </summary>
        [DataMember(Name = "remaining")]
        public decimal Remaining;
        /// <summary>
        /// The quantity changed. May be negative, if an order is filled or canceled. For initial messages, delta will equal remaining.
        /// </summary>
        [DataMember(Name = "delta")]
        public decimal Delta;
    }

    /// <summary>
    /// Response object from market data Websocket 
    /// </summary>
	[DataContract]
    public class MarketData
    {
        /// <summary>
        /// heartbeat or update.
        /// </summary>
        [DataMember(Name = "type")]
        public string Type;
        /// <summary>
        /// A monotonically increasing sequence number indicating when this change occurred. These numbers are persistent and consistent between market data connections.
        /// </summary>
        [DataMember(Name = "eventId")]
        public long Eventid;
        /// <summary>
        /// Either a change to the order book, or the indication that a trade has occurred.
        /// </summary>
        [DataMember(Name = "events")]
        public MarketDataEvent[] Events;
    }


    /// <summary>
    /// Response on error
    /// </summary>
    [DataContract]
    public class ErrorCode
    {
        /// <summary>
        /// "Error"
        /// </summary>
		[DataMember(Name = "result")]
        public string Result;
        /// <summary>
        /// Reason for the error
        /// </summary>
		[DataMember(Name = "reason")]
        public string Reason;
        /// <summary>
        /// Human readable message
        /// </summary>
		[DataMember(Name = "message")]
        public string Message;
    }

    /// <summary>
    /// Wrapper around Json serializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class Serializer<T> where T : class
    {
        private DataContractJsonSerializer json;

        /// <summary>
        /// Initialize new serializer
        /// </summary>
		public Serializer()
        {
            if (!typeof(T).IsDefined(typeof(DataContractAttribute), true))
                throw new Exception("Not a valid Data Contract");
            json = new DataContractJsonSerializer(typeof(T));
        }

        /// <summary>
        /// Write an object to a string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public string Write(T obj)
        {
            MemoryStream stream = new MemoryStream();
            json.WriteObject(stream, obj);
            stream.Seek(0, SeekOrigin.Begin);
            return new StreamReader(stream).ReadToEnd();
        }

        /// <summary>
        /// Read an object from a stream
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
		public T Read(Stream s)
        {
            return json.ReadObject(s) as T;
        }
    }

}
