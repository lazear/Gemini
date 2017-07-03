# Gemini
Open source C# Wrapper Library for the Gemini Cryptocurrency Exchange REST API.

Donate BTC: 12fj5Stp31SrYjtcN9T1jnXPGKUsEF92KD

Examples:

Initialize a new set of API Keys
```c#
using Gemini;

// Create a new wallet file
GeminiClient.Wallet.Create(new GeminiWallet { Key = "x736hasjdgf7t", Secret = "******" }, "mywallet.aes", "hunter1");
// Open the file
GeminiClient.Wallet.Open("mywallet.aes", "hunter1");
```

Account management
```c#
// Retrieve balances
var balances = GeminiClient.GetBalances();
Console.WriteLine(balances.FirstOrDefault((x) => x.Currency == "BTC").AvailableForWithdrawal);

// Retrieve active orders
var orders = GeminiClient.GetActiveOrders();
foreach(OrderStatus order in orders)
  Console.WriteLine(order.IsLive);
```

More indepth examples can be found in the Examples directory