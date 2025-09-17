using StockAlert.Core.Abstractions;
using StockAlert.Core.Model;

namespace StockAlert.Core.Alert;

public sealed class Engine
{
    private readonly IEmailSender _email;
    private readonly IClock _clock;
    private readonly TimeSpan _cooldown;
    private AlertType? _lastZone;           // zona onde o preço está ou entre
    private DateTimeOffset _lastNotified = DateTimeOffset.MinValue;

    public Engine(IEmailSender email, IClock clock, TimeSpan cooldown)
    {
        _email = email; _clock = clock; _cooldown = cooldown;
    }

    public AlertEvent? Evaluate(Thresholds t, Quote q)
    {
        AlertType? zone = q.Price > t.SellAbove ? AlertType.Sell :
                          q.Price < t.BuyBelow ? AlertType.Buy : null;

        // dispara apenas quando ENTRA numa zona diferente 
        if (zone.HasValue && zone != _lastZone)
        {
            var now = _clock.UtcNow;
            if (now - _lastNotified >= _cooldown)
            {
                _lastZone = zone;
                return new AlertEvent(q.Symbol, q.Price, zone.Value, q.Time);
            }
        }

        _lastZone = zone;
        return null;
    }

    public Task NotifyAsync(AlertEvent evt, CancellationToken ct = default)
    {
        _lastNotified = _clock.UtcNow;
        return _email.SendAsync(evt, ct);
    }
}
