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
    [Bind("Id,juego,estado,runN,rejugando,DatosAdicionales,Calificacion,img,fecha_finalizado")]
    juegos newgame, [FromForm] IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (img != null && img.Length > 0)
            {
                using (var archivoSubido = img.OpenReadStream())
                {
                    string child = "FotosTest";
                    string urlArchivo = await SubirArchivo(archivoSubido, img.FileName, child);
                    newgame.img = urlArchivo;
                }
            }

            _context.juegos.Add(newgame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDataById), new { id = newgame.Id }, newgame);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDataById(int id)
        {
            var game = await _context.juegos.FindAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        // Método para subir archivo a Firebase Storage
        private async Task<string> SubirArchivo(Stream archivoSubir, string nombreArchivo, string child)
        {
            string email = "jorgefranciscocz@gmail.com";
            string clave = "ContraseñaXDXD";
            string ruta = "desarolloweb-7ffb8.appspot.com";
            string apikey = "AIzaSyBbIwF8pmsda6lLtldYsro7e_Aa_SCNGq0";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(apikey));
            var autentificar = await auth.SignInWithEmailAndPasswordAsync(email, clave);
            var cancellation = new CancellationTokenSource();
            var tokenuser = autentificar.FirebaseToken;

            var cargararchivo = new FirebaseStorage(ruta,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(tokenuser),
                    ThrowOnCancel = true
                }
            ).Child(child)
            .Child(nombreArchivo)
            .PutAsync(archivoSubir, cancellation.Token);

            var urlcarga = await cargararchivo;

            return urlcarga;
        }
    }
}
