using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PreguntasController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        // GET: Obtener todas las preguntas
        [HttpGet]
        public ActionResult<IEnumerable<Pregunta>> GetPreguntas()
        {
            try
            {
                var preguntas = new List<Pregunta>();
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using(var command = new MySqlCommand("SELECT id_pregunta, pregunta, respuesta_correcta FROM preguntas_decision", connection))
                    {
                        using(var reader = command.ExecuteReader())
                        {
                            while(reader.Read())
                            {
                                preguntas.Add(new Pregunta
                                {
                                    Id = reader.GetInt32("id_pregunta"),
                                    TextoPregunta = reader.GetString("pregunta"),
                                    RespuestaCorrecta = reader.GetBoolean("respuesta_correcta")
                                });
                            }
                        }
                    }
                }
                return Ok(preguntas);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        // POST: Crear una nueva pregunta
        [HttpPost]
        public ActionResult CreatePregunta([FromBody] Pregunta pregunta)
        {
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using(var command = new MySqlCommand("INSERT INTO preguntas_decision (pregunta, respuesta_correcta) VALUES (@pregunta, @respuesta)", connection))
                    {
                        command.Parameters.AddWithValue("@pregunta", pregunta.TextoPregunta);
                        command.Parameters.AddWithValue("@respuesta", pregunta.RespuestaCorrecta);
                        command.ExecuteNonQuery();
                    }
                }
                return Ok("Pregunta creada exitosamente");
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }

    public class Pregunta
    {
        public int Id { get; set; }
        public string TextoPregunta { get; set; }
        public bool RespuestaCorrecta { get; set; }
    }
}