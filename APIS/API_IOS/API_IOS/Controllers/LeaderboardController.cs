using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeaderboardController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        [HttpGet]
        public ActionResult GetLeaderboard()
        {
            try
            {
                var leaderboard = new List<LeaderboardEntry>();
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using(var command = new MySqlCommand(@"
                        SELECT 
                            u.id_usuario,
                            u.nom_usuario, 
                            (COALESCE(t.total_puntos, 0) + COALESCE(d.total_puntos, 0)) as puntos
                        FROM usuario u
                        LEFT JOIN (
                            SELECT id_usuario, SUM(1000 - puntos) as total_puntos 
                            FROM taberna 
                            GROUP BY id_usuario
                        ) t ON u.id_usuario = t.id_usuario
                        LEFT JOIN (
                            SELECT id_usuario, SUM(puntos) as total_puntos 
                            FROM decision 
                            GROUP BY id_usuario
                        ) d ON u.id_usuario = d.id_usuario
                        ORDER BY puntos DESC ", connection))
                    {
                        using(var reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                leaderboard.Add(new LeaderboardEntry
                                {
                                    UserId = reader.GetInt32("id_usuario"),
                                    Username = reader.GetString("nom_usuario"),
                                    Points = reader.GetFloat("puntos")
                                });
                            }
                        }
                    }
                }
                return Ok(leaderboard);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    public class LeaderboardEntry
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public float Points { get; set; }
    }
}