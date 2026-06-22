using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages
{
    public class ReporteAlumnosModel : PageModel
    {
        private readonly ConexionBD _conexion;

        public List<AlumnoReporte> Alumnos { get; set; } = new();

        public ReporteAlumnosModel(ConexionBD conexion)
        {
            _conexion = conexion;
        }

        public void OnGet()
        {
            using var conn = _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"SELECT
                    matricula,
                    nombre_completo,
                    sexo,
                    kyu_actual,
                    peso,
                    estatus_medico
                FROM alumno";

            using var cmd = new MySqlCommand(sql, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Alumnos.Add(new AlumnoReporte
                {
                    Matricula = reader.GetString("matricula"),
                    NombreCompleto = reader.GetString("nombre_completo"),
                    Sexo = reader.GetString("sexo"),
                    KyuActual = reader.GetString("kyu_actual"),
                    Peso = reader.GetDecimal("peso"),
                    EstatusMedico = reader.GetString("estatus_medico")
                });
            }

            RegistrarAuditoria(
            1,
            "Generó reporte de alumnos",
            "Reportes");
        }

        private void RegistrarAuditoria(int idUsuario,string accion,string modulo){
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

    public class AlumnoReporte
    {
        public string Matricula { get; set; } = "";
        public string NombreCompleto { get; set; } = "";
        public string Sexo { get; set; } = "";
        public string KyuActual { get; set; } = "";
        public decimal Peso { get; set; }
        public string EstatusMedico { get; set; } = "";
    }

    
}
