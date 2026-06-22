using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIFK.Models
{
    [Table("asistencia")]
    public class Asistencia
    {
        [Key]
        [Column("id_asistencia")]
        public int IdAsistencia { get; set; }

        [Required]
        [Column("id_alumno")]
        public int IdAlumno { get; set; }

        [Required]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("estatus")]
        public string Estatus { get; set; } = "Asistencia";
    }
}