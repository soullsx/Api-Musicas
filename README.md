# API de Músicas

## Tema e objetivo

API REST de **músicas** feita em ASP.NET Core (.NET 10). Ela permite cadastrar, listar, buscar por id, atualizar e excluir músicas (CRUD), guardando tudo num banco **SQLite** com **Entity Framework Core**.

O projeto é o mesmo desde a AP1:

- **AP1:** API com os dados numa lista em memória (sumiam ao fechar a API).
- **AP2:** a lista foi trocada pelo banco SQLite com EF Core, e entrou uma interface (HTML, CSS e JavaScript) servida pela própria API.
- **AS:** o código foi organizado em pastas por responsabilidade. A entidade fica em `Models`, o tipo de entrada (DTO) em `Dtos` e o contexto do banco em `Data`, e o `Program.cs` ficou só com a inicialização, o middleware e as rotas. A collection do Bruno ganhou um ambiente local e passou a usar o id devolvido pelo POST.

## Tecnologias

- C# / ASP.NET Core (.NET 10) com Minimal API
- Entity Framework Core 10 + SQLite
- HTML, CSS e JavaScript (fetch) na interface
- Bruno para testar as requisições

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Bruno](https://www.usebruno.com/) para rodar a collection de testes
- Opcional: ferramenta `dotnet-ef`, só se quiser criar o banco manualmente

## Como preparar e executar

```bash
git clone https://github.com/soullsx/Api-Musicas.git
cd Api-Musicas
dotnet restore
dotnet build
dotnet run
```

A API sobe em `http://localhost:5050`. Para parar, use `Ctrl + C` no terminal.

### Banco de dados: migrations

O projeto usa **migrations** do Entity Framework Core (pasta `Migrations/`). Não precisa rodar nenhum comando para criar o banco: ao iniciar, o `Program.cs` chama `db.Database.Migrate()`, que aplica as migrations e cria o arquivo `musicas.db` com a tabela `Musicas` se ele ainda não existir.

Se preferir criar o banco manualmente antes de rodar:

```bash
dotnet tool install --global dotnet-ef
dotnet ef database update
```

A connection string fica no `appsettings.json` (`Data Source=musicas.db`). O arquivo `musicas.db` não vai para o GitHub (está no `.gitignore`). Para começar com o banco vazio, apague esse arquivo com a API parada.

## Estrutura de pastas

```
Api-Musicas/
├── Models/
│   └── Musica.cs                  # entidade: formato da música salva na tabela Musicas
├── Dtos/
│   └── MusicaEntrada.cs           # tipo de entrada (record) recebido no POST e no PUT, sem Id
├── Data/
│   └── AppDbContext.cs            # contexto do EF Core: liga o C# ao banco SQLite
├── Migrations/                    # migration que cria a tabela Musicas
├── wwwroot/                       # interface da AP2 (HTML, CSS e JS)
│   ├── index.html
│   ├── css/estilo.css
│   └── js/app.js
├── bruno/api musicas/             # collection do Bruno
│   ├── opencollection.yml         # configuração da collection
│   ├── environments/local.yml     # ambiente "local" (baseUrl = http://localhost:5050)
│   └── 00 … 09 *.yml              # requisições do roteiro de testes
├── Properties/launchSettings.json # porta da aplicação (5050)
├── Program.cs                     # inicialização, registro do contexto, middleware e rotas
├── appsettings.json               # connection string do SQLite
└── ApiMusicas.csproj              # arquivo do projeto e pacotes do EF Core
```

### Por que separar e como o Program.cs encontra os tipos

Cada tipo fica num arquivo próprio, na pasta da sua responsabilidade, com o namespace igual ao nome da pasta:

| Arquivo                 | Namespace           | Responsabilidade                                              |
| ----------------------- | ------------------- | ------------------------------------------------------------- |
| `Models/Musica.cs`      | `ApiMusicas.Models` | Entidade que vira a tabela `Musicas`                          |
| `Dtos/MusicaEntrada.cs` | `ApiMusicas.Dtos`   | Dados que o cliente envia (sem `Id`, que é gerado pelo banco) |
| `Data/AppDbContext.cs`  | `ApiMusicas.Data`   | Ponte entre o código e o banco (`DbSet<Musica> Musicas`)      |

O `Program.cs` não tem mais a definição desses tipos. Ele só importa os namespaces no topo:

```csharp
using ApiMusicas.Data;
using ApiMusicas.Dtos;
using ApiMusicas.Models;
```

O `AppDbContext` também importa `ApiMusicas.Models` para conhecer a entidade `Musica`. O `Program.cs` está dividido em três partes comentadas:

1. **Inicialização e registro dos serviços:** cria o builder, registra o `AppDbContext` com SQLite e o formato de erro (`AddProblemDetails`) e aplica as migrations.
2. **Middleware:** tratamento de exceções e os arquivos da interface (`UseDefaultFiles` e `UseStaticFiles`).
3. **Rotas (endpoints):** o CRUD de `/api/musicas`.

### Tratamento de erros

- Id que não existe → `404 Not Found` (GET, PUT e DELETE por id).
- JSON inválido no corpo → `400 Bad Request` (tratado pelo próprio ASP.NET Core).
- Erro inesperado fora do ambiente de desenvolvimento → `UseExceptionHandler` responde `500` num formato padrão (ProblemDetails), sem mostrar detalhes internos. Em desenvolvimento, o .NET mostra a página de erro detalhada.
- Na interface, cada chamada à API fica dentro de `try/catch` e mostra um aviso se a API estiver fora do ar.

## Rotas da API

| Método | Rota                | Descrição                    | Respostas       |
| ------ | ------------------- | ---------------------------- | --------------- |
| GET    | `/api`              | Verifica se a API está no ar | 200             |
| GET    | `/api/musicas`      | Lista todas as músicas       | 200             |
| GET    | `/api/musicas/{id}` | Busca uma música pelo id     | 200 / 404       |
| POST   | `/api/musicas`      | Cadastra uma música          | 201 / 400       |
| PUT    | `/api/musicas/{id}` | Atualiza uma música          | 200 / 400 / 404 |
| DELETE | `/api/musicas/{id}` | Exclui uma música            | 204 / 404       |

> O `/` abre a interface (pasta `wwwroot`). Por isso a rota de status é `GET /api`.

## Exemplos de JSON

**Corpo do POST** (`/api/musicas`):

```json
{
  "titulo": "Master of Puppets",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Heavy Metal",
  "ano": 1986
}
```

**Resposta do POST** (`201 Created`, o `id` é gerado pelo banco):

```json
{
  "id": 1,
  "titulo": "Master of Puppets",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Heavy Metal",
  "ano": 1986
}
```

**Corpo do PUT** (`/api/musicas/{id}`):

```json
{
  "titulo": "Master of Puppets (Remastered)",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Thrash Metal",
  "ano": 1986
}
```

## Testes com o Bruno

A collection está em **`bruno/api musicas/`**.

1. No Bruno, use **Open Collection** e selecione a pasta `bruno/api musicas`.
2. No canto superior direito, selecione o ambiente **local** (`baseUrl = http://localhost:5050`).
3. Com a API rodando, execute as requisições na ordem:

| Requisição                              | Método e rota                         | Esperado                           |
| --------------------------------------- | ------------------------------------- | ---------------------------------- |
| `00 api no ar`                          | GET `/api`                            | 200                                |
| `01 listar musicas`                     | GET `/api/musicas`                    | 200 e a lista                      |
| `02 cadastrar musica`                   | POST `/api/musicas`                   | 201 e o registro com id            |
| `03 buscar musica criada`               | GET `/api/musicas/{{idCriado}}`       | 200 e os mesmos dados              |
| `04 atualizar musica`                   | PUT `/api/musicas/{{idCriado}}`       | 200 e os dados atualizados         |
| `05 conferir atualizacao`               | GET `/api/musicas/{{idCriado}}`       | 200 com os dados novos             |
| `06 excluir musica`                     | DELETE `/api/musicas/{{idCriado}}`    | 204, sem corpo                     |
| `07 buscar musica excluida (404)`       | GET `/api/musicas/{{idCriado}}`       | 404                                |
| `08 cadastrar musica para persistencia` | POST `/api/musicas`                   | 201 (esse registro não é excluído) |
| `09 buscar depois de reiniciar a API`   | GET `/api/musicas/{{idPersistencia}}` | 200 depois de reiniciar            |

As requisições não usam id fixo. O `02` e o `08` têm um script que guarda o `id` devolvido pelo POST nas variáveis `idCriado` e `idPersistencia`, e as requisições seguintes usam essas variáveis. Cada requisição tem uma asserção do status esperado.

**Teste de persistência:** rode o `08`, pare a API (`Ctrl + C`), inicie de novo com `dotnet run` e rode o `09`. A música continua lá porque está gravada no `musicas.db`. Se o Bruno for fechado no meio, a variável se perde: troque `{{idPersistencia}}` pelo id que voltou no `08`.

## Interface (frontend)

Com a API rodando, abra `http://localhost:5050` no navegador. A tela é servida pela pasta `wwwroot` e faz o CRUD chamando as mesmas rotas. Acima do formulário aparece a requisição feita e o status devolvido.

## Vídeo de demonstração — AS

https://youtu.be/c0oUo6ElPyg

## Vídeo de demonstração — AP2

https://youtu.be/_y-xkROyy6o

## Vídeo de demonstração — AP1

https://www.youtube.com/watch?v=5ZbqNuw_UxQ
