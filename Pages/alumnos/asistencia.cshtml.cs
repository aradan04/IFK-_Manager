using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using ProyectoIFK.Services;
using MySqlConnector;

namespace ProyectoIFK.Pages.alumnos
{
    public class AlumnoAsistencia
    {
        public int IdAlumno { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string KyuActual { get; set; } = string.Empty;
        public bool Asistio { get; set; }
    }

    public class asistenciaModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ConexionBD _conexion;

        public asistenciaModel(
            ApplicationDbContext context,
            ConexionBD conexion)
        {
            _context = context;
            _conexion = conexion;
        }

        [BindProperty(SupportsGet = true)]
        public string ClaseSeleccionada { get; set; } = "ADULTOS";

        [BindProperty]
        public List<AlumnoAsistencia> ListaAlumnosAsistencia { get; set; } = new List<AlumnoAsistencia>();

        public async Task OnGetAsync()
        {
            // HE ELIMINADO EL .Where(...) PARA QUE NO DÉ ERROR.
            // Ahora cargará todos los alumnos sin filtrar.
            ListaAlumnosAsistencia = await _context.Alumno
                .AsNoTracking()
                .Select(a => new AlumnoAsistencia {
                    IdAlumno = a.IdAlumno,
                    NombreCompleto = a.NombreCompleto,
                    KyuActual = a.KyuActual ?? "Blanca"
                })
                .OrderBy(a => a.NombreCompleto)
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int idUsuario = int.TryParse(userIdClaim, out int res) ? res : 1; 
            var hoy = DateTime.Today;

            try 
            {
                int guardados = 0;
                foreach (var item in ListaAlumnosAsistencia.Where(a => a.Asistio))
                {
                    bool existe = await _context.Set<Asistencia>()
                        .AnyAsync(a => a.IdAlumno == item.IdAlumno && a.Fecha.Date == hoy);

                    if (!existe) {
                        await _context.Database.ExecuteSqlRawAsync(
                            "INSERT INTO asistencia (id_alumno, id_usuario, fecha, estatus) VALUES ({0}, {1}, {2}, {3})",
                            item.IdAlumno, idUsuario, DateTime.Now, "Asistencia"
                        );
                        guardados++;
                    }
                }
                TempData["Mensaje"] = guardados > 0 ? $"¡{guardados} asistencias guardadas!" : "No se registraron nuevas asistencias.";

                string? idGuardado =
                    HttpContext.Session.GetString("IdUsuario");

                if(idGuardado != null && guardados > 0)
                {
                    int idUsuarioAuditoria =
                        Convert.ToInt32(idGuardado);

                    RegistrarAuditoria(
                        idUsuarioAuditoria,
                        $"Registró {guardados} asistencias",
                        "Asistencia");
                }
            }
            catch (Exception ex) {
                TempData["Mensaje"] = "Error: " + ex.Message;
            }
            return RedirectToPage("./asistencia");
        }

        private void RegistrarAuditoria(
            int idUsuario,
            string accion,
            string modulo)
        {
            using var conn =
                _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"INSERT INTO log_auditoria
                (
                    id_usuario,
                    accion,
                    modulo,
                    ip_direccion
                )
                VALUES
                (
                    @idUsuario,
                    @accion,
                    @modulo,
                    @ip
                )";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@idUsuario",
                idUsuario);

            cmd.Parameters.AddWithValue(
                "@accion",
                accion);

            cmd.Parameters.AddWithValue(
                "@modulo",
                modulo);

            cmd.Parameters.AddWithValue(
                "@ip",
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString());

            cmd.ExecuteNonQuery();
        }
    }
}