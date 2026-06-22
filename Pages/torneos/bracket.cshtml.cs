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

    public List<string> Competidores { get; set; } = new();

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
            string sqlCompetidores =
                @"SELECT
                    A.nombre_completo
                FROM categoria_alumno CA
                INNER JOIN alumno A
                    ON A.id_alumno = CA.id_alumno
                WHERE CA.id_categoria = @idCategoria";

            using var cmdCompetidores =
                new MySqlCommand(sqlCompetidores, conn);

            cmdCompetidores.Parameters.AddWithValue(
                "@idCategoria",
                IdCategoria);

            using var readerCompetidores =
                cmdCompetidores.ExecuteReader();

            while (readerCompetidores.Read())
            {
                Competidores.Add(
                    readerCompetidores.GetString(
                        "nombre_completo"));
            }
        }
    }
}