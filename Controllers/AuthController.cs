using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using BiblioTek.Models;
using BiblioTek.Data;
using BiblioTek.Service;
using Microsoft.AspNetCore.Authorization;

namespace BiblioTek.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ApplicationDbContext _context;

        public AuthController(JwtService jwtService, ApplicationDbContext context)
        {
            _jwtService = jwtService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(Utilisateur user)
        {
            user.MotDePasse = BCrypt.Net.BCrypt.HashPassword(user.MotDePasse);
            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = user.Id }, user);
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            var user = _context.Utilisateurs.FirstOrDefault(u => u.Email == dto.Email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.MotDePasse, user.MotDePasse))
                return BadRequest("Identifiants invalides");

            var token = _jwtService.GenerateToken(user);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Ok(new { message = "Connexion réussie" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Ok(new { message = "Déconnexion réussie" });
        }

        [Authorize(Policy = "Admin")]
        [HttpPost("promote-to-admin/{userId}")]
        public IActionResult PromoteToAdmin(int userId)
        {
            var user = _context.Utilisateurs.Find(userId);
            if (user == null) return NotFound();

            user.Role = "Admin";
            _context.SaveChanges();

            return Ok(new { message = $"{user.Email} est maintenant administrateur" });
        }

        public class LoginDto
        {
            public string Email { get; set; }
            public string MotDePasse { get; set; }
        }
    }
}
