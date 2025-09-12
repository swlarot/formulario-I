using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;

public record EmailOptions
{
    public SmtpOptions Smtp { get; init; } = new();
    public string From { get; init; } = "";
    public string To { get; init; } = ""; // ayuda@clau.com.pa
    public record SmtpOptions
    {
        public string Host { get; init; } = "";
        public int Port { get; init; } = 587;
        public string User { get; init; } = "";
        public string Password { get; init; } = "";
        public bool UseStartTls { get; init; } = true;
    }
}

public interface IEmailService
{
    Task SendConocimientoClienteAsync(ConocimientoClienteModel m);
}

public class EmailService : IEmailService
{
    private readonly EmailOptions _opt;
    public EmailService(IOptions<EmailOptions> options) => _opt = options.Value;

    public async Task SendConocimientoClienteAsync(ConocimientoClienteModel m)
    {
        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(_opt.From));
        msg.To.Add(MailboxAddress.Parse(_opt.To));
        if (!string.IsNullOrWhiteSpace(m.CorreoPropuesta))
            msg.Cc.Add(MailboxAddress.Parse(m.CorreoPropuesta)); // quita si no deseas CC
        msg.Subject = $"Conocimiento del Cliente – {m.NombrePH}";

        var body = $@"
<h2>Conocimiento del Cliente</h2>
<table border='1' cellpadding='6' cellspacing='0'>
<tr><td><b>Nombre del P.H.</b></td><td>{m.NombrePH}</td></tr>
<tr><td><b>RUC</b></td><td>{m.RUC}-{m.DV}</td></tr>
<tr><td><b>Dirección</b></td><td>{m.Direccion}</td></tr>
<tr><td><b>Correo propuesta</b></td><td>{m.CorreoPropuesta}</td></tr>
<tr><td><b>Unidades</b></td><td>{m.Unidades}</td></tr>
<tr><td><b>Torres</b></td><td>{m.Torres}</td></tr>
<tr><td><b>Cuota mantenimiento (USD/mes)</b></td><td>{m.CuotaMantenimientoMes}</td></tr>
<tr><td><b>Gastos mantenimiento (USD/mes)</b></td><td>{m.GastosMantenimientoMes}</td></tr>
<tr><td><b>Empleados planilla</b></td><td>{m.EmpleadosPlanilla}</td></tr>
<tr><td><b>¿Horas extras?</b></td><td>{(m.ManejaHorasExtras ? "Sí" : "No")}</td></tr>
<tr><td><b>¿Promotora?</b></td><td>{(m.HayPromotora ? "Sí" : "No")}</td></tr>
<tr><td><b>Sistema contable</b></td><td>{m.SistemaContable}</td></tr>
<tr><td><b>EEFF auditados (año previo)</b></td><td>{(m.EEFFAuditados ? "Sí" : "No")}</td></tr>
<tr><td><b>Contacto</b></td><td>{m.Contacto}</td></tr>
<tr><td><b>Notas</b></td><td>{m.Notas}</td></tr>
</table>
<p style='color:#888'>*No enviar contraseñas por este medio.</p>";

        msg.Body = new BodyBuilder { HtmlBody = body, TextBody = StripHtml(body) }.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_opt.Smtp.Host, _opt.Smtp.Port,
            _opt.Smtp.UseStartTls ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
        await smtp.AuthenticateAsync(_opt.Smtp.User, _opt.Smtp.Password);
        await smtp.SendAsync(msg);
        await smtp.DisconnectAsync(true);
    }

    private static string StripHtml(string html) =>
        System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", " ");
}
