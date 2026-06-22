namespace ProyectoIFK.Models
{
    public class CategoriaTorneo
    {
        public int IdCategoria { get; set; }

        public int IdTorneo { get; set; }

        public string NombreCategoria { get; set; } = "";

        public int EdadMin { get; set; }

        public int EdadMax { get; set; }

        public decimal PesoMin { get; set; }

        public decimal PesoMax { get; set; }

        public string Rama { get; set; } = "";

        public string CintaMin { get; set; } = "";

        public string CintaMax { get; set; } = "";
    }
}