using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using ProyectoIFK.Models;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages.personal{

    public class ControlModel : PageModel{
        private readonly ConexionBD _conexion;

        public List<Usuario> Usuarios { get; set; } = new();

        public string Mensaje { get; set; } = "";

        [BindProperty]
        public int IdUsuario { get; set; }

        [BindProperty]
        public string NombreCompleto { get; set; } = "";

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        [BindProperty]
        public string Rol { get; set; } = "";

        public ControlModel(ConexionBD conexion)
        {
            _conexion = conexion;
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

        public void OnGet(int? idEditar){
            try{
                using var conn = _conexion.ObtenerConexion();

                conn.Open();

                string sql =
                    @"SELECT
                        id_usuario,
                        nombre_completo,
                        username,
                        rol,
                        estatus
                    FROM usuario";

                using var cmd = new MySqlCommand(sql, conn);

                using var reader = cmd.ExecuteReader();

                while (reader.Read()){
                    Usuarios.Add(new Usuario
                    {
                        IdUsuario = reader.GetInt32("id_usuario"),
                        NombreCompleto = reader.GetString("nombre_completo"),
                        Username = reader.GetString("username"),
                        Rol = reader.GetString("rol"),
                        Estatus = reader.GetString("estatus")
                    });
                }

                Mensaje = $"Usuarios encontrados: {Usuarios.Count}";
            } catch (Exception ex){
                Mensaje = ex.Message;
            }

            if (idEditar.HasValue){
                using var conn2 = _conexion.ObtenerConexion();

                conn2.Open();

                string sqlEditar =
                    @"SELECT *
                    FROM usuario
                    WHERE id_usuario = @id";

                using var cmdEditar =
                    new MySqlCommand(sqlEditar, conn2);

                cmdEditar.Parameters.AddWithValue(
                    "@id",
                    idEditar.Value);

                using var readerEditar =
                    cmdEditar.ExecuteReader();

                if (readerEditar.Read()){
                    IdUsuario =
                        readerEditar.GetInt32("id_usuario");

                    NombreCompleto =
                        readerEditar.GetString("nombre_completo");

                    Username =
                        readerEditar.GetString("username");

                    Rol =
                        readerEditar.GetString("rol");
                }
            }
        }

        public IActionResult OnPost(){
            try
            {
                using var conn = _conexion.ObtenerConexion();

                conn.Open();

                string sql;

                if (IdUsuario > 0)
                {
                    sql =
                        @"UPDATE usuario
                        SET nombre_completo = @nombre,
                            username = @username,
                            rol = @rol
                        WHERE id_usuario = @id";
                }
                else
                {
                    sql =
                        @"INSERT INTO usuario
                            (
                                nombre_completo,
                                username,
                                password,
                                rol,
                                estatus
                            )
                        VALUES
                            (
                                @nombre,
                                @username,
                                @password,
                                @rol,
                                'Activo'
                            )";
                }

                using var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@nombre", NombreCompleto);
                cmd.Parameters.AddWithValue("@username", Username);
                cmd.Parameters.AddWithValue("@rol", Rol);

                if (IdUsuario > 0)
                {
                    cmd.Parameters.AddWithValue("@id", IdUsuario);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@password", Password);
                }

                cmd.ExecuteNonQuery();

                if (IdUsuario > 0)
                {
                    RegistrarAuditoria(
                        IdUsuario,
                        $"Modificó usuario {Username}",
                        "Control de Acceso");
                }
                else
                {
                    RegistrarAuditoria(
                        1,
                        $"Creó usuario {Username}",
                        "Control de Acceso");
                }

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                Mensaje = ex.Message;

                OnGet(null);

                return Page();
            }
        }
        public IActionResult OnPostCambiarEstado(int idUsuario){
            try
            {
                using var conn = _conexion.ObtenerConexion();

                conn.Open();

                string sql =
                    @"UPDATE usuario
                    SET estatus =
                        CASE
                            WHEN estatus = 'Activo'
                            THEN 'Inactivo'
                            ELSE 'Activo'
                        END
                    WHERE id_usuario = @id";

                using var cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@id", idUsuario);

            cmd.ExecuteNonQuery();

            RegistrarAuditoria(
                idUsuario,
                "Cambió estado del usuario",
                "Control de Acceso");

            return RedirectToPage();
            }
            catch
            {
                return RedirectToPage();
            }
        }
    }
}