public class Encuentro
{
    public int IdEncuentro { get; set; }

    public int? IdAlumno1 { get; set; }

    public int? IdAlumno2 { get; set; }

    public string Alumno1 { get; set; } = "";

    public string Alumno2 { get; set; } = "";

    public string Fase { get; set; } = "";

    public string Estatus { get; set; } = "";

    public int? IdGanador { get; set; }
}