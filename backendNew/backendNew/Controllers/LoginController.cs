using backendNew.Dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ILogin _loginRepo;

    public LoginController(ILogin loginRepo)
    {
        _loginRepo = loginRepo;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var token = await _loginRepo.AuthenticateAsync(loginDto);

        if (token == null)
            return Unauthorized("Invalid credentials");

        return Ok(new { token });
    }
}