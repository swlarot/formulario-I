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
        // 1) Honeypot: si se llenó, ignoramos (probable bot)
        if (!string.IsNullOrEmpty(m.CodigoInterno))
        {
            _log.LogWarning("Formulario descartado por honeypot.");
            return;
        }

        // 2) Validaciones mínimas de config
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

        var from = _opt.From ?? _opt.Smtp.User; // De preferencia From = ayuda@clau.com.pa
        var ahora = DateTimeOffset.Now;

        // 3) Construcción del mensaje principal (interno a ayuda)
        var msg = new MimeMessage();

        // Nombre visible del remitente (branding) y remitente SMTP real
        msg.From.Clear();
        msg.From.Add(new MailboxAddress("CLAU – Ayuda", from));
        msg.Sender = MailboxAddress.Parse(_opt.Smtp.User); // cuenta autenticada (gmail)

        msg.To.Add(MailboxAddress.Parse(_opt.To));

        // Si el solicitante puso correo, usarlo como Reply-To (responder le escribe a él)
        if (!string.IsNullOrWhiteSpace(m.CorreoPropuesta))
            msg.ReplyTo.Add(MailboxAddress.Parse(m.CorreoPropuesta));

        msg.Subject = $"Conocimiento del Cliente - {m.NombrePH} ({ahora:yyyy-MM-dd HH:mm})";

        // Texto plano (fallback)
        var textBody = $@"
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
".Replace("\r\n", "\n").Trim();

        // HTML bonito en tabla
        string E(string? s) => System.Net.WebUtility.HtmlEncode(s ?? "");
        var htmlBody = $@"
<style>
table{{border-collapse:collapse;font-family:system-ui,Segoe UI,Arial,sans-serif}}
td,th{{border:1px solid #e5e7eb;padding:8px 10px;text-align:left;font-size:14px}}
th{{background:#111827;color:#fff}}
h2{{font:600 18px system-ui;margin:0 0 10px}}
small{{color:#6b7280}}
</style>
<h2>Conocimiento del Cliente</h2>
<small>Recibido: {ahora:yyyy-MM-dd HH:mm zzz}</small>
<table>
<tr><th>Campo</th><th>Valor</th></tr>
<tr><td>Nombre PH</td><td>{E(m.NombrePH)}</td></tr>
<tr><td>RUC / DV</td><td>{E(m.RUC)} / {E(m.DV)}</td></tr>
<tr><td>Correo propuesta</td><td>{E(m.CorreoPropuesta)}</td></tr>
<tr><td>Dirección</td><td>{E(m.Direccion)}</td></tr>
<tr><td>Unidades / Torres</td><td>{m.Unidades} / {m.Torres}</td></tr>
<tr><td>Cuota USD/mes</td><td>{m.CuotaMantenimientoMes}</td></tr>
<tr><td>Gastos USD/mes</td><td>{m.GastosMantenimientoMes}</td></tr>
<tr><td>Empleados / Horas extra</td><td>{m.EmpleadosPlanilla} / {m.ManejaHorasExtras}</td></tr>
<tr><td>Promotora</td><td>{m.HayPromotora}</td></tr>
<tr><td>Sistema contable</td><td>{E(m.SistemaContable)}</td></tr>
<tr><td>EEFF auditados</td><td>{m.EEFFAuditados}</td></tr>
<tr><td>Contacto</td><td>{E(m.Contacto)}</td></tr>
<tr><td>Notas</td><td>{E(m.Notas)}</td></tr>
</table>";

        var builder = new BodyBuilder
        {
            TextBody = textBody,
            HtmlBody = htmlBody
        };
        msg.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        try
        {
            var secure = _opt.Smtp.UseStartTls
                ? SecureSocketOptions.StartTls       // 587
                : SecureSocketOptions.SslOnConnect;  // 465

            client.CheckCertificateRevocation = false;
            client.AuthenticationMechanisms.Remove("XOAUTH2"); // forzar user/pass

            _log.LogInformation("Conectando a {Host}:{Port} (StartTLS={StartTls})", _opt.Smtp.Host, _opt.Smtp.Port, _opt.Smtp.UseStartTls);
            await client.ConnectAsync(_opt.Smtp.Host, _opt.Smtp.Port, secure);

            _log.LogInformation("Autenticando como {User}", _opt.Smtp.User);
            await client.AuthenticateAsync(_opt.Smtp.User, _opt.Smtp.Password);

            _log.LogInformation("Enviando correo a {To}", _opt.To);
            await client.SendAsync(msg);

            // 4) Acuse al solicitante (si dejó correo)
            if (!string.IsNullOrWhiteSpace(m.CorreoPropuesta))
            {
                var ack = new MimeMessage();

                // Misma identidad visible y remitente SMTP real
                ack.From.Clear();
                ack.From.Add(new MailboxAddress("CLAU – Ayuda", from));
                ack.Sender = MailboxAddress.Parse(_opt.Smtp.User);

                ack.To.Add(MailboxAddress.Parse(m.CorreoPropuesta!));

                // Que las respuestas del acuse vayan a ayuda
                ack.ReplyTo.Clear();
                ack.ReplyTo.Add(MailboxAddress.Parse(_opt.To!));

                ack.Subject = $"Recibido: Conocimiento del Cliente - {m.NombrePH}";
                ack.Body = new BodyBuilder
                {
                    TextBody = $"Hola,\n\nHemos recibido tu información para el P.H. {m.NombrePH}. Pronto te contactaremos.\n\nSaludos,\nEquipo CLAU",
                    HtmlBody = $"<p>Hola,</p><p>Hemos recibido tu información para el P.H. <b>{E(m.NombrePH)}</b>. Pronto te contactaremos.</p><p>Saludos,<br/>Equipo CLAU</p>"
                }.ToMessageBody();

                _log.LogInformation("Enviando acuse a {Correo}", m.CorreoPropuesta);
                await client.SendAsync(ack);
            }

            _log.LogInformation("Correos enviados correctamente.");
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Fallo al enviar correo SMTP");
            throw new InvalidOperationException($"SMTP error: {ex.Message}", ex);
        }
        finally
        {
            try { await client.DisconnectAsync(true); } catch { /* ignore */ }
        }
    }
}
