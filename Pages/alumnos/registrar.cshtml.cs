using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIFK.Pages.alumnos
{
    public class registrarModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public registrarModel(ApplicationDbContext context)
        {
            _context = context;
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

                TempData["MensajeExito"] = "¡Alumno registrado con éxito en la Base de Datos!";
                return RedirectToPage("./gestion");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = "Error crítico de Base de Datos: " + (ex.InnerException?.Message ?? ex.Message);
                return Page();
            }
        }
    }
}