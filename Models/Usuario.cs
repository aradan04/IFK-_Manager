namespace ProyectoIFK.Models
{
    public class Usuario
    {
        public int IdUsuario { get; set; }

        public string NombreCompleto { get; set; } = "";

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string Rol { get; set; } = "";

        public string Estatus { get; set; } = "";
    }
}