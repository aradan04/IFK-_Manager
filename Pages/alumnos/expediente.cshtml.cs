using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIFK.Pages.alumnos
{
    public class expedienteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public expedienteModel(ApplicationDbContext context)
        {
            _context = context;
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
    }
}