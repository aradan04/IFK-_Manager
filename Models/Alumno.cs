namespace ProyectoIFK.Models
{
    public class Alumno
    {
        public int IdAlumno { get; set; }

        public string Matricula { get; set; } = "";

        public string NombreCompleto { get; set; } = "";

        public DateTime FechaNacimiento { get; set; }

        public string Sexo { get; set; } = "";

        public decimal Peso { get; set; }

        public int Estatura { get; set; }

        public string KyuActual { get; set; } = "";

        public string EstatusMedico { get; set; } = "";
    }
}