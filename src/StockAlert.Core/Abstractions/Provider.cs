using System.Threading;
using System.Threading.Tasks;
using StockAlert.Core.Model;

namespace StockAlert.Core.Abstractions;

public interface IQuoteProvider
{
    Task<Quote> GetQuoteAsync(string symbol, CancellationToken ct = default);
}
