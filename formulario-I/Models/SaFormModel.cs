// Models/SaFormModel.cs
using System.ComponentModel.DataAnnotations;

namespace FormularioL.Models
{
    /// <summary>
    /// Datos para el formulario de Conocimiento del Cliente: Sociedad Anónima (SA).
    /// Incluye campos comunes (correo, dirección, etc.) y específicos de SA.
    /// </summary>
    public class SaFormModel
    {
        // ---------------- Campos comunes ----------------

        [Required, Display(Name = "Razón social")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required, Display(Name = "RUC")]
        public string RUC { get; set; } = string.Empty;

        [Required, Display(Name = "DV")]
        public string DV { get; set; } = string.Empty;

        [Required, EmailAddress, Display(Name = "Correo para envío de propuesta")]
        public string CorreoPropuesta { get; set; } = string.Empty;

        [Required, Display(Name = "Dirección")]
        public string Direccion { get; set; } = string.Empty;

        [Display(Name = "Sistema contable")]
        public string? SistemaContable { get; set; }

        [Display(Name = "Contacto responsable (nombre y teléfono)")]
        public string? Contacto { get; set; }

        [Display(Name = "Notas (no incluir contraseñas)")]
        public string? Notas { get; set; }

        /// <summary>Consentimiento. En el componente validamos que sea true antes de enviar.</summary>
        public bool AceptaPrivacidad { get; set; }

        /// <summary>Honeypot anti-bot: debe permanecer vacío.</summary>
        public string? CodigoInterno { get; set; }

        // ---------------- Campos específicos SA ----------------

        [Required, Display(Name = "Actividad económica (descripción/CIIU)")]
        public string ActividadEconomica { get; set; } = string.Empty;

        [Range(1900, 2100), Display(Name = "Año de constitución")]
        public int? AnioConstitucion { get; set; }

        [Range(0, double.MaxValue), Display(Name = "Capital social (USD)")]
        public decimal? CapitalSocial { get; set; }

        [Range(0, 100000), Display(Name = "Número de accionistas")]
        public int? NumeroAccionistas { get; set; }

        [Range(0, 100000), Display(Name = "Miembros de la junta/directores")]
        public int? MiembrosJunta { get; set; }

        [Range(0, 1000000), Display(Name = "Empleados de planilla")]
        public int? EmpleadosPlanilla { get; set; }

        [Range(0, double.MaxValue), Display(Name = "Facturación mensual promedio (USD)")]
        public decimal? FacturacionMensualPromedio { get; set; }

        [Range(0, 10000), Display(Name = "Número de cuentas bancarias")]
        public int? NumeroCuentasBancarias { get; set; }

        [Display(Name = "Operaciones en el exterior")]
        public bool OperacionesExterior { get; set; }

        [Display(Name = "EEFF auditados (año previo)")]
        public bool EEFFAuditados { get; set; }
    }
}
