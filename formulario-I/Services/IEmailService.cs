// Services/IEmailService.cs
using System.Threading;
using System.Threading.Tasks;
using FormularioL.Models;

namespace FormularioL.Services
{
    /// <summary>
    /// Servicio de envío de correos del formulario KYC.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envío retro-compatible (asume entidad "ph").
        /// </summary>
        Task SendConocimientoClienteAsync(ConocimientoClienteModel m);

        /// <summary>
        /// Envío con entidad. El CancellationToken es opcional para no romper llamadas existentes.
        /// </summary>
        Task SendConocimientoClienteAsync(ConocimientoClienteModel m, string entidadSlug, CancellationToken ct = default);

        // SA (nuevo)
        Task SendSaAsync(SaFormModel m, CancellationToken ct = default);
    }
}
