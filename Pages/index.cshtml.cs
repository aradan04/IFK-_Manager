using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySqlConnector;
using ProyectoIFK.Services;

namespace ProyectoIFK.Pages;

public class IndexModel : PageModel
{
    public string CustomCSS { get; set; } = string.Empty;

    private readonly ConexionBD _conexion;

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public string Mensaje { get; set; } = "";

    public IndexModel(ConexionBD conexion)
    {
        _conexion = conexion;
    }

    public void OnGet()
    {
        CustomCSS = @"
            body {
                margin: 0;
                background-color: #f4f4f4;
                display: flex;
                flex-direction: column;
                align-items: center;
                font-family: 'Segoe UI', sans-serif;
            }
            .top-banner {
                width: 100%;
                background-color: #1a1a1a;
                color: #d4af37;
                text-align: center;
                padding: 10px 0;
                font-weight: bold;
            }
            .login-container {
                margin-top: 30px;
                text-align: center;
                max-width: 450px;
            }
            .main-logo {
                width: 500px;
                margin-bottom: 40px;
            }
            label {
                display: block;
                font-weight: bold;
                margin-bottom: 10px;
                text-transform: uppercase;
            }
            input {
                width: 100%;
                padding: 12px;
                margin-bottom: 20px;
                border: 1px solid #ccc;
                border-radius: 5px;
                text-align: center;
            }
            .btn-login {
                background-color: #d4af37;
                color: #000;
                border: none;
                padding: 15px 40px;
                font-size: 1.2rem;
                font-weight: bold;
                border-radius: 8px;
                cursor: pointer;
                width: 100%;
            }
            .forgot-password {
                display: block;
                margin-top: 25px;
                color: #d4af37;
                text-decoration: none;
                font-style: italic;
            }";
    }

    public IActionResult OnPost(
        string Username,
        string Password)
    {
        try
        {
            using var conn =
                _conexion.ObtenerConexion();

            conn.Open();

            string sql =
                @"SELECT
                    id_usuario
                FROM usuario
                WHERE username = @username
                AND password = @password
                AND estatus = 'Activo'";

            using var cmd =
                new MySqlCommand(sql, conn);

            cmd.Parameters.AddWithValue(
                "@username",
                Username);

            cmd.Parameters.AddWithValue(
                "@password",
                Password);

            object? resultado =
                cmd.ExecuteScalar();

            if (resultado != null)
            {
                int idUsuario =
                    Convert.ToInt32(resultado);

                RegistrarAuditoria(
                    idUsuario,
                    "Inicio de sesión",
                    "Login");

                return RedirectToPage("/inicio");
            }

            Mensaje = "Usuario o contraseña incorrectos";
            OnGet();
            return Page();
        }
        catch(Exception ex)
        {
            Mensaje = ex.Message;
            OnGet();
            return Page();
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