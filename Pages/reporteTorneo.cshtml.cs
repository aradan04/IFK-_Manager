using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using ProyectoIFK.Models;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages
{
    public class ReporteTorneoModel : PageModel
    {
        private readonly ConexionBD _conexion;

        public List<TorneoReporte> Torneos { get; set; } = new();

        public ReporteTorneoModel(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public void OnGet()
        {
            using var conn = _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"SELECT
                    id_torneo,
                    nombre_evento,
                    fecha,
                    sede,
                    organizador
                FROM torneo
                ORDER BY fecha DESC";

            using var cmd =
                new MySqlCommand(sql, conn);

            using var reader =
                cmd.ExecuteReader();

            while (reader.Read())
            {
                Torneos.Add(new TorneoReporte
                {
                    IdTorneo = reader.GetInt32("id_torneo"),
                    NombreEvento = reader.GetString("nombre_evento"),
                    FechaEvento = reader.GetDateTime("fecha"),
                    Sede = reader.GetString("sede"),
                    Organizador =
                        reader.IsDBNull(reader.GetOrdinal("organizador"))
                        ? "-"
                        : reader.GetString("organizador")
                });
            }

            string? idGuardado =
                HttpContext.Session.GetString("IdUsuario");

            if(idGuardado != null)
            {
                int idUsuario =
                    Convert.ToInt32(idGuardado);

                RegistrarAuditoria(
                    idUsuario,
                    $"Generó el reportes de torneos",
                    "Reportes");
            }

        }

        private void RegistrarAuditoria(
            int idUsuario,
            string accion,
            string modulo)
        {
            using var conn = _conexion.ObtenerConexion();

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
                HttpContext.Connection.RemoteIpAddress?.ToString());

            cmd.ExecuteNonQuery();
        }
    }
}