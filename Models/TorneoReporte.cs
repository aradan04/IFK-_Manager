namespace ProyectoIFK.Models
{
    public class TorneoReporte
    {
        public int IdTorneo { get; set; }

        public string NombreEvento { get; set; } = "";

        public DateTime FechaEvento { get; set; }

        public string Sede { get; set; } = "";

        public string Organizador { get; set; } = "";
    }
}