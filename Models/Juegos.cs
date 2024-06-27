using System.ComponentModel.DataAnnotations;
namespace BackendAPI.Models
{
    public class juegos
    {
        [Key]
        public int Id { get; set; }
        public string? juego { get; set; }
        public string? estado { get; set; }
        public int? runN { get; set; }
        public string? rejugando { get; set; }
        public string? DatosAdicionales { get; set; }
        public decimal? Calificacion { get; set; }
        public string? img { get; set; }
        public DateTime? fecha_finalizado { get; set; }
    }
}
