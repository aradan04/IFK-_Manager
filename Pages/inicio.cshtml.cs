using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoIFK.Pages;

public class InicioModel : PageModel
{
    public int AlumnosActivos { get; set; }

    public int PagosPendientes { get; set; }

    public string SiguienteTorneo { get; set; } = string.Empty;

    public void OnGet()
    {
        // Datos de ejemplo mientras conectamos la BD

        AlumnosActivos = 45;
        PagosPendientes = 3;
        SiguienteTorneo = "Copa Primavera 2026";
    }
}