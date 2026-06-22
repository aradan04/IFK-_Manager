using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Linq;
using ProyectoIFK.Data;

namespace ProyectoIFK.Pages;

public class FinanzasModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public FinanzasModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public void OnGet() { }

    public JsonResult OnGetBuscarAlumnos(string query)
    {
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