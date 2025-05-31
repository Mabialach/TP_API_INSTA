using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BiblioTek.Models;
using BiblioTek.Data;

namespace BiblioTek.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LivresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LivresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/livres (accessible à tous)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Livre>>> GetLivres()
        {
            return await _context.Livres.Include(l => l.Genre).ToListAsync();
        }

        // GET: api/livres/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Livre>> GetLivre(int id)
        {
            var livre = await _context.Livres
                .Include(l => l.Genre)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (livre == null) return NotFound();

            return livre;
        }

        // POST: api/livres (admin seulement)
        [Authorize(Policy = "Admin")]
        [HttpPost]
        public async Task<ActionResult<Livre>> PostLivre(Livre livre)
        {
            _context.Livres.Add(livre);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLivre), new { id = livre.Id }, livre);
        }

        // PATCH: api/livres/5 (admin seulement)
        [Authorize(Policy = "Admin")]
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchLivre(int id, Livre livre)
        {
            var livreToUpdate = await _context.Livres.FindAsync(id);
            if (livreToUpdate == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(livre.Titre))
                livreToUpdate.Titre = livre.Titre;

            // S'assurer que GenreId a été fourni explicitement (et qu'il est valide)
            if (livre.GenreId != 0)
            {
                var genreExiste = await _context.Genres.AnyAsync(g => g.Id == livre.GenreId);
                if (!genreExiste)
                    return BadRequest("Le GenreId fourni n'existe pas.");

                livreToUpdate.GenreId = livre.GenreId;
            }

            await _context.SaveChangesAsync();

            return Ok(livreToUpdate);
        }


        // DELETE: api/livres/5 (admin seulement)
        [Authorize(Policy = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLivre(int id)
        {
            var livre = await _context.Livres.FindAsync(id);
            if (livre == null) return NotFound();

            _context.Livres.Remove(livre);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LivreExists(int id)
        {
            return _context.Livres.Any(e => e.Id == id);
        }
    }
}
