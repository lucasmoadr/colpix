using System;
using System.Threading.Tasks;
using Colpix.Data.Models;
using Colpix.Services;
using Microsoft.AspNetCore.Mvc;

namespace Colpix.Controllers
{
    /// <summary>
    /// Controlador de autenticación de usuarios.
    /// Proporciona endpoints para iniciar sesión y registrarse.
    /// </summary>
    /// <remarks>
    /// Esta API utiliza autenticación basada en **JWT (Bearer Token)**.  
    /// 
    /// - ?? **/Auth/login** ? genera un token JWT para el usuario autenticado.  
    /// - ?? **/Auth/register** ? registra un nuevo usuario.  
    /// 
    /// Una vez obtenido el token, debe enviarse en el encabezado HTTP:
    /// 
    ///     Authorization: Bearer {tu_token_jwt}
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Modelo de solicitud para el inicio de sesión.
        /// </summary>
        public record LoginRequest(string Username, string Password);

        /// <summary>
        /// Modelo de respuesta al iniciar sesión correctamente.
        /// </summary>
        public record LoginResponse(string Token, DateTime Expires);

        /// <summary>
        /// Modelo de solicitud para registrar un nuevo usuario.
        /// </summary>
        public record RegisterRequest(string Username, string Email, string Password);

        /// <summary>
        /// Inicia sesión con las credenciales del usuario y devuelve un token JWT.
        /// </summary>
        /// <param name="req">Credenciales del usuario: nombre de usuario y contraseña.</param>
        /// <remarks>
        /// Ejemplo de solicitud:
        /// 
        ///     POST /Auth/login
        ///     {
        ///       "username": "colpix",
        ///       "password": "colpix"
        ///     }
        /// 
        /// Ejemplo de respuesta (200 OK):
        /// 
        ///     {
        ///       "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        ///       "expires": "2025-11-05T22:45:00Z"
        ///     }
        /// 
        /// Ejemplo de respuesta (401 Unauthorized):
        /// 
        ///     {
        ///       "error": "Credenciales inválidas."
        ///     }
        /// </remarks>
        /// <response code="200">Inicio de sesión exitoso. Devuelve el token JWT.</response>
        /// <response code="400">Solicitud inválida (faltan campos o formato incorrecto).</response>
        /// <response code="401">Credenciales incorrectas o usuario no autorizado.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(LoginRequest req)
        {
            var token = await _authService.LoginAsync(req.Username, req.Password);
            if (token == null)
                return Unauthorized(new { error = "Credenciales inválidas." });

            return Ok(new LoginResponse(token.Token, token.Expires));
        }

        /// <summary>
        /// Registra un nuevo usuario en el sistema.
        /// </summary>
        /// <param name="req">Datos del nuevo usuario: nombre, correo y contraseña.</param>
        /// <remarks>
        /// Ejemplo de solicitud:
        /// 
        ///     POST /Auth/register
        ///     {
        ///       "username": "nuevoUsuario",
        ///       "email": "nuevo@correo.com",
        ///       "password": "123456"
        ///     }
        /// 
        /// Ejemplo de respuesta (201 Created):
        /// 
        ///     {
        ///       "id": 12,
        ///       "username": "nuevoUsuario"
        ///     }
        /// 
        /// Ejemplo de respuesta (409 Conflict):
        /// 
        ///     "El usuario o correo ya existe."
        /// </remarks>
        /// <response code="201">Usuario registrado correctamente.</response>
        /// <response code="400">Solicitud inválida o datos incompletos.</response>
        /// <response code="409">El usuario o correo ya está registrado.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register(RegisterRequest req)
        {
            var result = await _authService.RegisterAsync(req.Username, req.Email, req.Password);

            if (!result.Created)
                return Conflict(result.Error);

            var u = result.User!;
            return CreatedAtAction(nameof(Register), new { id = u.Id }, new { u.Id, u.Username });
        }
    }
}
