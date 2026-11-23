using ApiMusicas.Data;   // AppDbContext (pasta Data)
using ApiMusicas.Dtos;   // MusicaEntrada (pasta Dtos)
using ApiMusicas.Models; // Musica (pasta Models)
using Microsoft.EntityFrameworkCore;

// ===== 1. Inicialização e registro dos serviços =====

var builder = WebApplication.CreateBuilder(args);

// Registra o DbContext usando SQLite (a connection string fica no appsettings.json)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Formato padrão de resposta de erro (usado pelo tratamento de exceções abaixo)
builder.Services.AddProblemDetails();

var app = builder.Build();

// Aplica as migrations ao iniciar: se o arquivo musicas.db não existir, ele é criado com a tabela
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ===== 2. Middleware =====

// Tratamento de exceções: fora do ambiente de desenvolvimento, um erro inesperado
// vira uma resposta 500 padronizada, sem mostrar detalhes internos do código.
// Em desenvolvimento o próprio .NET mostra a página de erro detalhada.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
}

// Serve o frontend (pasta wwwroot) na mesma origem da API
app.UseDefaultFiles();
app.UseStaticFiles();

// ===== 3. Rotas (endpoints) =====

// Get Api
app.MapGet("/api", () => "API de Músicas está no ar!");

// Get Listar musicas
app.MapGet("/api/musicas", async (AppDbContext db) =>
{
    var musicas = await db.Musicas.ToListAsync();

    return Results.Ok(musicas);
});

// Get buscar musica
app.MapGet("/api/musicas/{id}", async (int id, AppDbContext db) =>
{
    var musica = await db.Musicas.FindAsync(id);

    if (musica == null)
    {
        return Results.NotFound();
    }

    return Results.Ok(musica);
});

// Cadastrar

app.MapPost("/api/musicas", async (MusicaEntrada entrada, AppDbContext db) =>
{
    var novaMusica = new Musica
    {
        Titulo = entrada.Titulo,
        Artista = entrada.Artista,
        Album = entrada.Album,
        Genero = entrada.Genero,
        Ano = entrada.Ano
    };

    db.Musicas.Add(novaMusica);
    await db.SaveChangesAsync(); // aqui grava no banco e o Id é gerado

    return Results.Created($"/api/musicas/{novaMusica.Id}", novaMusica);
});

// Atualizar

app.MapPut("/api/musicas/{id}", async (int id, MusicaEntrada entrada, AppDbContext db) =>
{
    var musica = await db.Musicas.FindAsync(id);

    if (musica == null)
    {
        return Results.NotFound();
    }

    musica.Titulo = entrada.Titulo;
    musica.Artista = entrada.Artista;
    musica.Album = entrada.Album;
    musica.Genero = entrada.Genero;
    musica.Ano = entrada.Ano;

    await db.SaveChangesAsync();

    return Results.Ok(musica);
});

// Deletar

app.MapDelete("/api/musicas/{id}", async (int id, AppDbContext db) =>
{
    var musica = await db.Musicas.FindAsync(id);

    if (musica == null)
    {
        return Results.NotFound();
    }

    db.Musicas.Remove(musica);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();
