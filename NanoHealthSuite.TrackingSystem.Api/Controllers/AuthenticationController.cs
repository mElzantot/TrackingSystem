using Microsoft.AspNetCore.Mvc;
using NanoHealthSuite.TrackingSystem.Services;

namespace NanoHealthSuite.TrackingSystem.Api.Controllers;

[Route("api/v1/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _authenticationService;

    public AuthenticationController(AuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(AuthRequest authRequestDto)
    {
        if (await _authenticationService.CheckIfUserNameExist(authRequestDto.UserName))
            return BadRequest(new { Errors = "User Name is already exist , Please try another one" });
        var userTokenResult = await _authenticationService.Register(authRequestDto);
        return userTokenResult.IsSuccess ? Ok(userTokenResult.Value) : BadRequest(new {message = userTokenResult.Message});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var tokens = await _authenticationService.Login(request);
        return tokens == null ? BadRequest("User Name or Password is incorrect") : Ok(tokens);
    }
}