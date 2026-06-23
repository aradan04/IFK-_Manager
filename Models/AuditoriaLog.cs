    namespace ProyectoIFK.Models
{
    public class AuditoriaLog
    {
        public int IdLog { get; set; }

        public string Usuario { get; set; } = "";

        public string Accion { get; set; } = "";

        public string Modulo { get; set; } = "";

        public DateTime FechaHora { get; set; }

        public string IpDireccion { get; set; } = "";
    }
}