namespace ApiMusicas.Models;

// Dados que chegam no POST e no PUT (sem o Id, porque quem gera o Id é o banco)
public record MusicaEntrada(
    string Titulo,
    string Artista,
    string Album,
    string Genero,
    int Ano
);
