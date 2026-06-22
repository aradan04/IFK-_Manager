using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIFK.Models
{
    [Table("tarifa")]
    public class Tarifa
    {
        [Key]
        [Column("id_tarifa")]
        public int IdTarifa { get; set; }

        [Required]
        [StringLength(100)]
        [Column("nombre_categoria")]
        public string NombreCategoria { get; set; } = string.Empty;

        [Column("monto")]
        public decimal Monto { get; set; }

        [StringLength(255)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("fecha_actualizacion")]
        public DateTime FechaActualizacion { get; set; }
    }
}