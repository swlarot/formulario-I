using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FormularioL.Services;

public sealed class EmailService : IEmailService
{
    private readonly EmailOptions _opt;
    private readonly ILogger<EmailService> _log;

    public EmailService(IOptions<EmailOptions> opt, ILogger<EmailService> log)
    {
        _opt = opt.Value;
        _log = log;
    }

    public async Task SendConocimientoClienteAsync(Models.ConocimientoClienteModel m)
    {
        // Honeypot: si se llenó, ignoramos (probable bot)
        if (!string.IsNullOrEmpty(m.CodigoInterno))
        {
            _log.LogWarning("Formulario descartado por honeypot.");
            return;
        }

        // Validaciones mínimas
        if (string.IsNullOrWhiteSpace(_opt.To))
            throw new InvalidOperationException("Email:To no está configurado.");
        if (string.IsNullOrWhiteSpace(_opt.Smtp.Host))
            throw new InvalidOperationException("Email:Smtp:Host no está configurado.");
        if (_opt.Smtp.Port <= 0)
            throw new InvalidOperationException("Email:Smtp:Port inválido.");
        if (string.IsNullOrWhiteSpace(_opt.Smtp.User))
            throw new InvalidOperationException("Email:Smtp:User no está configurado.");
        if (string.IsNullOrWhiteSpace(_opt.Smtp.Password))
            throw new InvalidOperationException("Email:Smtp:Password no está configurado.");

        var from = _opt.From ?? _opt.Smtp.User; // muchos servidores exigen From==User o mismo dominio

        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(from));
        msg.To.Add(MailboxAddress.Parse(_opt.To));
        msg.Subject = $"Conocimiento del Cliente - {m.NombrePH}";

        var body = $@"
Nombre PH: {m.NombrePH}
RUC: {m.RUC} - DV: {m.DV}
Correo propuesta: {m.CorreoPropuesta}
Dirección: {m.Direccion}
Unidades: {m.Unidades} | Torres: {m.Torres}
Cuota USD/mes: {m.CuotaMantenimientoMes} | Gastos USD/mes: {m.GastosMantenimientoMes}
Empleados planilla: {m.EmpleadosPlanilla} | Horas extras: {m.ManejaHorasExtras}
Promotora: {m.HayPromotora}
Sistema contable: {m.SistemaContable}
EEFF auditados: {m.EEFFAuditados}
Contacto: {m.Contacto}
Notas: {m.Notas}
";

        var builder = new BodyBuilder
        {
            TextBody = body.Replace("\r\n", "\n"),
            HtmlBody = $"<pre style='font-family:ui-monospace,monospace'>{System.Net.WebUtility.HtmlEncode(body)}</pre>"
        };
        msg.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            // Algunos proveedores con certs auto-firmados en dev:
            // client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            var secure = _opt.Smtp.UseStartTls
                ? SecureSocketOptions.StartTls           // 587
                : SecureSocketOptions.SslOnConnect;      // 465

            client.CheckCertificateRevocation = false;
            client.AuthenticationMechanisms.Remove("XOAUTH2"); // forzar user/pass

            _log.LogInformation("Conectando a {Host}:{Port} (StartTLS={StartTls})", _opt.Smtp.Host, _opt.Smtp.Port, _opt.Smtp.UseStartTls);
            await client.ConnectAsync(_opt.Smtp.Host, _opt.Smtp.Port, secure);

            _log.LogInformation("Autenticando como {User}", _opt.Smtp.User);
            await client.AuthenticateAsync(_opt.Smtp.User, _opt.Smtp.Password);

            _log.LogInformation("Enviando correo a {To}", _opt.To);
            await client.SendAsync(msg);

            _log.LogInformation("Correo enviado correctamente.");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Fallo al enviar correo SMTP");
            // Re-lanzamos con mensaje limpio para que lo veas en UI mientras pruebas
            throw new InvalidOperationException($"SMTP error: {ex.Message}", ex);
        }
        finally
        {
            try { await client.DisconnectAsync(true); } catch { /* ignore */ }
        }
    }
}
