using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProyectoIFK.Data;
using ProyectoIFK.Models;
using ProyectoIFK.Services;
using MySqlConnector;

namespace ProyectoIFK.Pages
{
    public class FinanzasModel : PageModel
    {
        
        private readonly ApplicationDbContext _context;
        private readonly ConexionBD _conexion;

        public FinanzasModel(
            ApplicationDbContext context,
            ConexionBD conexion)
        {
            _context = context;
            _conexion = conexion;
        }

        // Lista para guardar las tarifas y mostrar en el select de la vista
        public List<Tarifa> ListaTarifas { get; set; } = new List<Tarifa>();

        // Método que se ejecuta al cargar la página
        public async Task OnGetAsync() 
        {
            // Cargamos todas las tarifas de la base de datos
            ListaTarifas = await _context.Tarifa.ToListAsync();

                    string? idGuardado =
                HttpContext.Session.GetString("IdUsuario");

            if(idGuardado != null)
            {
                int idUsuario =
                    Convert.ToInt32(idGuardado);

                RegistrarAuditoria(
                    idUsuario,
                    "Ingresó al módulo de finanzas",
                    "Finanzas");
            }
        }

        // Método para el buscador (lo que usa tu JS en la vista)
        public JsonResult OnGetBuscarAlumnos(string query)
        {
            if (string.IsNullOrEmpty(query)) return new JsonResult(new { });

            var resultados = _context.Alumno
                .Where(a => a.NombreCompleto.Contains(query) || a.Matricula.Contains(query))
                .Take(5)
                .Select(a => new { 
                    matricula = a.Matricula, 
                    nombre = a.NombreCompleto 
                })
                .ToList();

            return new JsonResult(resultados);
        }
    

            private void RegistrarAuditoria(
            int idUsuario,
            string accion,
            string modulo)
        {
            using var conn =
                _conexion.ObtenerConexion();

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
                HttpContext.Connection
                    .RemoteIpAddress?
                    .ToString());

            cmd.ExecuteNonQuery();
        }
    }
}