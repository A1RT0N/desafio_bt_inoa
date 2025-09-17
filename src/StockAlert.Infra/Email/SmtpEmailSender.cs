using MailKit.Net.Smtp;
using MimeKit;
using StockAlert.Core.Abstractions;
using StockAlert.Core.Model;

namespace StockAlert.Infra.Email;

public sealed class SmtpEmailSender : IEmailSender
{
    private readonly string _fromName, _from, _to, _host, _username, _password;
    private readonly int _port;
    private readonly bool _useSsl;

    public SmtpEmailSender(string fromName, string fromAddress, string toAddress,
        string host, int port, bool useSsl, string? username, string? password)
    {
        _fromName = fromName; _from = fromAddress; _to = toAddress;
        _host = host; _port = port; _useSsl = useSsl;
        _username = username ?? ""; _password = password ?? "";
    }

    public async Task SendAsync(AlertEvent evt, CancellationToken ct = default)
    {
        var msg = new MimeMessage();
        msg.From.Add(new MailboxAddress(_fromName, _from));
        msg.To.Add(MailboxAddress.Parse(_to));
        msg.Subject = evt.Type == AlertType.Buy
            ? $"[COMPRA] {evt.Symbol} {evt.Price}"
            : $"[VENDA] {evt.Symbol} {evt.Price}";
        msg.Body = new TextPart("plain")
        {
            Text =
$@"Alerta: {evt.Type}
Ativo:  {evt.Symbol}
Preço:  {evt.Price}
Hora:   {evt.Time:dd/MM/yyyy HH:mm:ss zzz}

Regras:
- Envia quando preço cruza acima de Sell ou abaixo de Buy.
- Cooldown ativo para evitar spam."
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_host, _port, _useSsl, ct);
        if (!string.IsNullOrEmpty(_username))
            await smtp.AuthenticateAsync(_username, _password, ct);
        await smtp.SendAsync(msg, ct);
        await smtp.DisconnectAsync(true, ct);
    }
}
