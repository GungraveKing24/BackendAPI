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
                var games = _context.juegos.ToList();
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
            [FromForm] string juego,
            [FromForm] string estado,
            [FromForm] int? runN,
            [FromForm] string rejugando,
            [FromForm] string DatosAdicionales,
            [FromForm] decimal? Calificacion,
            [FromForm] DateTime? fecha_finalizado,
            [FromForm] IFormFile img)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newgame = new juegos
            {
                juego = juego,
                estado = estado,
                runN = runN,
                rejugando = rejugando,
                DatosAdicionales = DatosAdicionales,
                Calificacion = Calificacion,
                fecha_finalizado = fecha_finalizado
            };

            if (img != null && img.Length > 0)
            {
                try
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                    var fileUrl = await SubirArchivo(img.OpenReadStream(), fileName, "images");
                    newgame.img = fileUrl;
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Error uploading image: " + ex.Message);
                }
            }

            _context.juegos.Add(newgame);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetData), new { id = newgame.Id }, newgame);
        }

        private async Task<string> SubirArchivo(Stream archivoSubir, string nombreArchivo, string child)
        {
            string email = "jorgefranciscocz@gmail.com";
            string clave = "Contrasenaxd54";
            string ruta = "composite-sun-374501.appspot.com";
            string apikey = "AIzaSyDxmS6QxnaFnnhybus3Z8KNjeQuVXSeaWU";

            var auth = new FirebaseAuthProvider(new FirebaseConfig(apikey));
            var autentificar = await auth.SignInWithEmailAndPasswordAsync(email, clave);
            var tokenuser = autentificar.FirebaseToken;

            var firebaseStorage = new FirebaseStorage(ruta, new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(tokenuser),
                ThrowOnCancel = true
            });

            var uploadTask = firebaseStorage.Child(child).Child(nombreArchivo).PutAsync(archivoSubir);
            var urlcarga = await uploadTask;

            return urlcarga;
        }
    }
}
