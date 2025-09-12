namespace FormularioL.Services;

using FormularioL.Models;

public interface IEmailService
{
    Task SendConocimientoClienteAsync(ConocimientoClienteModel m);
}
