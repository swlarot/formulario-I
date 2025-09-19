// Services/EmailOptions.cs
using System.Collections.Generic;

namespace FormularioL.Services
{
    /// <summary>
    /// Opciones para el envío de correos del formulario.
    /// Se vincula desde appsettings: "Email": { ... }.
    /// </summary>
    public record EmailOptions
    {
        /// <summary>Configuración SMTP.</summary>
        public SmtpOptions Smtp { get; init; } = new();

        /// <summary>Remitente para el correo interno y, por defecto, para el acuse.</summary>
        public string From { get; init; } = string.Empty;

        /// <summary>Destinatario interno (si no hay ruteo por entidad).</summary>
        public string To { get; init; } = string.Empty;

        /// <summary>
        /// Ruteo por entidad (opcional). Ej.: { "ph":"ayuda@...", "sa":"corporativo@..." }.
        /// Si no existe la clave, se usa "To".
        /// </summary>
        public Dictionary<string, string> Routing { get; init; } = new();

        /// <summary>Opciones del acuse de recibo (al solicitante).</summary>
        public AckOptions Ack { get; init; } = new();

        /// <summary>Opciones SMTP.</summary>
        public record SmtpOptions
        {
            public string Host { get; init; } = string.Empty;
            public int Port { get; init; } = 587;
            public string User { get; init; } = string.Empty;
            public string Password { get; init; } = string.Empty;
            public bool UseStartTls { get; init; } = true;

            /// <summary>Timeout de operaciones SMTP en milisegundos.</summary>
            public int TimeoutMs { get; init; } = 15000;
        }

        /// <summary>Opciones para el acuse de recibo.</summary>
        public record AckOptions
        {
            /// <summary>Habilita/Deshabilita el envío de acuse.</summary>
            public bool Enabled { get; init; } = true;

            /// <summary>Remitente del acuse; si está vacío usa "From".</summary>
            public string From { get; init; } = string.Empty;

            /// <summary>Asunto del acuse.</summary>
            public string Subject { get; init; } = "Hemos recibido tu solicitud";

            /// <summary>Texto (HTML permitido) corto para el acuse.</summary>
            public string HtmlBody { get; init; } =
                "<p>Gracias por tu mensaje. Hemos recibido tu información y te contactaremos pronto.</p>" +
                "<p><small>No compartas contraseñas por este medio.</small></p>";

            /// <summary>Texto alterno en plano para el acuse.</summary>
            public string TextBody { get; init; } =
                "Gracias por tu mensaje. Hemos recibido tu información y te contactaremos pronto.\n" +
                "(No compartas contraseñas por este medio.)";
        }
    }
}
