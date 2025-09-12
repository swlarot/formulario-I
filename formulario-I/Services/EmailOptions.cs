namespace FormularioL.Services;

public sealed class EmailOptions
{
    public string? From { get; set; }          // remitente (suele ser igual al User)
    public string? To { get; set; }            // destino: ayuda@clau.com.pa
    public SmtpOptions Smtp { get; set; } = new();

    public sealed class SmtpOptions
    {
        public string Host { get; set; } = ""; // p.ej. "mail.tudominio.com"
        public int Port { get; set; } = 587;   // 587(StartTLS) o 465(SSL)
        public string? User { get; set; }      // p.ej. "no-reply@tudominio.com"
        public string? Password { get; set; }  // en Secrets/Variables
        public bool UseStartTls { get; set; } = true; // true=587(StartTLS). Para 465 pon false.
    }
}
