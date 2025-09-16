using System.ComponentModel.DataAnnotations;

namespace FormularioL.Models;

public sealed class ConocimientoClienteModel
{
    [Display(Name = "Nombre del P.H.")]
    [Required(ErrorMessage = "Indica el nombre del P.H.")]
    [StringLength(120, ErrorMessage = "Máx. {1} caracteres.")]
    public string NombrePH { get; set; } = "";

    [Display(Name = "RUC")]
    [Required(ErrorMessage = "Indica el RUC.")]
    [StringLength(30, ErrorMessage = "Máx. {1} caracteres.")]
    public string RUC { get; set; } = "";

    [Display(Name = "DV")]
    [Required(ErrorMessage = "Indica el DV.")]
    [StringLength(10, ErrorMessage = "Máx. {1} caracteres.")]
    public string DV { get; set; } = "";

    [Display(Name = "Correo para envío de propuesta")]
    [Required(ErrorMessage = "Indica un correo de contacto.")]
    [EmailAddress(ErrorMessage = "Correo inválido.")]
    [StringLength(200, ErrorMessage = "Máx. {1} caracteres.")]
    public string? CorreoPropuesta { get; set; }

    [Display(Name = "Dirección física del P.H.")]
    [Required(ErrorMessage = "Indica la dirección.")]
    [StringLength(300, ErrorMessage = "Máx. {1} caracteres.")]
    public string? Direccion { get; set; }

    [Display(Name = "Unidades inmobiliarias")]
    [Range(0, 100000, ErrorMessage = "Valor fuera de rango.")]
    public int Unidades { get; set; }

    [Display(Name = "Torres")]
    [Range(0, 10000, ErrorMessage = "Valor fuera de rango.")]
    public int Torres { get; set; }

    [Display(Name = "Cuota mantenimiento (USD/mes)")]
    [Range(0, 1_000_000, ErrorMessage = "Valor fuera de rango.")]
    public decimal CuotaMantenimientoMes { get; set; }

    [Display(Name = "Gastos mantenimiento (USD/mes)")]
    [Range(0, 1_000_000, ErrorMessage = "Valor fuera de rango.")]
    public decimal GastosMantenimientoMes { get; set; }

    [Display(Name = "Empleados de planilla")]
    [Range(0, 10000, ErrorMessage = "Valor fuera de rango.")]
    public int EmpleadosPlanilla { get; set; }

    [Display(Name = "¿Manejan horas extras?")]
    public bool ManejaHorasExtras { get; set; }

    [Display(Name = "¿Hay promotora en el P.H.?")]
    public bool HayPromotora { get; set; }

    [Display(Name = "Sistema contable")]
    [Required(ErrorMessage = "Indica el sistema contable.")]
    [StringLength(120, ErrorMessage = "Máx. {1} caracteres.")]
    public string? SistemaContable { get; set; }

    [Display(Name = "EEFF auditados (año previo)")]
    public bool EEFFAuditados { get; set; }

    [Display(Name = "Contacto responsable (nombre y teléfono)")]
    [Required(ErrorMessage = "Indica un contacto.")]
    [StringLength(200, ErrorMessage = "Máx. {1} caracteres.")]
    public string? Contacto { get; set; }

    [Display(Name = "Notas (no incluir contraseñas)")]
    [StringLength(1000, ErrorMessage = "Máx. {1} caracteres.")]
    public string? Notas { get; set; }

    // Honeypot anti-bot (campo oculto en el formulario)
    public string? CodigoInterno { get; set; }

    [Display(Name = "Acepto el uso de esta información")]
    [RequiredTrue(ErrorMessage = "Debes aceptar para continuar.")]
    public bool AceptaPrivacidad { get; set; }
}

/// <summary>Validador para checks obligatorios.</summary>
public sealed class RequiredTrueAttribute : ValidationAttribute
{
    public override bool IsValid(object? value) => value is bool b && b;
}
