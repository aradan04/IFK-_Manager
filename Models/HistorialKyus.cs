using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIFK.Models
{
    [Table("Historial_Kyus")]
    public class HistorialKyus
    {
        [Key]
        [Column("id_historial")]
        public int IdHistorial { get; set; }

        [Column("id_alumno")]
        public int IdAlumno { get; set; }

        [Required]
        [StringLength(50)]
        [Column("kyu_obtenido")] // <-- Corregido con el dato real
        public string KyuOtorgado { get; set; } = string.Empty;

        [Column("fecha_examen")] // <-- Corregido con el dato real
        public DateTime FechaEvaluacion { get; set; }

        [Column("sinodal")] // <-- Ahora sí sabemos cómo se llama el evaluador
        public string? Evaluador { get; set; }
    }
}