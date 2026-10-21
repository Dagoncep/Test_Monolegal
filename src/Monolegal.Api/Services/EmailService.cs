using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Monolegal.Api.Settings;

namespace Monolegal.Api.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
    {
        var mensaje = new MimeMessage();
        mensaje.From.Add(new MailboxAddress(_settings.RemitenteNombre, _settings.RemitenteEmail));
        mensaje.To.Add(MailboxAddress.Parse(destinatario));
        mensaje.Subject = asunto;
        mensaje.Body = new TextPart("html") { Text = cuerpoHtml };

        using var smtp = new SmtpClient();
        var seguridad = _settings.UsarTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None;
        await smtp.ConnectAsync(_settings.Host, _settings.Puerto, seguridad);

        if (!string.IsNullOrEmpty(_settings.Usuario))
        {
            await smtp.AuthenticateAsync(_settings.Usuario, _settings.Password);
        }

        await smtp.SendAsync(mensaje);
        await smtp.DisconnectAsync(true);
    }
}
