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

    public IActionResult OnPostGuardarCategoria(){
        try
        {
            OnPostPrevisualizar();

            if (AlumnosEncontrados.Count < 2)
            {
                ViewData["Mensaje"] =
                    "No se puede guardar una categoría con menos de 2 competidores.";

                return Page();
            }

            using var conn =
                _conexion.ObtenerConexion();

            conn.Open();

            string sqlCategoria =
                @"INSERT INTO categoria_torneo
                (
                    id_torneo,
                    nombre_categoria,
                    edad_min,
                    edad_max,
                    peso_min,
                    peso_max,
                    rama,
                    cinta_min,
                    cinta_max
                )
                VALUES
                (
                    @idTorneo,
                    @nombre,
                    @edadMin,
                    @edadMax,
                    @pesoMin,
                    @pesoMax,
                    @rama,
                    @cintaMin,
                    @cintaMax
                );";

            using var cmdCategoria =
                new MySqlCommand(sqlCategoria, conn);

            cmdCategoria.Parameters.AddWithValue(
                "@idTorneo",
                1);

            cmdCategoria.Parameters.AddWithValue(
                "@nombre",
                NombreCategoria);

            cmdCategoria.Parameters.AddWithValue(
                "@edadMin",
                EdadMin);

            cmdCategoria.Parameters.AddWithValue(
                "@edadMax",
                EdadMax);

            cmdCategoria.Parameters.AddWithValue(
                "@pesoMin",
                PesoMin);

            cmdCategoria.Parameters.AddWithValue(
                "@pesoMax",
                PesoMax);

            cmdCategoria.Parameters.AddWithValue(
                "@rama",
                Rama);

            cmdCategoria.Parameters.AddWithValue(
                "@cintaMin",
                CintaMin);

            cmdCategoria.Parameters.AddWithValue(
                "@cintaMax",
                CintaMax);

            cmdCategoria.ExecuteNonQuery();

            long idCategoria =
                cmdCategoria.LastInsertedId;

            foreach(var alumno in AlumnosEncontrados)
            {
                string sqlAlumno =
                    @"INSERT INTO categoria_alumno
                    (
                        id_categoria,
                        id_alumno
                    )
                    VALUES
                    (
                        @idCategoria,
                        @idAlumno
                    )";

                using var cmdAlumno =
                    new MySqlCommand(sqlAlumno, conn);

                cmdAlumno.Parameters.AddWithValue(
                    "@idCategoria",
                    idCategoria);

                cmdAlumno.Parameters.AddWithValue(
                    "@idAlumno",
                    alumno.IdAlumno);

                cmdAlumno.ExecuteNonQuery();
            }

            for (int i = 0; i < AlumnosEncontrados.Count; i += 2)
{
    int idAlumno1 =
        AlumnosEncontrados[i].IdAlumno;

    int? idAlumno2 = null;

    if (i + 1 < AlumnosEncontrados.Count)
    {
        idAlumno2 =
            AlumnosEncontrados[i + 1].IdAlumno;
    }

    string sqlEncuentro =
        @"INSERT INTO encuentro
        (
            id_torneo,
            id_categoria,
            id_alumno_1,
            id_alumno_2,
            fase,
            estatus
        )
        VALUES
        (
            @idTorneo,
            @idCategoria,
            @alumno1,
            @alumno2,
            @fase,
            @estatus
        )";

    using var cmdEncuentro =
        new MySqlCommand(sqlEncuentro, conn);

        cmdEncuentro.Parameters.AddWithValue(
            "@idTorneo",
            1);

        cmdEncuentro.Parameters.AddWithValue(
            "@idCategoria",
            idCategoria);

        cmdEncuentro.Parameters.AddWithValue(
            "@alumno1",
            idAlumno1);

        cmdEncuentro.Parameters.AddWithValue(
            "@alumno2",
            (object?)idAlumno2 ?? DBNull.Value);

        cmdEncuentro.Parameters.AddWithValue(
            "@fase",
            "Primera Ronda");

        cmdEncuentro.Parameters.AddWithValue(
            "@estatus",
            "Pendiente");

        cmdEncuentro.ExecuteNonQuery();
    }

            string? idGuardado =
                HttpContext.Session.GetString("IdUsuario");

            if(idGuardado != null)
            {
                int idUsuario =
                    Convert.ToInt32(idGuardado);

                RegistrarAuditoria(
                    idUsuario,
                    $"Creó categoría: {NombreCategoria}",
                    "Torneos");
            }

            ViewData["Mensaje"] =
                "Categoría guardada correctamente.";

            return Page();
        }
        catch(Exception ex)
        {
            ViewData["Mensaje"] =
                ex.Message;

            return Page();
        }
    }

private void RegistrarAuditoria(
        int idUsuario,
        string accion,
        string modulo)
    {
        using var conn =
            _conexion.ObtenerConexion();

        conn.Open();

        string sql =
            @"INSERT INTO log_auditoria
            (
                id_usuario,
                accion,
                modulo,
                ip_direccion
            )
            VALUES
            (
                @idUsuario,
                @accion,
                @modulo,
                @ip
            )";

        using var cmd =
            new MySqlCommand(sql, conn);

        cmd.Parameters.AddWithValue(
            "@idUsuario",
            idUsuario);

        cmd.Parameters.AddWithValue(
            "@accion",
            accion);

        cmd.Parameters.AddWithValue(
            "@modulo",
            modulo);

        cmd.Parameters.AddWithValue(
            "@ip",
            HttpContext.Connection
                .RemoteIpAddress?
                .ToString());

        cmd.ExecuteNonQuery();
    }

}