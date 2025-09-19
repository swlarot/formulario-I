// Services/EmailService.cs
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using FormularioL.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FormularioL.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _opt;
        private readonly ILogger<EmailService> _log;

        public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> log)
        {
            _opt = options.Value;
            _log = log;
        }

        // -------------------- PH (existente) --------------------
        public Task SendConocimientoClienteAsync(ConocimientoClienteModel m)
            => SendConocimientoClienteAsync(m, "ph", default);

        public async Task SendConocimientoClienteAsync(ConocimientoClienteModel m, string entidadSlug, CancellationToken ct)
        {
            string entidadKey = (entidadSlug ?? "ph").Trim().ToLowerInvariant();
            string entidadUp = entidadKey.ToUpperInvariant();
            string N2(decimal d) => d.ToString("N2", CultureInfo.CurrentCulture);

            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_opt.From));
            msg.To.Add(MailboxAddress.Parse(GetRoutedTo(entidadKey)));
            if (!string.IsNullOrWhiteSpace(m.CorreoPropuesta))
                msg.ReplyTo.Add(MailboxAddress.Parse(m.CorreoPropuesta));
            msg.Subject = $"Conocimiento – {entidadUp} – {m.NombrePH}";

            var html = $@"
<h2>Conocimiento del Cliente — {entidadUp}</h2>
<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;'>
<tr><td><b>Nombre del P.H.</b></td><td>{m.NombrePH}</td></tr>
<tr><td><b>RUC</b></td><td>{m.RUC}-{m.DV}</td></tr>
<tr><td><b>Dirección</b></td><td>{m.Direccion}</td></tr>
<tr><td><b>Correo propuesta</b></td><td>{m.CorreoPropuesta}</td></tr>
<tr><td><b>Unidades</b></td><td>{m.Unidades}</td></tr>
<tr><td><b>Torres</b></td><td>{m.Torres}</td></tr>
<tr><td><b>Cuota mantenimiento (USD/mes)</b></td><td>{N2(m.CuotaMantenimientoMes)}</td></tr>
<tr><td><b>Gastos mantenimiento (USD/mes)</b></td><td>{N2(m.GastosMantenimientoMes)}</td></tr>
<tr><td><b>Empleados planilla</b></td><td>{m.EmpleadosPlanilla}</td></tr>
<tr><td><b>¿Horas extras?</b></td><td>{(m.ManejaHorasExtras ? "Sí" : "No")}</td></tr>
<tr><td><b>¿Promotora?</b></td><td>{(m.HayPromotora ? "Sí" : "No")}</td></tr>
<tr><td><b>Sistema contable</b></td><td>{m.SistemaContable}</td></tr>
<tr><td><b>EEFF auditados (año previo)</b></td><td>{(m.EEFFAuditados ? "Sí" : "No")}</td></tr>
<tr><td><b>Contacto</b></td><td>{m.Contacto}</td></tr>
<tr><td><b>Notas</b></td><td>{m.Notas}</td></tr>
</table>
<p style='color:#888; margin-top:8px'>*No enviar contraseñas por este medio.</p>";

            msg.Body = new BodyBuilder { HtmlBody = html, TextBody = StripHtml(html) }.ToMessageBody();
            await SendAsync(msg, ct);
            await SendAckIfAppliesAsync(m.CorreoPropuesta, ct);
        }

        // -------------------- SA (nuevo) --------------------
        public async Task SendSaAsync(SaFormModel m, CancellationToken ct = default)
        {
            const string entidadKey = "sa";
            string N2(decimal d) => d.ToString("N2", CultureInfo.CurrentCulture);

            var msg = new MimeMessage();
            msg.From.Add(MailboxAddress.Parse(_opt.From));
            msg.To.Add(MailboxAddress.Parse(GetRoutedTo(entidadKey)));
            if (!string.IsNullOrWhiteSpace(m.CorreoPropuesta))
                msg.ReplyTo.Add(MailboxAddress.Parse(m.CorreoPropuesta));
            msg.Subject = $"Conocimiento – SA – {m.RazonSocial}";

            // HTML específico para SA
            var html = $@"
<h2>Conocimiento del Cliente — SA</h2>
<table border='1' cellpadding='6' cellspacing='0' style='border-collapse:collapse;'>
<tr><td><b>Razón social</b></td><td>{m.RazonSocial}</td></tr>
<tr><td><b>RUC</b></td><td>{m.RUC}-{m.DV}</td></tr>
<tr><td><b>Dirección</b></td><td>{m.Direccion}</td></tr>
<tr><td><b>Correo propuesta</b></td><td>{m.CorreoPropuesta}</td></tr>

<tr><td><b>Actividad económica</b></td><td>{m.ActividadEconomica}</td></tr>
<tr><td><b>Año de constitución</b></td><td>{m.AnioConstitucion}</td></tr>
<tr><td><b>Capital social (USD)</b></td><td>{(m.CapitalSocial is null ? "" : N2(m.CapitalSocial.Value))}</td></tr>
<tr><td><b>N.º accionistas</b></td><td>{m.NumeroAccionistas}</td></tr>
<tr><td><b>Miembros junta/directores</b></td><td>{m.MiembrosJunta}</td></tr>
<tr><td><b>Empleados planilla</b></td><td>{m.EmpleadosPlanilla}</td></tr>
<tr><td><b>Facturación mensual promedio (USD)</b></td><td>{(m.FacturacionMensualPromedio is null ? "" : N2(m.FacturacionMensualPromedio.Value))}</td></tr>
<tr><td><b>N.º cuentas bancarias</b></td><td>{m.NumeroCuentasBancarias}</td></tr>
<tr><td><b>Operaciones en el exterior</b></td><td>{(m.OperacionesExterior ? "Sí" : "No")}</td></tr>
<tr><td><b>EEFF auditados (año previo)</b></td><td>{(m.EEFFAuditados ? "Sí" : "No")}</td></tr>

<tr><td><b>Sistema contable</b></td><td>{m.SistemaContable}</td></tr>
<tr><td><b>Contacto</b></td><td>{m.Contacto}</td></tr>
<tr><td><b>Notas</b></td><td>{m.Notas}</td></tr>
</table>
<p style='color:#888; margin-top:8px'>*No enviar contraseñas por este medio.</p>";

            msg.Body = new BodyBuilder { HtmlBody = html, TextBody = StripHtml(html) }.ToMessageBody();
            await SendAsync(msg, ct);
            await SendAckIfAppliesAsync(m.CorreoPropuesta, ct);
        }

        // -------------------- Helpers comunes --------------------
        private string GetRoutedTo(string entidadKey)
            => (_opt.Routing != null && _opt.Routing.TryGetValue(entidadKey, out var to) && !string.IsNullOrWhiteSpace(to))
                ? to
                : _opt.To;

        private async Task SendAsync(MimeMessage message, CancellationToken ct)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                using var smtp = new SmtpClient { Timeout = _opt.Smtp.TimeoutMs };
                await smtp.ConnectAsync(
                    _opt.Smtp.Host,
                    _opt.Smtp.Port,
                    _opt.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto,
                    ct);
                await smtp.AuthenticateAsync(_opt.Smtp.User, _opt.Smtp.Password, ct);
                await smtp.SendAsync(message, ct);
                await smtp.DisconnectAsync(true, ct);
                _log.LogInformation("Email enviado a {To} en {Elapsed} ms.", string.Join(",", message.To), sw.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error enviando correo.");
                throw;
            }
        }

        private async Task SendAckIfAppliesAsync(string? destinatario, CancellationToken ct)
        {
            if (!_opt.Ack.Enabled || string.IsNullOrWhiteSpace(destinatario)) return;

            var ackFrom = string.IsNullOrWhiteSpace(_opt.Ack.From) ? _opt.From : _opt.Ack.From;
            var ack = new MimeMessage();
            ack.From.Add(MailboxAddress.Parse(ackFrom));
            ack.To.Add(MailboxAddress.Parse(destinatario));
            ack.Subject = _opt.Ack.Subject;
            ack.Body = new BodyBuilder
            {
                HtmlBody = _opt.Ack.HtmlBody,
                TextBody = _opt.Ack.TextBody
            }.ToMessageBody();

            await SendAsync(ack, ct);
        }

        private static string StripHtml(string html) => Regex.Replace(html, "<.*?>", " ").Trim();
    }
}
