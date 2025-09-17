using System.Globalization;
using Microsoft.Extensions.Configuration;
using StockAlert.Core.Abstractions;
using StockAlert.Core.Alert;
using StockAlert.Infra.Email;
using StockAlert.Infra.Providers;

if (args.Length != 3)
{
    Console.WriteLine("Uso: stock-quote-alert <ATIVO> <preco_venda> <preco_compra>");
    return 1;
}

var symbol = args[0].Trim().ToUpperInvariant();
if (!decimal.TryParse(args[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var sellRef) ||
    !decimal.TryParse(args[2], NumberStyles.Any, CultureInfo.InvariantCulture, out var buyRef))
{
    Console.WriteLine("Preços inválidos. Ex.: 22.67 22.59");
    return 2;
}

// carrega config
var cfg = new ConfigurationBuilder()
    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "StockAlert.Infra"))
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

IQuoteProvider provider = new TwelveDataQuoteProvider(cfg["QuoteProvider:TwelveDataApiKey"]!);

IEmailSender emailSender = new SmtpEmailSender(
    fromName: cfg["Email:FromName"]!,
    fromAddress: cfg["Email:FromAddress"]!,
    toAddress: cfg["Email:ToAddress"]!,
    host: cfg["Email:Smtp:Host"]!,
    port: int.Parse(cfg["Email:Smtp:Port"]!),
    useSsl: bool.Parse(cfg["Email:Smtp:UseSsl"]!),
    username: cfg["Email:Smtp:Username"],
    password: cfg["Email:Smtp:Password"]
);

var thresholds = new Thresholds(sellRef, buyRef);
var engine = new Engine(emailSender, new SystemClock(),
    TimeSpan.FromMinutes(int.Parse(cfg["QuoteProvider:CooldownMinutes"] ?? "10")));

Console.WriteLine($"Monitorando {symbol} | Venda > {sellRef} | Compra < {buyRef} (Ctrl+C p/ sair)");
using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

var polling = TimeSpan.FromSeconds(int.Parse(cfg["QuoteProvider:PollingSeconds"] ?? "5"));

while (!cts.IsCancellationRequested)
{
    try
    {
        var quote = await provider.GetQuoteAsync(symbol, cts.Token);
        var evt = engine.Evaluate(thresholds, quote);
        if (evt is not null)
        {
            Console.WriteLine($"{quote.Symbol} {quote.Price} @ {quote.Time:HH:mm:ss} -> {evt.Type}");
            await engine.NotifyAsync(evt!, cts.Token);
        }
        else
        {
            Console.WriteLine($"{quote.Symbol} {quote.Price} @ {quote.Time:HH:mm:ss}");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"[WARN] {ex.Message}");
    }

    await Task.Delay(polling, cts.Token);
}

return 0;
