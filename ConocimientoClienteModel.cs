using System.ComponentModel.DataAnnotations;

public class ConocimientoClienteModel
{
    [Required, Display(Name = "Nombre del P.H.")] public string NombrePH { get; set; } = "";
    [Required] public string RUC { get; set; } = "";
    [Required] public string DV { get; set; } = "";
    [Required, EmailAddress, Display(Name = "Correo para envío de propuesta")]
    public string CorreoPropuesta { get; set; } = "";
    [Required, Display(Name = "Dirección física del P.H.")] public string Direccion { get; set; } = "";
    [Required, Range(1, int.MaxValue), Display(Name = "Unidades inmobiliarias")] public int Unidades { get; set; }
    [Required, Range(1, int.MaxValue), Display(Name = "Torres")] public int Torres { get; set; }
    [Required, Range(0, double.MaxValue), Display(Name = "Cuota de mantenimiento (USD/mes)")]
    public decimal CuotaMantenimientoMes { get; set; }
    [Required, Range(0, double.MaxValue), Display(Name = "Gastos de mantenimiento (USD/mes)")]
    public decimal GastosMantenimientoMes { get; set; }
    [Required, Range(0, int.MaxValue), Display(Name = "Empleados de planilla")]
    public int EmpleadosPlanilla { get; set; }
    [Required, Display(Name = "¿Manejan horas extras?")] public bool ManejaHorasExtras { get; set; }
    [Required, Display(Name = "¿Hay promotora en el P.H.?")] public bool HayPromotora { get; set; }
    [Required, Display(Name = "Sistema contable")] public string SistemaContable { get; set; } = "";
    [Required, Display(Name = "EEFF auditados (año previo)")] public bool EEFFAuditados { get; set; }
    [Required, Display(Name = "Contacto responsable (nombre y teléfono)")] public string Contacto { get; set; } = "";
    [Display(Name = "Notas (no incluir contraseñas)")] public string? Notas { get; set; }
}
