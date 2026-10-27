namespace ApiMusicas.Models;

// Model: representa uma música e vira a tabela "Musicas" no banco
public class Musica
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Artista { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string Genero { get; set; } = string.Empty;
    public int Ano { get; set; }
}
