using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LogrosControllerTaberna : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        [HttpGet("{userId}")]
        public ActionResult<LogrosTabernaModel> GetLogros(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        "SELECT logro_tab1, logro_tab2, logro_tab3, logro_tab4 FROM Logros WHERE id_usuario = @userId",
                        connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new LogrosTabernaModel
                                {
                                    logro_tab1 = reader.GetBoolean("logro_tab1"),
                                    logro_tab2 = reader.GetBoolean("logro_tab2"),
                                    logro_tab3 = reader.GetBoolean("logro_tab3"),
                                    logro_tab4 = reader.GetBoolean("logro_tab4")
                                });
                            }
                            reader.Close();

                            // Si no existe, crear nuevo registro
                            using (var insertCommand = new MySqlCommand(
                                "INSERT INTO Logros (id_usuario, logro_tab1, logro_tab2, logro_tab3, logro_tab4) VALUES (@userId, false, false, false, false)",
                                connection))
                            {
                                insertCommand.Parameters.AddWithValue("@userId", userId);
                                insertCommand.ExecuteNonQuery();

                                return Ok(new LogrosTabernaModel
                                {
                                    logro_tab1 = false,
                                    logro_tab2 = false,
                                    logro_tab3 = false,
                                    logro_tab4 = false
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
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        $"UPDATE Logros SET logro_tab{logroNum} = true WHERE id_usuario = @userId",
                        connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                            return Ok($"Logro {logroNum} actualizado para el usuario {userId}");
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

    public class LogrosTabernaModel
    {
        public bool logro_tab1 { get; set; }
        public bool logro_tab2 { get; set; }
        public bool logro_tab3 { get; set; }
        public bool logro_tab4 { get; set; }
    }
}
