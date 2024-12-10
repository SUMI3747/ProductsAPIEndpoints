using Microsoft.AspNetCore.Mvc;
using ProductInventoryManagerAPI.Models.DTOs;
using ProductInventoryManagerAPI.ProductServices;


namespace ProductInventoryManagerAPI.Controllers;
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly ITokenGenerationService _tokenService;
    public AuthController(ITokenGenerationService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDetails)
    {
        var response = await _tokenService.ReturnJWTTokenIfCredetialsAreValid(loginDetails);

        return string.IsNullOrEmpty(response) ? Unauthorized(response) : Ok(response);
    }

    [HttpPost("SignUp")]
    public async Task<ActionResult> SignUp([FromBody] UserCredetialsDto credetilaDeatails)
    {
        var response = await _tokenService.AddNewCredetialsToDB(credetilaDeatails);

        return string.IsNullOrEmpty(response) ? BadRequest(response) : Ok(response);
    }
}


