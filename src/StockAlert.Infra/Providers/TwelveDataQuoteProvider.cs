using System.Net.Http.Json;
using StockAlert.Core.Abstractions;
using StockAlert.Core.Model;
using System.Globalization;  


namespace StockAlert.Infra.Providers;

public sealed class TwelveDataQuoteProvider : IQuoteProvider
{
    private readonly string _apiKey;
    private static readonly HttpClient _http = new();

    public TwelveDataQuoteProvider(string apiKey)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
    }

    public async Task<Quote> GetQuoteAsync(string symbol, CancellationToken ct = default)
    {
        var url = $"https://api.twelvedata.com/price?symbol={symbol}&apikey={_apiKey}";
        var resp = await _http.GetFromJsonAsync<PriceResponse>(url, ct)
                ?? throw new InvalidOperationException("Resposta vazia da TwelveData");

        if (!decimal.TryParse(resp.price, CultureInfo.InvariantCulture, out var price))
            throw new InvalidOperationException($"Preço inválido: {resp.price}");

        return new Quote(symbol, price, DateTimeOffset.UtcNow);
    }


    private sealed class PriceResponse
    {
        public string price { get; set; } = "0";
    }
}
