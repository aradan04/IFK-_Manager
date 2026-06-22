using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProyectoIFK.Data;
using ProyectoIFK.Models;

namespace ProyectoIFK.Pages
{
    public class FinanzasModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public FinanzasModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista para guardar las tarifas y mostrar en el select de la vista
        public List<Tarifa> ListaTarifas { get; set; } = new List<Tarifa>();

        // Método que se ejecuta al cargar la página
        public async Task OnGetAsync() 
        {
            // Cargamos todas las tarifas de la base de datos
            ListaTarifas = await _context.Tarifa.ToListAsync();
        }

        // Método para el buscador (lo que usa tu JS en la vista)
        public JsonResult OnGetBuscarAlumnos(string query)
        {
            if (string.IsNullOrEmpty(query)) return new JsonResult(new { });

            var resultados = _context.Alumno
                .Where(a => a.NombreCompleto.Contains(query) || a.Matricula.Contains(query))
                .Take(5)
                .Select(a => new { 
                    matricula = a.Matricula, 
                    nombre = a.NombreCompleto 
                })
                .ToList();

            return new JsonResult(resultados);
        }
    }
}