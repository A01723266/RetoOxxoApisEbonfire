using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace APIoxxo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly string _connectionString = "Server=mysql-3d246747-tec-acff.b.aivencloud.com;Port=25482;Database=oxxojuego;User Id=avnadmin;Password=AVNS_308DdCWk2oAlMYpGE-Q;SslMode=Required";

        public class InventoryItem
        {
            public int userId { get; set; }
            public bool item_1 { get; set; }
            public bool item_2 { get; set; }
            public bool item_3 { get; set; }
            public bool item_4 { get; set; }
        }

        [HttpGet("{userId}")]
        public ActionResult<InventoryItem> GetInventory(int userId)
        {
            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("get_inventory", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_user_id", userId);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var inventory = new InventoryItem
                                {
                                    userId = reader.GetInt32("id_usuario"),
                                    item_1 = reader.GetBoolean("item_1"),
                                    item_2 = reader.GetBoolean("item_2"),
                                    item_3 = reader.GetBoolean("item_3"),
                                    item_4 = reader.GetBoolean("item_4")
                                };
                                return Ok(inventory);
                            }
                            return NotFound($"No inventory found for user {userId}");
                        }
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }

        [HttpPut("{userId}/{itemNumber}")]
        public IActionResult UpdateInventoryItem(int userId, int itemNumber, [FromBody] bool value)
        {
            if (itemNumber < 1 || itemNumber > 4)
            {
                return BadRequest("Item number must be between 1 and 4");
            }

            try
            {
                using(var connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    using (var command = new MySqlCommand("update_inventory_item", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("p_user_id", userId);
                        command.Parameters.AddWithValue("p_item_number", itemNumber);
                        command.Parameters.AddWithValue("p_value", value);
                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            return Ok($"Item {itemNumber} updated for user {userId}");
                        }
                        return NotFound($"No inventory found for user {userId}");
                    }
                }
            }
            catch (MySqlException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
        }
    }
}