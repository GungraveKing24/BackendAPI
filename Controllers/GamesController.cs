using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using BackendAPI.Models;

namespace BackendAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : Controller
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
    }
}