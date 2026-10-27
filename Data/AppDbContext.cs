using ApiMusicas.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMusicas.Data;

// DbContext: é a "ponte" entre o C# e o banco SQLite
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Cada DbSet vira uma tabela. Aqui a tabela "Musicas"
    public DbSet<Musica> Musicas => Set<Musica>();
}
