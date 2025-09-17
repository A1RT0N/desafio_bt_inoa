using StockAlert.Core.Alert;
using StockAlert.Core.Model;
using StockAlert.Core.Abstractions;
using Xunit;


namespace StockAlert.Tests;

internal sealed class DummyEmail : IEmailSender
{
    public int Count { get; private set; }
    public Task SendAsync(AlertEvent evt, CancellationToken ct = default)
    { Count++; return Task.CompletedTask; }
}

public class EngineTest
{
    [Fact]
    public async Task DisparaEmCruzamentoERespeitaCooldown()

    {
        var email = new DummyEmail();
        var clock = new FakeClock { UtcNow = DateTimeOffset.Parse("2025-01-01T10:00:00Z") };
        var engine = new Engine(email, clock, TimeSpan.FromMinutes(10));
        var t = new Thresholds(SellAbove: 10m, BuyBelow: 5m);

        Assert.Null(engine.Evaluate(t, new Quote("XYZ", 7.5m, clock.UtcNow)));

        var evt1 = engine.Evaluate(t, new Quote("XYZ", 10.5m, clock.UtcNow))!;
        Assert.Equal(AlertType.Sell, evt1.Type);

        await engine.NotifyAsync(evt1);
        Assert.Equal(1, email.Count);

        clock.UtcNow = clock.UtcNow.AddMinutes(1);
        Assert.Null(engine.Evaluate(t, new Quote("XYZ", 11m, clock.UtcNow)));

        clock.UtcNow = clock.UtcNow.AddMinutes(10);
        Assert.Null(engine.Evaluate(t, new Quote("XYZ", 9m, clock.UtcNow)));
        var evt2 = engine.Evaluate(t, new Quote("XYZ", 10.2m, clock.UtcNow))!;
        Assert.Equal(AlertType.Sell, evt2.Type);
    }
}
