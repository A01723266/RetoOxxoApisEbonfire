using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogrosController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        [HttpGet("{userId}")]
        public ActionResult<Logros> GetLogros(int userId)
        {
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        "SELECT logro_dec1, logro_dec2, logro_dec3, logro_dec4 FROM Logros WHERE id_usuario = @userId", 
                        connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new Logros
                                {
                                    logro_dec1 = reader.GetBoolean("logro_dec1"),
                                    logro_dec2 = reader.GetBoolean("logro_dec2"),
                                    logro_dec3 = reader.GetBoolean("logro_dec3"),
                                    logro_dec4 = reader.GetBoolean("logro_dec4")
                                });
                            }
                            reader.Close();
                            
                            // Si no existe, crear nuevo registro
                            using (var insertCommand = new MySqlCommand(
                                "INSERT INTO Logros (id_usuario, logro_dec1, logro_dec2, logro_dec3, logro_dec4) VALUES (@userId, false, false, false, false)", 
                                connection))
                            {
                                insertCommand.Parameters.AddWithValue("@userId", userId);
                                insertCommand.ExecuteNonQuery();
                                
                                return Ok(new Logros
                                {
                                    logro_dec1 = false,
                                    logro_dec2 = false,
                                    logro_dec3 = false,
                                    logro_dec4 = false
                                });
                            }
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpPut("{userId}/{logroNum}")]
        public IActionResult UpdateLogro(int userId, int logroNum)
        {
            if (logroNum < 1 || logroNum > 4) return BadRequest("Logro number must be between 1 and 4");
            
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        $"UPDATE Logros SET logro_dec{logroNum} = true WHERE id_usuario = @userId", 
                        connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return Ok($"Logro {logroNum} updated for user {userId}");
                        return NotFound();
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    public class Logros
    {
        public bool logro_dec1 { get; set; }
        public bool logro_dec2 { get; set; }
        public bool logro_dec3 { get; set; }
        public bool logro_dec4 { get; set; }
    }
}