using StockAlert.Core.Abstractions;

namespace StockAlert.Tests;

public sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; }
}
