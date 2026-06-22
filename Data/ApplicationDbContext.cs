using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Models;

namespace ProyectoIFK.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Registramos las tablas del módulo Alumnos
        public DbSet<Alumno> Alumno { get; set; }
        public DbSet<Asistencia> Asistencia { get; set; }
        public DbSet<HistorialKyus> HistorialKyus { get; set; }
    }
}