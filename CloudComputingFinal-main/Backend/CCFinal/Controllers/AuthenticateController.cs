using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CCFinal.Dtos;
using CCFinal.Entities;
using CCFinal.PublishedEvents;
using DotNetCore.CAP;
using DotNetCore.CAP.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CCFinal.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticateController : ControllerBase {
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticateController> _logger;
    private readonly ICapPublisher _capPublisher;

    public AuthenticateController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        ILogger<AuthenticateController> logger,
        ICapPublisher capPublisher) {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _logger = logger;
        _capPublisher = capPublisher;
    }

    [HttpPost]
    [Route("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<IActionResult> Login([FromBody] LoginModel model) {
        var user = await _userManager.FindByNameAsync(model.Username);
        if ((user is not null && await _userManager.CheckPasswordAsync(user, model.Password)) || !ModelState.IsValid) {
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            List<Claim> authClaims = new() {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach (var userRole in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));

            var token = GetToken(authClaims);

            return Ok(new {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        return Unauthorized();
    }

    [HttpPost]
    [Route("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] RegisterModel model) {
        if (!ModelState.IsValid)
            return BadRequest();

        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists is not null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "User already exists!" });

        IdentityUser user = new() {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response
                    { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    [HttpPost]
    [Route("register-admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model) {
        if (!ModelState.IsValid)
            return BadRequest();

        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response { Status = "Error", Message = "User already exists!" });

        IdentityUser user = new() {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return StatusCode(StatusCodes.Status500InternalServerError,
                new Response
                    { Status = "Error", Message = "User creation failed! Please check user details and try again." });

        if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
        if (!await _roleManager.RoleExistsAsync(UserRoles.User))
            await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _userManager.AddToRoleAsync(user, UserRoles.Admin);
        if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            await _userManager.AddToRoleAsync(user, UserRoles.User);
        return Ok(new Response { Status = "Success", Message = "User created successfully!" });
    }

    [Authorize]
    [HttpPost("change-password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model) {
        if (!ModelState.IsValid)
            return BadRequest();

        try {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user is null)
                return NotFound();

            var pass = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (pass.Errors.Any()) {
                _logger.LogInformation($"Password change errors {pass.Errors}");
                return BadRequest("Unable to change password");
            }

            if (pass.Succeeded)
                return Ok(new Response { Status = "Success", Message = "Password changed successfully!" });
        }
        catch (Exception ex) {
            _logger.LogInformation(ex, "Unable to change password");
        }

        return BadRequest();
    }

    [Authorize]
    [HttpGet("check")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    public async Task<IActionResult> CheckAuthorization([FromHeader(Name = "Authorize")] string? token) {
        if (!ModelState.IsValid)
            return BadRequest();
        return Ok();
    }

    [Authorize]
    [HttpPost("add-CanvasKey")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult>
        AddCanvasIntegrationKey(CanvasApiModel model, CancellationToken cancellationToken = default) {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);

        if (user is null)
            return BadRequest();

        _logger.LogInformation($"""
            Received CanvasAPI model:
                {model.AccessToken}
                {model.CanvasUrl}
            """);

        await _capPublisher.PublishAsync("Integration.Canvas",
            new IntegrationCanvas(user.Id, model.AccessToken, model.CanvasUrl),
            new Dictionary<string, string> { { KafkaHeaders.KafkaKey, user.Id } }!,
            cancellationToken);

        return Ok();
    }

    private JwtSecurityToken GetToken(List<Claim> authClaims) {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            _configuration["JWT:ValidIssuer"],
            _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }
}

