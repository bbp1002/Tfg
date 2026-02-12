using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TFG_Cultivos.Models;

namespace TFG_Cultivos.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _config;

    // El controlador recibe el UserManager para gestionar usuarios
    // y IConfiguration para acceder a la clave JWT configurada en appsettings.json.
    public AuthController(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    // --------------------------------------------------------------------
    // POST api/auth/register
    // Registra un nuevo usuario en el sistema utilizando Identity.
    // Recibe email y password como parámetros.
    // --------------------------------------------------------------------
    [HttpPost("register")]
    public async Task<IActionResult> Register(string email, string password)
    {
        var user = new ApplicationUser
        {
            UserName = email,
            Email = email
        };

        // Crea el usuario en la base de datos con Identity
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Usuario creado");
    }

    // --------------------------------------------------------------------
    // POST api/auth/login
    // Valida las credenciales del usuario y, si son correctas,
    // genera y devuelve un token JWT para autenticación.
    // --------------------------------------------------------------------
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Models.LoginRequest request)
    {
        var email = request.Email; 
        var password = request.Password;

        // Busca al usuario por email
        var user = await _userManager.FindByEmailAsync(email);

        // Comprueba si existe y si la contraseña es correcta
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            return Unauthorized();

        // Genera el token JWT
        var token = GenerarJwt(user);

        return Ok(new { token });
    }

    // --------------------------------------------------------------------
    // Método privado: GenerarJwt
    // Crea un token JWT con los claims básicos del usuario.
    // El token incluye:
    //   - NameIdentifier (ID del usuario)
    //   - Email
    //
    // El token se firma con la clave configurada en appsettings.json
    // y tiene una duración de 8 horas.
    // --------------------------------------------------------------------
    private string GenerarJwt(ApplicationUser user)
    {
        // Claims que se incluirán en el token
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Clave secreta para firmar el token
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Construcción del token
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(8),
            signingCredentials: creds);

        // Serializa el token a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
