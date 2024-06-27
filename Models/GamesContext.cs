using Microsoft.EntityFrameworkCore;
using BackendAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Models
{
    public class GamesContext : DbContext
    {
        public GamesContext(DbContextOptions<GamesContext> options) : base(options)
        {

        }
        public DbSet<juegos> juegos { get; set; }
    }
}
