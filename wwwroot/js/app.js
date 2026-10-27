// Endereço da API. Como a tela é servida pela própria API (mesma origem),
// não precisa colocar http://localhost:5050 na frente
const API_URL = "/api/musicas";

// Elementos da tela
const formMusica = document.getElementById("formMusica");
const campoId = document.getElementById("musicaId");
const campoTitulo = document.getElementById("titulo");
const campoArtista = document.getElementById("artista");
const campoAlbum = document.getElementById("album");
const campoGenero = document.getElementById("genero");
const campoAno = document.getElementById("ano");
const tituloFormulario = document.getElementById("tituloFormulario");
const btnSalvar = document.getElementById("btnSalvar");
const btnCancelar = document.getElementById("btnCancelar");
const btnRecarregar = document.getElementById("btnRecarregar");
const listaMusicas = document.getElementById("listaMusicas");
const listaVazia = document.getElementById("listaVazia");
const caixaStatus = document.getElementById("status");

const nomesStatus = {
    200: "OK",
    201: "Created",
    204: "No Content",
    400: "Bad Request",
    404: "Not Found",
    500: "Internal Server Error"
};

// Mostra na tela qual requisição foi feita e qual status a API devolveu
function mostrarStatus(metodo, url, status) {
    const nome = nomesStatus[status] || "";
    caixaStatus.textContent = metodo + " " + url + " → " + status + " " + nome;

    if (status >= 200 && status < 300) {
        caixaStatus.className = "status sucesso";
    } else {
        caixaStatus.className = "status erro";
    }
}

function mostrarErro(mensagem) {
    caixaStatus.textContent = mensagem;
    caixaStatus.className = "status erro";
}

// LISTAR (GET /api/musicas)
async function carregarMusicas() {
    try {
        const resposta = await fetch(API_URL);
        mostrarStatus("GET", API_URL, resposta.status);

        const musicas = await resposta.json();
        desenharTabela(musicas);
    } catch (erro) {
        mostrarErro("Não foi possível falar com a API. Ela está rodando?");
    }
}

// Monta as linhas da tabela com as músicas que vieram do banco
function desenharTabela(musicas) {
    listaMusicas.innerHTML = "";

    if (musicas.length === 0) {
        listaVazia.classList.remove("escondido");
        return;
    }

    listaVazia.classList.add("escondido");

    musicas.forEach(function (musica) {
        const linha = document.createElement("tr");

        const valores = [musica.id, musica.titulo, musica.artista, musica.album, musica.genero, musica.ano];
        valores.forEach(function (valor) {
            const celula = document.createElement("td");
            celula.textContent = valor;
            linha.appendChild(celula);
        });

        const celulaAcoes = document.createElement("td");
        celulaAcoes.className = "acoes";

        const btnEditar = document.createElement("button");
        btnEditar.textContent = "Editar";
        btnEditar.className = "btn btn-secundario btn-pequeno";
        btnEditar.addEventListener("click", function () {
            editarMusica(musica.id);
        });

        const btnExcluir = document.createElement("button");
        btnExcluir.textContent = "Excluir";
        btnExcluir.className = "btn btn-excluir btn-pequeno";
        btnExcluir.addEventListener("click", function () {
            excluirMusica(musica.id, musica.titulo);
        });

        celulaAcoes.appendChild(btnEditar);
        celulaAcoes.appendChild(btnExcluir);
        linha.appendChild(celulaAcoes);

        listaMusicas.appendChild(linha);
    });
}

// Pega os dados digitados no formulário
function lerFormulario() {
    return {
        titulo: campoTitulo.value,
        artista: campoArtista.value,
        album: campoAlbum.value,
        genero: campoGenero.value,
        ano: Number(campoAno.value)
    };
}

// CADASTRAR (POST) ou ATUALIZAR (PUT), dependendo se tem id no formulário
formMusica.addEventListener("submit", async function (evento) {
    evento.preventDefault(); // não deixa a página recarregar

    const dados = lerFormulario();
    const id = campoId.value;

    let metodo = "POST";
    let url = API_URL;

    if (id) {
        metodo = "PUT";
        url = API_URL + "/" + id;
    }

    try {
        const resposta = await fetch(url, {
            method: metodo,
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(dados)
        });

        mostrarStatus(metodo, url, resposta.status);

        if (resposta.ok) {
            limparFormulario();
            await carregarListaSemTrocarStatus();
        }
    } catch (erro) {
        mostrarErro("Não foi possível falar com a API. Ela está rodando?");
    }
});

// EDITAR: busca a música pelo id (GET /api/musicas/{id}) e preenche o formulário
async function editarMusica(id) {
    const url = API_URL + "/" + id;

    try {
        const resposta = await fetch(url);
        mostrarStatus("GET", url, resposta.status);

        if (!resposta.ok) {
            return;
        }

        const musica = await resposta.json();

        campoId.value = musica.id;
        campoTitulo.value = musica.titulo;
        campoArtista.value = musica.artista;
        campoAlbum.value = musica.album;
        campoGenero.value = musica.genero;
        campoAno.value = musica.ano;

        tituloFormulario.textContent = "Editando música #" + musica.id;
        btnSalvar.textContent = "Salvar alterações";
        btnCancelar.classList.remove("escondido");
        campoTitulo.focus();
    } catch (erro) {
        mostrarErro("Não foi possível falar com a API. Ela está rodando?");
    }
}

// EXCLUIR (DELETE /api/musicas/{id})
async function excluirMusica(id, titulo) {
    const confirmou = confirm("Excluir a música \"" + titulo + "\"?");
    if (!confirmou) {
        return;
    }

    const url = API_URL + "/" + id;

    try {
        const resposta = await fetch(url, { method: "DELETE" });
        mostrarStatus("DELETE", url, resposta.status);

        // se estava editando essa mesma música, limpa o formulário
        if (campoId.value == id) {
            limparFormulario();
        }

        await carregarListaSemTrocarStatus();
    } catch (erro) {
        mostrarErro("Não foi possível falar com a API. Ela está rodando?");
    }
}

// Recarrega a lista mas mantém na tela o status da última ação (POST, PUT ou DELETE)
async function carregarListaSemTrocarStatus() {
    const resposta = await fetch(API_URL);
    const musicas = await resposta.json();
    desenharTabela(musicas);
}

function limparFormulario() {
    formMusica.reset();
    campoId.value = "";
    tituloFormulario.textContent = "Cadastrar música";
    btnSalvar.textContent = "Cadastrar";
    btnCancelar.classList.add("escondido");
}

btnCancelar.addEventListener("click", limparFormulario);
btnRecarregar.addEventListener("click", carregarMusicas);

// Quando a página abre, já busca as músicas do banco
carregarMusicas();
