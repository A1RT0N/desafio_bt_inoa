namespace StockAlert.Core.Model;

public enum AlertType { Buy, Sell }

public record AlertEvent(string Symbol, decimal Price, AlertType Type, DateTimeOffset Time)
{
    public override string ToString() =>
        $"{Symbol} {Price} {Type} @ {Time:O}";
}
