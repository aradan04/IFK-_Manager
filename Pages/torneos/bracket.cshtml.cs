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

    public List<Encuentro> PrimeraRonda { get; set; } = new();

    public List<Encuentro> Semifinales { get; set; } = new();

    public List<Encuentro> Finales { get; set; } = new();

    public string CampeonNombre { get; set; } = "";

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
        WHERE E.id_categoria = @idCategoria
        ORDER BY E.id_encuentro";

        using var cmdEncuentros =
            new MySqlCommand(sqlEncuentros, conn);

        cmdEncuentros.Parameters.AddWithValue(
            "@idCategoria",
            IdCategoria);

        using var reader =
            cmdEncuentros.ExecuteReader();

        while(reader.Read())
        {
            var encuentro = new Encuentro
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
            };

            Encuentros.Add(encuentro);

            if(encuentro.Fase == "Primera Ronda")
            {
                PrimeraRonda.Add(encuentro);
            }
        }
        } 

        string sqlSemifinales =
        @"SELECT
            E.id_encuentro,
            E.id_alumno_1,
            E.id_alumno_2,
            E.id_ganador,
            A1.nombre_completo AS alumno1,
            A2.nombre_completo AS alumno2
        FROM encuentro E
        LEFT JOIN alumno A1
            ON A1.id_alumno = E.id_alumno_1
        LEFT JOIN alumno A2
            ON A2.id_alumno = E.id_alumno_2
        WHERE E.id_categoria = @idCategoria
        AND E.fase = 'Semifinal'";

        using var cmdSemi =
            new MySqlCommand(sqlSemifinales, conn);

        cmdSemi.Parameters.AddWithValue(
            "@idCategoria",
            IdCategoria);

        using var readerSemi =
            cmdSemi.ExecuteReader();

        while(readerSemi.Read())
        {
                Semifinales.Add(
        new Encuentro
        {
            IdEncuentro =
                Convert.ToInt32(readerSemi["id_encuentro"]),

            IdAlumno1 =
                Convert.ToInt32(readerSemi["id_alumno_1"]),

            IdAlumno2 =
                Convert.ToInt32(readerSemi["id_alumno_2"]),

            IdGanador =
                readerSemi["id_ganador"] == DBNull.Value
                    ? null
                    : Convert.ToInt32(readerSemi["id_ganador"]),

            Alumno1 =
                readerSemi["alumno1"]?.ToString() ?? "",

            Alumno2 =
                readerSemi["alumno2"]?.ToString() ?? ""
        });
        }

        readerSemi.Close();

        string sqlFinal =
        @"SELECT
            E.id_encuentro,
            E.id_alumno_1,
            E.id_alumno_2,
            E.id_ganador,
            A1.nombre_completo AS alumno1,
            A2.nombre_completo AS alumno2
        FROM encuentro E
        LEFT JOIN alumno A1
            ON A1.id_alumno = E.id_alumno_1
        LEFT JOIN alumno A2
            ON A2.id_alumno = E.id_alumno_2
        WHERE E.id_categoria = @idCategoria
        AND E.fase = 'Final'";

        using var cmdFinal =
            new MySqlCommand(sqlFinal, conn);

        cmdFinal.Parameters.AddWithValue(
            "@idCategoria",
            IdCategoria);

        using var readerFinal =
            cmdFinal.ExecuteReader();

        while(readerFinal.Read())
        {
            Finales.Add(
                new Encuentro
                {
                    IdEncuentro =
                        Convert.ToInt32(
                            readerFinal["id_encuentro"]),

                    Alumno1 =
                        readerFinal["alumno1"]?.ToString() ?? "",

                    Alumno2 =
                        readerFinal["alumno2"]?.ToString() ?? "",

                    IdAlumno1 =
                        Convert.ToInt32(
                            readerFinal["id_alumno_1"]),

                    IdAlumno2 =
                        Convert.ToInt32(
                            readerFinal["id_alumno_2"]),

                    IdGanador =
                        readerFinal["id_ganador"] == DBNull.Value
                            ? null
                            : Convert.ToInt32(
                                readerFinal["id_ganador"])
                });
        }

        readerFinal.Close();

        string sqlCampeon =
        @"SELECT A.nombre_completo
        FROM categoria_torneo C
        INNER JOIN alumno A
            ON A.id_alumno = C.campeon
        WHERE C.id_categoria = @idCategoria";

        using var cmdCampeon =
            new MySqlCommand(sqlCampeon, conn);

        cmdCampeon.Parameters.AddWithValue(
            "@idCategoria",
            IdCategoria);

        object? nombre =
            cmdCampeon.ExecuteScalar();

        if(nombre != null)
        {
            CampeonNombre =
                nombre.ToString() ?? "";
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

            GenerarSemifinales(
            conn,
            IdCategoria);

            GenerarFinal(
            conn,
            IdCategoria);

            GenerarCampeon(conn, IdCategoria);

            return RedirectToPage(
                new
                {
                    IdCategoria = IdCategoria
                });
        }

        private void GenerarSemifinales(
            MySqlConnection conn,
            int idCategoria)
        {
            List<int> ganadores = new();

            string sql =
                @"SELECT id_ganador
                FROM encuentro
                WHERE id_categoria = @idCategoria
                AND fase = 'Primera Ronda'
                AND estatus = 'Terminado'
                ORDER BY id_encuentro";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@idCategoria",
                idCategoria);

            using var reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                ganadores.Add(
                    Convert.ToInt32(
                        reader["id_ganador"]));
            }

            reader.Close();

            if(ganadores.Count < 4)
                return;

            string verificar =
                @"SELECT COUNT(*)
                FROM encuentro
                WHERE id_categoria = @idCategoria
                AND fase = 'Semifinal'";

            using var cmdVerificar =
                new MySqlCommand(verificar, conn);

            cmdVerificar.Parameters.AddWithValue(
                "@idCategoria",
                idCategoria);

            int existentes =
                Convert.ToInt32(
                    cmdVerificar.ExecuteScalar());

            if(existentes > 0)
                return;

            for(int i = 0; i < ganadores.Count; i += 2)
            {
                string insert =
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
                        1,
                        @categoria,
                        @alumno1,
                        @alumno2,
                        'Semifinal',
                        'Pendiente'
                    )";

                using var cmdInsert =
                    new MySqlCommand(insert, conn);

                cmdInsert.Parameters.AddWithValue(
                    "@categoria",
                    idCategoria);

                cmdInsert.Parameters.AddWithValue(
                    "@alumno1",
                    ganadores[i]);

                cmdInsert.Parameters.AddWithValue(
                    "@alumno2",
                    ganadores[i + 1]);

                cmdInsert.ExecuteNonQuery();
            }
        }

        private void GenerarFinal(
            MySqlConnection conn,
            int idCategoria)
        {
            List<int> ganadores = new();

            string sql =
                @"SELECT id_ganador
                FROM encuentro
                WHERE id_categoria = @idCategoria
                AND fase = 'Semifinal'
                AND estatus = 'Terminado'
                ORDER BY id_encuentro";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@idCategoria",
                idCategoria);

            using var reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                ganadores.Add(
                    Convert.ToInt32(
                        reader["id_ganador"]));
            }

            reader.Close();

            Console.WriteLine(
                $"SEMIFINALES TERMINADAS: {ganadores.Count}");


            if(ganadores.Count < 2)
            {
                Console.WriteLine(
                    "NO HAY SUFICIENTES GANADORES");

                return;
            }

            string verificar =
                @"SELECT COUNT(*)
                FROM encuentro
                WHERE id_categoria = @idCategoria
                AND fase = 'Final'";

            using var cmdVerificar =
                new MySqlCommand(verificar, conn);

            cmdVerificar.Parameters.AddWithValue(
                "@idCategoria",
                idCategoria);

            int existentes =
                Convert.ToInt32(
                    cmdVerificar.ExecuteScalar());

            if(existentes > 0)
                return;

            string insert =
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
                    1,
                    @categoria,
                    @alumno1,
                    @alumno2,
                    'Final',
                    'Pendiente'
                )";

            using var cmdInsert =
                new MySqlCommand(insert, conn);

            cmdInsert.Parameters.AddWithValue(
                "@categoria",
                idCategoria);

            cmdInsert.Parameters.AddWithValue(
                "@alumno1",
                ganadores[0]);

            cmdInsert.Parameters.AddWithValue(
                "@alumno2",
                ganadores[1]);

            Console.WriteLine(
            $"INSERTANDO FINAL: {ganadores[0]} vs {ganadores[1]}");

            cmdInsert.ExecuteNonQuery();
        }

        private void GenerarCampeon(
            MySqlConnection conn,
            int idCategoria)
        {
            string sql =
                @"SELECT id_ganador
                FROM encuentro
                WHERE id_categoria = @idCategoria
                AND fase = 'Final'
                AND estatus = 'Terminado'";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@idCategoria",
                idCategoria);

            object? resultado =
                cmd.ExecuteScalar();

            if(resultado == null)
                return;

            int idCampeon =
                Convert.ToInt32(resultado);

            string actualizar =
                @"UPDATE categoria_torneo
                SET campeon = @campeon
                WHERE id_categoria = @categoria";

            using var cmdUpdate =
                new MySqlCommand(actualizar, conn);

            cmdUpdate.Parameters.AddWithValue(
                "@campeon",
                idCampeon);

            cmdUpdate.Parameters.AddWithValue(
                "@categoria",
                idCategoria);

            cmdUpdate.ExecuteNonQuery();
        }
}