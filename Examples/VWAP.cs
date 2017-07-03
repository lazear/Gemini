using System;
using Gemini;
using Gemini.Contracts;

namespace VWAP
{
	class Program
	{
		// cumulative VWAP for running calculations
		static decimal PV = 0;
		static decimal V = 0;

		/// <summary>
		/// Calculate the VWAP for a period
		/// </summary>
		/// <param name="start">epoch timestamp for beginning period</param>
		/// <param name="end">epoch timestamp for end period</param>
		public static decimal Vwap(long start, long end)
		{
			long timestamp = start;
			decimal pv = 0;
			decimal v = 0;

			TradeHistory[] history = null;
			do
			{
				var result = Requests.Get(String.Format("https://api.gemini.com/v1/trades/btcusd?limit_trades=500&since={0}", timestamp)).Result;
				if (!result.IsSuccessStatusCode)
					throw new Exception("Bad response");

				history = result.Json<TradeHistory[]>();
				foreach (var trade in history)
				{
					pv += (trade.Price * trade.Amount);
					v += trade.Amount;
				}
				timestamp = history[0].Timestamp;
			} while (history.Length > 1 && (timestamp < end));
			// add to the cumulative variables
			PV += pv;
			V += v;
			return pv / v;
		}


		static bool initial = true;

		/// <summary>
		/// Websocket callback. Whenever data is received, this method will be called
		/// </summary>
		/// <param name="message">Raw data in string format, UTF8 encoded</param>
		/// <param name="state">Optional state object, initialize when the Websocket is created</param>
		static void callback(string message, object state)
		{
			// Initial message contains all of the current market order book
			if (initial)
			{
				initial = false;
				return;
			}
			
			// Serialize the data into a MarketData object
			var e = message.Json<MarketData>();
			if (e.Type == "update")
			{
				foreach (var ev in e.Events)
				{
					// Most of the data will be change events. We're only looking for executed trades
					if (ev.Type == "trade")
					{
						PV += ev.Price * ev.Amount;
						V += ev.Amount;

						Console.WriteLine("Last Price {0} 24hr running VWAP {1}", ev.Price, Math.Round(PV/V, 2));
					}					
				}
			}
		}

		static void Main(string[] args)
		{
	
			Console.WriteLine("24hr VWAP {0}", 
				Math.Round(Vwap(DateTime.UtcNow.Subtract(new TimeSpan(24, 0, 0)).ToTimestamp(), DateTime.UtcNow.ToTimestamp()), 2));

			// Initialize the websocket with a callback
			Gemini.Websocket ws = new Gemini.Websocket("wss://api.gemini.com/v1/marketdata/BTCUSD?heartbeat=true", callback, null);
			ws.Connect();
			while (true) ;
		}
	}
}
