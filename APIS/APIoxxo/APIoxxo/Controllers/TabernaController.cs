using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TabernaController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        [HttpGet("{userId}")]
        public ActionResult<TabernaPoints> GetPoints(int userId)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(
                        "SELECT puntos, tiempo FROM taberna WHERE id_usuario = @userId ORDER BY id_taberna DESC LIMIT 1", 
                        connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return Ok(new TabernaPoints
                                {
                                    points = reader.GetFloat("puntos"),
                                    time = reader.GetTimeSpan("tiempo")
                                });
                            }

                            return Ok(new TabernaPoints
                            {
                                points = 0,
                                time = TimeSpan.Zero
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpPut("{userId}")]
        public ActionResult SavePoints(int userId, [FromBody] TabernaPoints pointsData)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand(@"
                        UPDATE taberna 
                        SET puntos = @points, tiempo = @time 
                        WHERE id_usuario = @userId", connection))
                    {
                        command.Parameters.AddWithValue("@points", pointsData.points);
                        command.Parameters.AddWithValue("@time", pointsData.time);
                        command.Parameters.AddWithValue("@userId", userId);

                        command.ExecuteNonQuery();
                        return Ok("Points updated successfully");
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    public class TabernaPoints
    {
        public float points { get; set; }
        public TimeSpan time { get; set; }
    }
}
