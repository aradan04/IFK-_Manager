using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIFK.Pages.alumnos
{
    public class AlumnoAsistencia
    {
        public int IdAlumno { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string KyuActual { get; set; } = string.Empty;
        public bool Asistio { get; set; }
        public string Observaciones { get; set; } = string.Empty;
    }

    public class asistenciaModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public asistenciaModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string ClaseSeleccionada { get; set; } = "ADULTOS";

        [BindProperty]
        public List<AlumnoAsistencia> ListaAlumnosAsistencia { get; set; } = new List<AlumnoAsistencia>();

        public async Task OnGetAsync()
        {
            // Consultamos la base de datos real y ordenamos alfabéticamente
            ListaAlumnosAsistencia = await _context.Alumno
                .Select(a => new AlumnoAsistencia
                {
                    IdAlumno = a.IdAlumno,
                    NombreCompleto = a.NombreCompleto,
                    KyuActual = a.KyuActual ?? "Blanca (sin grado)",
                    Asistio = false,
                    Observaciones = string.Empty
                })
                .OrderBy(a => a.NombreCompleto)
                .ToListAsync();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Aquí irá la lógica para guardar el pase de lista en el futuro
            TempData["MensajeExito"] = "¡Asistencia guardada correctamente!";
            return RedirectToPage("./asistencia");
        }
    }
}