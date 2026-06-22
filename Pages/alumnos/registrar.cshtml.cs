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
            // Establece una fecha por defecto en el formulario (15 años atrás)
            NuevoAlumno.FechaNacimiento = DateTime.Now.AddYears(-15);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // SOLUCIÓN AL REBOTE: Forzamos a C# a ignorar la validación obligatoria de propiedades
            // que el usuario no llena a mano porque las calcula el sistema automáticamente.
            ModelState.Remove("NuevoAlumno.Matricula");
            ModelState.Remove("NuevoAlumno.Estatus");
            ModelState.Remove("NuevoAlumno.EstatusMedico");
            ModelState.Remove("NuevoAlumno.FechaInscripcion");

            // Si por alguna otra razón el formulario es rechazado, extraemos el nombre exacto 
            // del campo faltante para que sepas con precisión qué arreglar en el navegador.
            if (!ModelState.IsValid)
            {
                // CORRECCIÓN CS8602: Se agregó el operador '!' en ModelState[k]! 
                var erroresDetallados = string.Join(", ", ModelState.Keys
                    .Where(k => ModelState[k]!.Errors.Count > 0)
                    .Select(k => k.Replace("NuevoAlumno.", "")));

                TempData["MensajeError"] = $"El sistema rebotó el formulario. Campos con problemas: [{erroresDetallados}]. Revisa que tengan un formato correcto.";
                return Page();
            }

            try
            {
                // 1. GENERACIÓN AUTOMÁTICA Y CONSECUTIVA DE LA MATRÍCULA
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

                // 2. CONCATENACIÓN MÉDICA SOLICITADA
                NuevoAlumno.EstatusMedico = $"Contacto: {ContactoNombreAux} | Tel: {ContactoTelefonoAux} | Alergias: {MedicosAlergiasAux}";
                
                // Propiedades auxiliares mapeadas correctamente
                NuevoAlumno.ContactoEmergenciaNombre = "-";
                NuevoAlumno.ContactoEmergenciaTelefono = "-"; 
                NuevoAlumno.FechaInscripcion = DateTime.Now;
                NuevoAlumno.Estatus = "Activo";

                // 3. REGLA AUTOMÁTICA DE MINORÍA DE EDAD (Límite dinámico de 18 años)
                if (NuevoAlumno.FechaNacimiento > DateTime.Today.AddYears(-18))
                {
                    NuevoAlumno.IdTutor = 1; // Asigna un tutor por defecto si es menor de edad
                }
                else
                {
                    NuevoAlumno.IdTutor = null; // Mayor de edad no requiere tutor
                }

                // 4. INSERCIÓN DIRECTA EN LA BASE DE DATOS
                _context.Alumno.Add(NuevoAlumno);
                await _context.SaveChangesAsync();

                // Mensaje de éxito que viaja hacia la vista de gestión
                TempData["MensajeExito"] = "¡Alumno registrado con éxito en la Base de Datos!";
                return RedirectToPage("./gestion");
            }
            catch (Exception ex)
            {
                // Si el motor de MySQL rechaza la consulta, atrapamos el mensaje interno real
                TempData["MensajeError"] = "Error crítico de Base de Datos: " + (ex.InnerException?.Message ?? ex.Message);
                return Page();
            }
        }
    }
}