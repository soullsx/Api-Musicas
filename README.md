# API de Músicas

## Sobre o projeto

API de músicas feita em ASP.NET Core com .NET 10. Permite cadastrar, listar, buscar, atualizar e excluir músicas.

Na **AP1** os dados ficavam numa lista em memória e sumiam quando a API era fechada.

Na **AP2** a lista foi trocada por um banco **SQLite** usando **Entity Framework Core**. Agora os dados ficam gravados no arquivo `musicas.db` e continuam lá depois de reiniciar a API. O projeto também tem um **frontend** (HTML, CSS e JavaScript) servido pela própria API, que faz o CRUD chamando as rotas reais.

## Tecnologias utilizadas

- C# / ASP.NET Core (.NET 10) com Minimal API
- Entity Framework Core 10 + SQLite
- HTML, CSS e JavaScript (fetch)
- Bruno para testar as requisições

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Bruno](https://www.usebruno.com/) (para rodar a collection de testes)
- Opcional: ferramenta `dotnet-ef`, só se quiser criar o banco manualmente

## Estrutura do projeto

```
├── Data/
│   └── AppDbContext.cs          # DbContext (ligação com o banco)
├── Models/
│   ├── Musica.cs                # Model / entidade da tabela Musicas
│   └── MusicaEntrada.cs         # dados recebidos no POST e no PUT
├── Migrations/                  # migrations do EF Core (criam a tabela)
├── wwwroot/                     # frontend
│   ├── index.html
│   ├── css/estilo.css
│   └── js/app.js
├── bruno/api musicas/           # collection do Bruno
├── Program.cs                   # configuração e rotas da API
├── appsettings.json             # connection string do SQLite
└── ApiMusicas.csproj
```

## Como executar

```bash
git clone https://github.com/soullsx/Api-Musicas.git
cd Api-Musicas
dotnet run
```

A API sobe em `http://localhost:5050`.

### Preparação do banco

Não precisa fazer nada: ao iniciar, a API aplica as migrations automaticamente (`db.Database.Migrate()` no `Program.cs`) e cria o arquivo `musicas.db` com a tabela `Musicas`.

Se preferir criar o banco manualmente antes de rodar:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

O arquivo `musicas.db` não vai para o GitHub (está no `.gitignore`). Para começar com o banco vazio, basta apagar esse arquivo com a API parada.

## Interface (frontend)

Com a API rodando, abra no navegador:

```
http://localhost:5050
```

A tela é servida pela mesma origem da API (pasta `wwwroot`). Nela dá para cadastrar, listar, editar e excluir músicas. Acima do formulário aparece qual requisição foi feita e o status que a API respondeu.

## Rotas da API

| Método | Rota              | Descrição                          | Respostas  |
| ------ | ----------------- | ---------------------------------- | ---------- |
| GET    | /api              | Verifica se a API está no ar       | 200        |
| GET    | /api/musicas      | Lista todas as músicas             | 200        |
| GET    | /api/musicas/{id} | Busca uma música pelo ID           | 200 / 404  |
| POST   | /api/musicas      | Cadastra uma nova música           | 201        |
| PUT    | /api/musicas/{id} | Atualiza uma música                | 200 / 404  |
| DELETE | /api/musicas/{id} | Remove uma música                  | 204 / 404  |

> Na AP1 a rota de status era `GET /`. Ela mudou para `GET /api` porque agora o `/` abre o frontend.

### Exemplo de corpo para POST e PUT

```json
{
  "titulo": "Master of Puppets",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Heavy Metal",
  "ano": 1986
}
```

## Testes com o Bruno

A collection está na pasta `bruno/api musicas`. No Bruno, use **Open Collection** e selecione essa pasta.

Ordem dos testes:

1. `00 api no ar` – GET /api → 200
2. `01 cadastrar musica` – POST → 201
3. `02 listar musicas` – GET da lista → 200
4. `03 buscar musica por id` – GET por ID → 200
5. `04 atualizar musica` – PUT → 200
6. `05 excluir musica` – DELETE → 204
7. `06 buscar musica excluida (404)` – GET do ID excluído → 404

As requisições usam o ID `1`. Se o banco já tiver registros, troque o ID na URL pelo ID que voltou no POST.

## Vídeo de demonstração — AP2

   https://youtu.be/_y-xkROyy6o

## Vídeo de demonstração — AP1

https://www.youtube.com/watch?v=5ZbqNuw_UxQ
