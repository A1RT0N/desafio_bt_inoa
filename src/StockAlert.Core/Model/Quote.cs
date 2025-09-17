namespace StockAlert.Core.Model;

public readonly record struct Quote(string Symbol, decimal Price, DateTimeOffset Time);
