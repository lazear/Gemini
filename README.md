# Gemini
_Open source C# Wrapper Library for the Gemini Cryptocurrency Exchange REST API._

![alt text](https://winklevosscapital.com/wp-content/uploads/2015/01/gemini-logo-2.png "Gemini Logo")

Examples:

```
// Retrieve balances
var balances = Gemini.GeminiClient.GetBalances();
Console.WriteLine(balances.FirstOrDefault((x) => x.Currency == "BTC").AvailableForWithdrawal);
```
