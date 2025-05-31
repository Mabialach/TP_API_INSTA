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
using Microsoft.EntityFrameworkCore;


namespace BiblioTek.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavorisController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FavorisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/favoris (mes favoris)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livre>>> GetFavoris()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            return await _context.Favoris
                .Where(f => f.UtilisateurId == userId)
                .Include(f => f.Livre)
                .ThenInclude(l => l.Genre)
                .Select(f => f.Livre)
                .ToListAsync();
        }

        // POST: api/favoris/5
        [HttpPost("{livreId}")]
        public async Task<IActionResult> AddFavorite(int livreId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Vérifier si le livre existe
            if (!await _context.Livres.AnyAsync(l => l.Id == livreId))
                return NotFound("Livre non trouvé");

            // Vérifier si déjà en favoris
            if (await _context.Favoris.AnyAsync(f => f.UtilisateurId == userId && f.LivreId == livreId))
                return BadRequest("Déjà en favoris");

            var favorite = new Favori
            {
                UtilisateurId = userId,
                LivreId = livreId
            };

            _context.Favoris.Add(favorite);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // DELETE: api/favoris/5
        [HttpDelete("{livreId}")]
        public async Task<IActionResult> RemoveFavorite(int livreId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var favorite = await _context.Favoris
                .FirstOrDefaultAsync(f => f.UtilisateurId == userId && f.LivreId == livreId);

            if (favorite == null) return NotFound();

            _context.Favoris.Remove(favorite);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
