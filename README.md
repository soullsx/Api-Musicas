# API de Músicas

## Sobre o projeto

Este projeto foi desenvolvido para a atividade de API utilizando ASP.NET Core e .NET 10.

A API tem como tema músicas e permite cadastrar, consultar, atualizar e excluir músicas.

Os dados ficam armazenados somente em memória, utilizando uma lista. Por isso, quando a aplicação é fechada, os dados cadastrados são perdidos.

## Tecnologias utilizadas

- C#
- ASP.NET Core
- .NET 10
- Bruno para testar as requisições

## Como executar

Abra o terminal na pasta do projeto e execute:

```bash
dotnet run --urls http://localhost:5050
```

## Endpoints

| Método | Rota | Descrição |
|---|---|---|
| GET | / | Verifica se a API está funcionando |
| GET | /api/musicas | Lista todas as músicas |
| GET | /api/musicas/{id} | Busca uma música pelo ID |
| POST | /api/musicas | Cadastra uma nova música |
| PUT | /api/musicas/{id} | Atualiza uma música |
| DELETE | /api/musicas/{id} | Remove uma música |

## Exemplo de POST

```json
{
  "titulo": "Master of Puppets",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Heavy Metal",
  "ano": 1986
}

{
  "titulo": "Master of Puppets",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Metal",
  "ano": 1986
}{
  "titulo": "Master of Puppets",
  "artista": "Metallica",
  "album": "Master of Puppets",
  "genero": "Metal",
  "ano": 1986
}


### 4. Testes

```markdown
## Testes

Os testes foram realizados utilizando o Bruno.

A collection está localizada na pasta:

bruno/api musicas

Foram testados os seguintes retornos:

- 200 OK
- 201 Created
- 204 No Content
- 404 Not Found

## Observação

Os dados da API são armazenados somente em memória, utilizando uma lista.

Por isso, quando a aplicação é encerrada, os dados cadastrados são perdidos.

## Vídeo

Link do vídeo da apresentação:

COLOCAR LINK AQUI