using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

var musicas = new List<Musica>
{
    new Musica(1, "Sad But True", "Metallica", "Black Album", "Heavy Metal", 1991),
    new Musica(2, "Numb", "Linkin Park", "Meteora", "Nu Metal", 2003)
};
// Get Api
app.MapGet("/", () => "API de Músicas está no ar!");
// Get Listar musicas
app.MapGet("/api/musicas", () => musicas);
// Get buscar musica
app.MapGet("/api/musicas/{id}", (int id) =>
{
    var musica = musicas.FirstOrDefault(m => m.Id == id);

    if (musica == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(musica);
});

// Cadastrar

app.MapPost("/api/musicas", (MusicaEntrada entrada) =>
{
    int novoId = musicas.Count + 1;

    var novaMusica = new Musica(
        novoId,
        entrada.Titulo,
        entrada.Artista,
        entrada.Album,
        entrada.Genero,
        entrada.Ano
    );

    musicas.Add(novaMusica);

    return Results.Created($"/api/musicas/{novoId}", novaMusica);
});

// Atualizar

app.MapPut("/api/musicas/{id}", (int id, MusicaEntrada entrada) =>
{
    var indice = musicas.FindIndex(m => m.Id == id);

    if (indice == -1)
    {
        return Results.NotFound();
    }

    var musicaAtualizada = new Musica(
        id,
        entrada.Titulo,
        entrada.Artista,
        entrada.Album,
        entrada.Genero,
        entrada.Ano
    );

    musicas[indice] = musicaAtualizada;

    return Results.Ok(musicaAtualizada);
});

// Deletar

app.MapDelete("/api/musicas/{id}", (int id) =>
{
    var indice = musicas.FindIndex(m => m.Id == id);

    if (indice == -1)
    {
        return Results.NotFound();
    }

    musicas.RemoveAt(indice);

    return Results.NoContent();
});

app.Run();

record Musica(
    int Id,
    string Titulo,
    string Artista,
    string Album,
    string Genero,
    int Ano
);

record MusicaEntrada(
    string Titulo,
    string Artista,
    string Album,
    string Genero,
    int Ano
);
class MusicaEntity
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Artista { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Ano { get; set; }
}

class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options)
        : base(options)
    {
    }

    public DbSet<MusicaEntity> Musicas => Set<MusicaEntity>();
}