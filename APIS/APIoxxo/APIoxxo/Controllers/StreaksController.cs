using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StreaksController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        [HttpGet("{userId}")]
        public ActionResult<Streak> GetStreak(int userId)
        {
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new MySqlCommand("revisar_racha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@_userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            bool exists = reader.Read();
                            if (exists)
                            {
                                var streak = new Streak
                                {
                                    userId = reader.GetInt32("id_usuario"),
                                    streak = reader.GetInt32("streak_count")
                                };
                                reader.Close();
                                return Ok(streak);
                            }
                            reader.Close();
                            
                            // Si no existe el usuario, lo creamos con racha 0
                            using (var createCommand = new MySqlCommand("actualizar_racha", connection))
                            {
                                createCommand.CommandType = CommandType.StoredProcedure;
                                createCommand.Parameters.AddWithValue("@_userId", userId);
                                createCommand.Parameters.AddWithValue("@_racha", 0);
                                createCommand.ExecuteNonQuery();
                            }
                            return Ok(new Streak
                            {
                                userId = userId,
                                streak = 0
                            });
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public IActionResult UpdateStreak(int userId, [FromBody] Streak streakData)
        {
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = new MySqlCommand("actualizar_racha", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@_userId", userId);
                        command.Parameters.AddWithValue("@_racha", streakData.streak);
                        command.ExecuteNonQuery();
                    }
                }
                return Ok($"Streak updated for user {userId}");
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

}