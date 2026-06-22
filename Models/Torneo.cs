using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIFK.Models
{
    [Table("torneo")]
    public class Torneo
    {
        [Key]
        [Column("id_torneo")]
        public int IdTorneo { get; set; }

        [Required]
        [Column("nombre_evento")]
        public string NombreEvento { get; set; } = string.Empty;

        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Column("sede")]
        public string Sede { get; set; } = string.Empty;

        [Column("organizador")]
        public string Organizador { get; set; } = string.Empty;
    }
}