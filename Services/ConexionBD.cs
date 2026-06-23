using Microsoft.Extensions.Configuration;
using MySqlConnector;

namespace ProyectoIFK.Services
{
    public class ConexionBD
    {
        private readonly IConfiguration _configuration;

        public ConexionBD(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public MySqlConnection ObtenerConexion()
        {
            string conexion =
                _configuration.GetConnectionString("DefaultConnection")!;

            return new MySqlConnection(conexion);
        }
    }
}