using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ProyectoIFK.Models;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages.torneos;

public class categoriasModel : PageModel
{
    private readonly ConexionBD _conexion;

    public categoriasModel(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    private int ObtenerNivelKyu(string kyu)
    {
        return kyu switch
        {
            "10mo Kyu" => 1,
            "9no Kyu"  => 2,
            "8vo Kyu"  => 3,
            "7mo Kyu"  => 4,
            "6to Kyu"  => 5,
            "5to Kyu"  => 6,
            "4to Kyu"  => 7,
            "3er Kyu"  => 8,
            "2do Kyu"  => 9,
            "1er Kyu"  => 10,
            _ => 0
        };
    }

    public string ObtenerCinta(string kyu)
    {
        return kyu switch
        {
            "10mo Kyu" => "Blanca",
            "9no Kyu"  => "Amarilla",
            "8vo Kyu"  => "Naranja",
            "7mo Kyu"  => "Azul",
            "6to Kyu"  => "Verde",
            "5to Kyu"  => "Verde",
            "4to Kyu"  => "Café",
            "3er Kyu"  => "Café",
            "2do Kyu"  => "Café",
            "1er Kyu"  => "Café",
            _ => kyu
        };
    }

    private int ObtenerNivelSeleccionado(string cinta)
    {
        return cinta switch
        {
            "Blanca" => 1,
            "Amarilla" => 2,
            "Naranja" => 3,
            "Azul" => 4,
            "Verde" => 5,
            "Café" => 10,
            _ => 0
        };
    }

    public List<Alumno> AlumnosEncontrados { get; set; } = new();

    public string MensajeGuardado { get; set; } = "";

    [BindProperty]
    public string NombreCategoria { get; set; } = "";

    [BindProperty]
    public int EdadMin { get; set; }

    [BindProperty]
    public int EdadMax { get; set; }

    [BindProperty]
    public decimal PesoMin { get; set; }

    [BindProperty]
    public decimal PesoMax { get; set; }

    [BindProperty]
    public string Rama { get; set; } = "";

    [BindProperty]
    public string CintaMin { get; set; } = "";

    [BindProperty]
    public string CintaMax { get; set; } = "";

    public void OnGet()
    {
        NombreCategoria = "Infantil Principiantes";

        EdadMin = 8;
        EdadMax = 11;

        PesoMin = 25;
        PesoMax = 40;

        Rama = "Varonil";

        CintaMin = "Blanca";
        CintaMax = "Amarilla";
    }

    public IActionResult OnPostPrevisualizar(){
        try{
            AlumnosEncontrados.Clear();

            using var conn =
                _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"SELECT
                    id_alumno,
                    matricula,
                    nombre_completo,
                    fecha_nacimiento,
                    sexo,
                    peso,
                    estatura,
                    kyu_actual,
                    estatus_medico
                FROM alumno";

            using var cmd =
                new MySqlCommand(sql, conn);

            using var reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                DateTime fechaNacimiento =
                    Convert.ToDateTime(
                        reader["fecha_nacimiento"]);

                int edad =
                    DateTime.Today.Year -
                    fechaNacimiento.Year;

                if (fechaNacimiento.Date >
                    DateTime.Today.AddYears(-edad))
                {
                    edad--;
                }

                decimal peso =
                    reader["peso"] == DBNull.Value
                    ? 0
                    : Convert.ToDecimal(
                        reader["peso"]);

                string sexo =
                    reader["sexo"]?.ToString() ?? "";

                bool cumpleRama = true;

                string kyuActual =
                    reader["kyu_actual"]?.ToString() ?? "";

                int nivelAlumno =
                    ObtenerNivelKyu(kyuActual);

                int nivelMinimo =
                    ObtenerNivelSeleccionado(CintaMin);

                int nivelMaximo =
                    ObtenerNivelSeleccionado(CintaMax);

                bool cumpleCinta =
                    nivelAlumno >= nivelMinimo &&
                    nivelAlumno <= nivelMaximo;

                if (Rama == "Varonil")
                {
                    cumpleRama = sexo == "H";
                }
                else if (Rama == "Femenil")
                {
                    cumpleRama = sexo == "M";
                }

                if (
                    edad >= EdadMin &&
                    edad <= EdadMax &&
                    peso >= PesoMin &&
                    peso <= PesoMax &&
                    cumpleRama &&
                    cumpleCinta
                )
                {
                    AlumnosEncontrados.Add(
                        new Alumno
                        {
                            IdAlumno =
                                Convert.ToInt32(
                                    reader["id_alumno"]),

                            Matricula =
                                reader["matricula"]?.ToString() ?? "",

                            NombreCompleto =
                                reader["nombre_completo"]?.ToString() ?? "",

                            FechaNacimiento =
                                fechaNacimiento,

                            Sexo =
                                sexo,

                            Peso =
                                peso,

                            Estatura =
                                reader["estatura"] == DBNull.Value
                                ? 0
                                : Convert.ToInt32(
                                    reader["estatura"]),

                            KyuActual =
                                reader["kyu_actual"]?.ToString() ?? "",

                            EstatusMedico =
                                reader["estatus_medico"]?.ToString() ?? ""
                        });
                }
            }

            if(AlumnosEncontrados.Count < 2)
                {
                    ViewData["Mensaje"] =
                        "Se necesitan al menos 2 competidores para crear una categoría.";
                }

            return Page();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            return Page();
        }
    }
}