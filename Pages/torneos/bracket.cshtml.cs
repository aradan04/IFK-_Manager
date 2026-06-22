using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using ProyectoIFK.Models;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages.torneos;

public class bracketModel : PageModel
{
    private readonly ConexionBD _conexion;

    public bracketModel(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    public List<Encuentro> Encuentros { get; set; } = new();

    public List<CategoriaTorneo> Categorias { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public int IdCategoria { get; set; }
    

    public void OnGet()
    {
        using var conn =
            _conexion.ObtenerConexion();

        conn.Open();

        string sqlCategorias =
            @"SELECT
                id_categoria,
                nombre_categoria
            FROM categoria_torneo
            ORDER BY nombre_categoria";

        using var cmdCategorias =
            new MySqlCommand(sqlCategorias, conn);

        using var readerCategorias =
            cmdCategorias.ExecuteReader();
            

        while (readerCategorias.Read())
        {
            Categorias.Add(
                new CategoriaTorneo
                {
                    IdCategoria =
                        readerCategorias.GetInt32("id_categoria"),

                    NombreCategoria =
                        readerCategorias.GetString("nombre_categoria")
                });
        }

        readerCategorias.Close();

        if (IdCategoria > 0)
        {
        string sqlEncuentros =
            @"SELECT
                E.id_encuentro,
                E.id_alumno_1,
                E.id_alumno_2,
                E.id_ganador,
                A1.nombre_completo AS alumno1,
                A2.nombre_completo AS alumno2,
                E.fase,
                E.estatus
            FROM encuentro E
            LEFT JOIN alumno A1
                ON A1.id_alumno = E.id_alumno_1
            LEFT JOIN alumno A2
                ON A2.id_alumno = E.id_alumno_2
            WHERE E.id_categoria = @idCategoria";

        using var cmdEncuentros =
            new MySqlCommand(sqlEncuentros, conn);

        cmdEncuentros.Parameters.AddWithValue(
            "@idCategoria",
            IdCategoria);

        using var reader =
            cmdEncuentros.ExecuteReader();

        while(reader.Read())
        {
            Encuentros.Add(
                new Encuentro
                {
                    IdEncuentro =
                        reader.GetInt32("id_encuentro"),

                    Alumno1 =
                        reader["alumno1"]?.ToString() ?? "",

                    Alumno2 =
                        reader["alumno2"]?.ToString() ?? "PASE",

                    IdAlumno1 =
                        reader["id_alumno_1"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["id_alumno_1"]),

                    IdAlumno2 =
                        reader["id_alumno_2"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["id_alumno_2"]),

                    IdGanador =
                        reader["id_ganador"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(reader["id_ganador"]),

                    Fase =
                        reader["fase"]?.ToString() ?? "",

                    Estatus =
                        reader["estatus"]?.ToString() ?? ""
                    });
            }
        }    
    }

    public IActionResult OnPostSeleccionarGanador(
    int IdEncuentro,
    int IdGanador,
    int IdCategoria)
        {
            using var conn =
                _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"UPDATE encuentro
                SET
                    id_ganador = @ganador,
                    estatus = 'Terminado'
                WHERE id_encuentro = @encuentro";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@ganador",
                IdGanador);

            cmd.Parameters.AddWithValue(
                "@encuentro",
                IdEncuentro);

            cmd.ExecuteNonQuery();

            return RedirectToPage(
                new
                {
                    IdCategoria = IdCategoria
                });
        }
}