using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages
{
    public class ReportePagosModel : PageModel
    {
        private readonly ConexionBD _conexion;

        public List<PagoReporte> Pagos { get; set; } = new();

        public ReportePagosModel(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public void OnGet()
        {
            using var conn = _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"SELECT
                    A.nombre_completo,
                    P.concepto,
                    P.monto,
                    P.metodo_pago,
                    P.fecha_pago
                FROM pago P
                JOIN alumno A
                    ON P.id_alumno = A.id_alumno
                ORDER BY P.fecha_pago DESC";

            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Pagos.Add(new PagoReporte
                {
                    NombreCompleto =
                        reader.GetString("nombre_completo"),

                    Concepto =
                        reader.GetString("concepto"),

                    Monto =
                        reader.GetDecimal("monto"),

                    MetodoPago =
                        reader.GetString("metodo_pago"),

                    FechaPago =
                        reader.GetDateTime("fecha_pago")
                });
            }

            RegistrarAuditoria(
                1,
                "Generó reporte de esatdos de cuenta",
                "Reportes");
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

    public class PagoReporte
    {
        public string NombreCompleto { get; set; } = "";

        public string Concepto { get; set; } = "";

        public decimal Monto { get; set; }

        public string MetodoPago { get; set; } = "";

        public DateTime FechaPago { get; set; }
    }
}