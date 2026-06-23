using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProyectoIFK.Services;
using MySqlConnector;

namespace ProyectoIFK.Pages.alumnos
{
    public class expedienteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ConexionBD _conexion;

        public expedienteModel(
            ApplicationDbContext context,
            ConexionBD conexion)
        {
            _context = context;
            _conexion = conexion;
        }

        [BindProperty(SupportsGet = true)]
        public string? TerminoBusqueda { get; set; }

        public Alumno? AlumnoSeleccionado { get; set; }

        public List<HistorialKyus> HistorialPromociones { get; set; } = new List<HistorialKyus>();

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                AlumnoSeleccionado = await _context.Alumno
                    .FirstOrDefaultAsync(a => a.IdAlumno == id.Value);
            }
            else if (!string.IsNullOrEmpty(TerminoBusqueda))
            {
                // Busca coincidencia exacta con matrícula o parcial con el nombre
                AlumnoSeleccionado = await _context.Alumno
                    .FirstOrDefaultAsync(a => a.Matricula == TerminoBusqueda || a.NombreCompleto.Contains(TerminoBusqueda));
            }

            if (AlumnoSeleccionado != null)
            {
                HistorialPromociones = await _context.Historial_Kyus
                    .Where(h => h.IdAlumno == AlumnoSeleccionado.IdAlumno)
                    .OrderBy(h => h.FechaEvaluacion)
                    .ToListAsync();

                        string? idGuardado =
                            HttpContext.Session.GetString("IdUsuario");

                        if(idGuardado != null)
                        {
                            int idUsuario =
                                Convert.ToInt32(idGuardado);

                            RegistrarAuditoria(
                                idUsuario,
                                $"Consultó expediente de {AlumnoSeleccionado.NombreCompleto}",
                                "Expediente");
                        }
            }
        }

        public IActionResult OnPostBuscar()
        {
            if (string.IsNullOrEmpty(TerminoBusqueda))
            {
                return RedirectToPage();
            }
            return RedirectToPage(new { TerminoBusqueda = TerminoBusqueda });
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