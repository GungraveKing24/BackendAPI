using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;
using Firebase.Storage;
using Firebase.Auth;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GamesContext _context;

        public GamesController(GamesContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetData()
        {
            try
            {
                var games = _context.juegos.ToList(); // Asegúrate de que 'Games' es el nombre correcto del DbSet
                return Ok(games);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostData(
            [Bind("juego,estado,runN,rejugando,DatosAdicionales,Calificacion")] juegos newgame)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.juegos.Add(newgame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetData), new { id = newgame.Id }, newgame);
        }
    }
}
