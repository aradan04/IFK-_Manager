using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages
{
    public class ReporteAsistenciaModel : PageModel
    {
        private readonly ConexionBD _conexion;

        public List<AsistenciaReporte> Asistencias { get; set; } = new();

        public ReporteAsistenciaModel(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public void OnGet()
        {
            using var conn = _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"SELECT
                    A.fecha,
                    AL.matricula,
                    AL.nombre_completo,
                    A.estatus
                FROM asistencia A
                JOIN alumno AL
                    ON A.id_alumno = AL.id_alumno
                ORDER BY A.fecha DESC";

            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Asistencias.Add(new AsistenciaReporte
                {
                    Fecha = reader.GetDateTime("fecha"),
                    Matricula = reader.GetString("matricula"),
                    NombreCompleto = reader.GetString("nombre_completo"),
                    Estatus = reader.GetString("estatus")
                });

                RegistrarAuditoria(
                1,
                "Generó reporte de asistencia",
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

    public class AsistenciaReporte
    {
        public DateTime Fecha { get; set; }

        public string Matricula { get; set; } = "";

        public string NombreCompleto { get; set; } = "";

        public string Estatus { get; set; } = "";
    }
}