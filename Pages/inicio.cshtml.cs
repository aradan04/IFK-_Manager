using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProyectoIFK.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoIFK.Pages
{
    public class InicioModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public InicioModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public int AlumnosActivos { get; set; }

        public string SiguienteTorneo { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            var listaAlumnos = await _context.Alumno.ToListAsync();
            
            AlumnosActivos = listaAlumnos.Count(a => a.Estatus == "Activo" || a.Estatus == null);

            var proximoTorneo = await _context.Torneo
                .Where(t => t.Fecha >= DateTime.Today)
                .OrderBy(t => t.Fecha)
                .FirstOrDefaultAsync();

            if (proximoTorneo != null)
            {
                SiguienteTorneo = proximoTorneo.NombreEvento;
            }
            else
            {
                SiguienteTorneo = "No hay torneos próximos";
            }
        }
    }
}