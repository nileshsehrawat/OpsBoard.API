using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new IdentityUser { UserName = email, Email = email };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("User created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(string email, string password)
    {
        var result = await _signInManager.PasswordSignInAsync(email, password, true, false);

        if (!result.Succeeded)
            return Unauthorized();

        return Ok("Logged in");
    }
}