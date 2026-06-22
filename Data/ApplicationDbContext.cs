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

        // Módulo de Alumnos
        public DbSet<Alumno> Alumno { get; set; }
        public DbSet<Asistencia> Asistencia { get; set; }
        
        // Módulo de Promociones (Historial de Grados)
        public DbSet<HistorialKyus> Historial_Kyus { get; set; }

        // Módulo de Finanzas - NUEVA TABLA
        public DbSet<Tarifa> Tarifa { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}