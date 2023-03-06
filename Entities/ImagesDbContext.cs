using ImagesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ImagesApi.Entities;

public class ImagesDbContext : DbContext
{
    public ImagesDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Image> Images {get; set;}
}