namespace FormularioL.Services;

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
