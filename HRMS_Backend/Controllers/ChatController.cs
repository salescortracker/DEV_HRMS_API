using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

[ApiController]
[Route("api/chat")]
public class ChatController : ControllerBase
{
    private readonly IConfiguration _config;

    public ChatController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost]
    public IActionResult Ask([FromBody] ChatRequest req)
    {
        string input = req.Message?.Trim();

        if (string.IsNullOrEmpty(input))
        {
            return Ok(new
            {
                type = "text",
                text = "Please enter a message."
            });
        }

        string connectionString = _config.GetConnectionString("DefaultConnection");

        using SqlConnection con = new SqlConnection(connectionString);

        string sql = @"
        SELECT TOP 1 *
        FROM chatbot.ChatbotKnowledge
        WHERE IsActive = 1
        AND (
            Question LIKE @search
            OR Keywords LIKE @search
        )";

        SqlCommand cmd = new SqlCommand(sql, con);
        cmd.Parameters.AddWithValue("@search", "%" + input + "%");

        con.Open();

        SqlDataReader dr = cmd.ExecuteReader();

        if (dr.Read())
        {
            return Ok(new
            {
                type = dr["CardType"].ToString(),
                title = dr["Question"].ToString(),
                text = dr["Answer"].ToString(),
                fileUrl = dr["FileUrl"] == DBNull.Value ? "" : dr["FileUrl"].ToString()
            });
        }

        return Ok(new
        {
            type = "text",
            text = "No matching answer found."
        });
    }
}

public class ChatRequest
{
    public string Message { get; set; }
}