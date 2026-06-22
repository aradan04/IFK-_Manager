using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoIFK.Models;

[Table("Asistencia")]
public class Asistencia
{
    [Key]
    [Column("id_asistencia")]
    public int IdAsistencia { get; set; }

    [Required]
    [Column("id_alumno")]
    public int IdAlumno { get; set; }

    [Required]
    [Column("fecha")]
    public DateTime Fecha { get; set; }

    [Required]
    [Column("estatus")]
    public string Estatus { get; set; } = string.Empty;

    [Column("observaciones")]
    public string? Observaciones { get; set; }

    [ForeignKey("IdAlumno")]
    public Alumno Alumno { get; set; } = default!;
}