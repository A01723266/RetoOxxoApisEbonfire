using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreguntasTabernaController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        // GET: Obtener preguntas fáciles
        [HttpGet("faciles")]
        public ActionResult<IEnumerable<PreguntaTaberna>> GetFaciles()
        {
            return ObtenerPreguntasDesdeTabla("Preguntas_Faciles_Taberna");
        }

        // GET: Obtener preguntas difíciles
        [HttpGet("dificiles")]
        public ActionResult<IEnumerable<PreguntaTaberna>> GetDificiles()
        {
            return ObtenerPreguntasDesdeTabla("Preguntas_Dificiles_Taberna");
        }

        // POST: Crear una nueva pregunta fácil
        [HttpPost("faciles")]
        public ActionResult CreateFacil([FromBody] PreguntaTaberna pregunta)
        {
            return InsertarPregunta("Preguntas_Faciles_Taberna", pregunta);
        }

        // POST: Crear una nueva pregunta difícil
        [HttpPost("dificiles")]
        public ActionResult CreateDificil([FromBody] PreguntaTaberna pregunta)
        {
            return InsertarPregunta("Preguntas_Dificiles_Taberna", pregunta);
        }

        private ActionResult<IEnumerable<PreguntaTaberna>> ObtenerPreguntasDesdeTabla(string nombreTabla)
        {
            try
            {
                var preguntas = new List<PreguntaTaberna>();
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand($"SELECT * FROM {nombreTabla}", connection);
                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        preguntas.Add(new PreguntaTaberna
                        {
                            Id = reader.GetInt32("id"),
                            Pregunta = reader.GetString("pregunta"),
                            Respuesta1 = reader.GetString("respuesta1"),
                            Respuesta2 = reader.GetString("respuesta2"),
                            Respuesta3 = reader.GetString("respuesta3"),
                            Respuesta4 = reader.GetString("respuesta4"),
                            RespuestaCorrecta = reader.GetString("respuestaCorrecta")
                        });
                    }
                }
                return Ok(preguntas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        private ActionResult InsertarPregunta(string nombreTabla, PreguntaTaberna pregunta)
        {
            try
            {
                using (var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    var command = new MySqlCommand($@"
                        INSERT INTO {nombreTabla} (pregunta, respuesta1, respuesta2, respuesta3, respuesta4, respuestaCorrecta)
                        VALUES (@pregunta, @r1, @r2, @r3, @r4, @rc)", connection);

                    command.Parameters.AddWithValue("@pregunta", pregunta.Pregunta);
                    command.Parameters.AddWithValue("@r1", pregunta.Respuesta1);
                    command.Parameters.AddWithValue("@r2", pregunta.Respuesta2);
                    command.Parameters.AddWithValue("@r3", pregunta.Respuesta3);
                    command.Parameters.AddWithValue("@r4", pregunta.Respuesta4);
                    command.Parameters.AddWithValue("@rc", pregunta.RespuestaCorrecta);

                    command.ExecuteNonQuery();
                }
                return Ok("Pregunta insertada correctamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    public class PreguntaTaberna
    {
        public int Id { get; set; }
        public string Pregunta { get; set; }
        public string Respuesta1 { get; set; }
        public string Respuesta2 { get; set; }
        public string Respuesta3 { get; set; }
        public string Respuesta4 { get; set; }
        public string RespuestaCorrecta { get; set; }
    }
}
