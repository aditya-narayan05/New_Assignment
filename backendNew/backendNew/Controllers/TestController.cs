using backendNew.Model;
using Microsoft.AspNetCore.Mvc;

namespace backendNew.Controllers
{
  
        [ApiController]
        [Route("api/[controller]")]
        public class TestController : ControllerBase
        {
            [HttpGet("success")]
            public IActionResult Success()
            {
                return Ok(new { message = "Success", timestamp = DateTime.UtcNow });
            }

            [HttpPost("create")]
            public IActionResult Create([FromBody] TestModel model)
            {
                return CreatedAtAction(nameof(Success), new { id = 1 }, model);
            }

            [HttpGet("unhandled-error")]
            public IActionResult UnhandledException()
            {
                throw new InvalidOperationException("This is an unhandled exception");
            }

            [HttpGet("handled-error")]
            public IActionResult HandledException()
            {
                try
                {
                    throw new ArgumentException("This exception is handled");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Handled error: {ex.Message}");
                }
            }

            [HttpGet("validation-error")]
            public IActionResult ValidationError([FromQuery] string required)
            {
                if (string.IsNullOrEmpty(required))
                {
                    return BadRequest("Required parameter is missing");
                }
                return Ok();
            }
        }

       
    }
