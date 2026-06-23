using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using System;
using System.Threading.Tasks;
using ProyectoIFK.Services;
using MySqlConnector;

namespace ProyectoIFK.Pages.alumnos
{
    public class promocionarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ConexionBD _conexion;

        public promocionarModel(
            ApplicationDbContext context,
            ConexionBD conexion)
        {
            _context = context;
            _conexion = conexion;
        }

        [BindProperty]
        public HistorialKyus NuevaPromocion { get; set; } = new HistorialKyus();

        public string NombreAlumno { get; set; } = string.Empty;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Buscamos al alumno para mostrar su nombre en el formulario
            var alumno = await _context.Alumno.FirstOrDefaultAsync(a => a.IdAlumno == id);
            if (alumno == null)
            {
                return RedirectToPage("./gestion");
            }

            NombreAlumno = alumno.NombreCompleto;
            NuevaPromocion.IdAlumno = id;
            NuevaPromocion.FechaEvaluacion = DateTime.Today; // Fecha actual por defecto

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var alumno = await _context.Alumno.FirstOrDefaultAsync(a => a.IdAlumno == NuevaPromocion.IdAlumno);
                if (alumno != null) NombreAlumno = alumno.NombreCompleto;
                return Page();
            }

            try
            {
                // 1. INSERCIÓN EN LA TABLA HISTORIAL_KYUS
                _context.Historial_Kyus.Add(NuevaPromocion);

                // 2. ACTUALIZACIÓN DE LA CINTA ACTUAL EN LA TABLA ALUMNO
                var alumno = await _context.Alumno.FirstOrDefaultAsync(a => a.IdAlumno == NuevaPromocion.IdAlumno);
                if (alumno != null)
                {
                    alumno.KyuActual = NuevaPromocion.KyuOtorgado;
                    _context.Alumno.Update(alumno);
                }

                // Guardamos ambos cambios en una sola transacción segura
                await _context.SaveChangesAsync();

                string? idGuardado =
                    HttpContext.Session.GetString("IdUsuario");

                if(idGuardado != null && alumno != null)
                {
                    int idUsuario =
                        Convert.ToInt32(idGuardado);

                    RegistrarAuditoria(
                        idUsuario,
                        $"Promovió a {alumno.NombreCompleto} a {NuevaPromocion.KyuOtorgado}",
                        "Promociones");
                }

                TempData["MensajeExito"] = "¡Promoción registrada y cinta actualizada con éxito!";
                
                // Redirigimos de vuelta al expediente del alumno para ver el cambio reflejado
                return RedirectToPage("./expediente", new { id = NuevaPromocion.IdAlumno });
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error al registrar promoción: " + (ex.InnerException?.Message ?? ex.Message);
                
                var alumno = await _context.Alumno.FirstOrDefaultAsync(a => a.IdAlumno == NuevaPromocion.IdAlumno);
                if (alumno != null) NombreAlumno = alumno.NombreCompleto;
                return Page();
            }
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