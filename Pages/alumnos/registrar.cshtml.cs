using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProyectoIFK.Services;
using MySqlConnector;

namespace ProyectoIFK.Pages.alumnos
{
    public class registrarModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ConexionBD _conexion;

        public registrarModel(
            ApplicationDbContext context,
            ConexionBD conexion)
        {
            _context = context;
            _conexion = conexion;
        }

        [BindProperty]
        public Alumno NuevoAlumno { get; set; } = new Alumno();

        [BindProperty]
        public string ContactoNombreAux { get; set; } = string.Empty;

        [BindProperty]
        public string ContactoTelefonoAux { get; set; } = string.Empty;

        [BindProperty]
        public string MedicosAlergiasAux { get; set; } = string.Empty;

        public void OnGet()
        {
            NuevoAlumno.FechaNacimiento = DateTime.Now.AddYears(-15);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Se eliminó la referencia a NuevoAlumno.FechaInscripcion
            ModelState.Remove("NuevoAlumno.Matricula");
            ModelState.Remove("NuevoAlumno.Estatus");
            ModelState.Remove("NuevoAlumno.EstatusMedico");

            if (!ModelState.IsValid)
            {
                var erroresDetallados = string.Join(", ", ModelState.Keys
                    .Where(k => ModelState[k]!.Errors.Count > 0)
                    .Select(k => k.Replace("NuevoAlumno.", "")));

                TempData["MensajeError"] = $"El sistema rebotó el formulario. Campos con problemas: [{erroresDetallados}]. Revisa que tengan un formato correcto.";
                return Page();
            }

            try
            {
                var ultimoAlumno = await _context.Alumno
                    .Where(a => a.Matricula.StartsWith("S"))
                    .OrderByDescending(a => a.Matricula)
                    .FirstOrDefaultAsync();

                if (ultimoAlumno != null && ultimoAlumno.Matricula.Length > 1)
                {
                    string parteNumericaStr = ultimoAlumno.Matricula.Substring(1);
                    if (long.TryParse(parteNumericaStr, out long numeroActual))
                    {
                        NuevoAlumno.Matricula = "S" + (numeroActual + 1).ToString();
                    }
                    else
                    {
                        NuevoAlumno.Matricula = "S24013371";
                    }
                }
                else
                {
                    NuevoAlumno.Matricula = "S24013370";
                }

                NuevoAlumno.EstatusMedico = $"Contacto: {ContactoNombreAux} | Tel: {ContactoTelefonoAux} | Alergias: {MedicosAlergiasAux}";
                
                NuevoAlumno.ContactoEmergenciaNombre = "-";
                NuevoAlumno.ContactoEmergenciaTelefono = "-"; 
                NuevoAlumno.Estatus = "Activo";

                if (NuevoAlumno.FechaNacimiento > DateTime.Today.AddYears(-18))
                {
                    NuevoAlumno.IdTutor = 1; 
                }
                else
                {
                    NuevoAlumno.IdTutor = null; 
                }

                _context.Alumno.Add(NuevoAlumno);
                await _context.SaveChangesAsync();

                string? idGuardado =
                    HttpContext.Session.GetString("IdUsuario");

                if(idGuardado != null)
                {
                    int idUsuario =
                        Convert.ToInt32(idGuardado);

                    RegistrarAuditoria(
                        idUsuario,
                        $"Registró alumno {NuevoAlumno.NombreCompleto} ({NuevoAlumno.Matricula})",
                        "Registro de Alumnos");
                }

                TempData["MensajeExito"] = "¡Alumno registrado con éxito en la Base de Datos!";
                return RedirectToPage("./gestion");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error crítico de Base de Datos: " + (ex.InnerException?.Message ?? ex.Message);
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