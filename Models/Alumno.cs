using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIFK.Models
{
    [Table("alumno")]
    public class Alumno
    {
        [Key]
        [Column("id_alumno")]
        public int IdAlumno { get; set; }

        [Required]
        [StringLength(20)]
        [Column("matricula")]
        public string Matricula { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        [Column("nombre_completo")]
        public string NombreCompleto { get; set; } = string.Empty;

        [Column("fecha_nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(18)]
        [Column("curp")]
        public string? Curp { get; set; }

        [StringLength(1)]
        [Column("sexo")]
        public string? Sexo { get; set; }

        [Column("peso")]
        public decimal? Peso { get; set; }

        [Column("estatura")]
        public int? Estatura { get; set; }

        [StringLength(30)]
        [Column("kyu_actual")]
        public string? KyuActual { get; set; }

        [Column("estatus_medico")]
        public string? EstatusMedico { get; set; }

        [Column("id_tutor")]
        public int? IdTutor { get; set; }

        [Required]
        [Column("id_tarifa")]
        public int IdTarifa { get; set; }

        // --- CAMPOS NO MAPEADOS (Para evitar errores de "Unknown column") ---

        [NotMapped]
        public DateTime FechaInscripcion { get; set; }

        [NotMapped]
        public string? Estatus { get; set; }

        [NotMapped]
        public string? ContactoEmergenciaNombre { get; set; }
        
        [NotMapped]
        public string? ContactoEmergenciaTelefono { get; set; }
    }
}