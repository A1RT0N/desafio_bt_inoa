using System.Threading;
using System.Threading.Tasks;
using StockAlert.Core.Model;

namespace StockAlert.Core.Abstractions;

public interface IEmailSender
{
    Task SendAsync(AlertEvent evt, CancellationToken ct = default);
}
