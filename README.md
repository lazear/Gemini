# Gemini
_Open source C# Wrapper Library for the Gemini Cryptocurrency Exchange REST API._

![alt text](https://winklevosscapital.com/wp-content/uploads/2015/01/gemini-logo-2.png "Gemini Logo")

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
