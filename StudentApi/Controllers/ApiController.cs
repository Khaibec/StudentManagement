using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentApi.Data;


[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    private readonly ApplicationDBContext _context;

    public TestController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet("database")]
    public IActionResult TestDatabase()
    {
        try
        {
            var connected = _context.Database.CanConnect();

            return Ok(new
            {
                connected,
                database = _context.Database.GetDbConnection().Database,
                server = _context.Database.GetDbConnection().DataSource
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                connected = false,
                error = ex.Message
            });
        }
    }
}
