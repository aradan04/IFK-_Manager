using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ProyectoIFK.Models;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages.personal
{
    public class AuditoriaModel : PageModel
    {
        private readonly ConexionBD _conexion;

        public List<AuditoriaLog> Registros { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string Busqueda { get; set; } = "";

        public AuditoriaModel(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public void OnGet()
        {
            using var conn =
                _conexion.ObtenerConexion();

            conn.Open();

            string sql =
            @"SELECT
                l.id_log,
                u.username,
                l.accion,
                l.modulo,
                l.fecha_hora,
                l.ip_direccion
            FROM log_auditoria l
            INNER JOIN usuario u
                ON l.id_usuario = u.id_usuario
            WHERE
                (@busqueda = ''
                OR u.username LIKE @texto
                OR l.accion LIKE @texto
                OR l.modulo LIKE @texto)
            ORDER BY l.fecha_hora DESC";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@busqueda",
                Busqueda);

            cmd.Parameters.AddWithValue(
                "@texto",
                "%" + Busqueda + "%");

            using var reader =
                cmd.ExecuteReader();

            while(reader.Read())
            {
                Registros.Add(new AuditoriaLog
                {
                    IdLog =
                        reader.GetInt32("id_log"),

                    Usuario =
                        reader.GetString("username"),

                    Accion =
                        reader.GetString("accion"),

                    Modulo =
                        reader.GetString("modulo"),

                    FechaHora =
                        reader.GetDateTime("fecha_hora"),

                    IpDireccion =
                        reader.GetString("ip_direccion")
                });
            }
        }
    }
}