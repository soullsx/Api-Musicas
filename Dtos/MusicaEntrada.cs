namespace ApiMusicas.Dtos;

// DTO (objeto de transferência de dados): é o que chega no corpo do POST e do PUT.
// Não tem o Id, porque quem gera o Id é o banco. Fica separado do Model
// para a entrada da API não ser a mesma coisa que a tabela do banco.
public record MusicaEntrada(
    string Titulo,
    string Artista,
    string Album,
    string Genero,
    int Ano
);
