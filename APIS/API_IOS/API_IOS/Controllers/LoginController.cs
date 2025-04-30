using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        [HttpPost]
        public ActionResult<LoginResponse> Login([FromBody] LoginRequest request)
        {
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        "SELECT id_usuario, nom_usuario, nombre FROM usuario WHERE nom_usuario = @username AND contraseña = @password", 
                        connection))
                    {
                        command.Parameters.AddWithValue("@username", request.Username);
                        command.Parameters.AddWithValue("@password", request.Password);
                        
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new LoginResponse { 
                                    Success = true,
                                    UserId = reader.GetInt32("id_usuario"),
                                    Username = reader.GetString("nom_usuario"),
                                    Nombre = reader.GetString("nombre")
                                });
                            }
                            return Ok(new LoginResponse { Success = false });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Nombre { get; set; }
    }
}