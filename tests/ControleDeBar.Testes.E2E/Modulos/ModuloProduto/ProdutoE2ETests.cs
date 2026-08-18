using System.Text.RegularExpressions;
using Azure;
using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

[TestClass]
public sealed class ProdutoE2ETests : E2ETestsBase
{
    private ProdutoFormPage _formPage = null!;
    private ProdutoListarPage _listarPage = null!;
    private ProdutoExcluirPage _excluirPage = null!;

    [TestInitialize]
    public void SetupPages()
    {
        _formPage = new ProdutoFormPage(Page, UrlBase);
        _listarPage = new ProdutoListarPage(Page, UrlBase);
        _excluirPage = new ProdutoExcluirPage(Page);
    }

    // Registra e autentica um usuário antes de cada teste — sem isso, telas
    // como /Produto/Cadastrar redirecionam para o Login (multi-tenancy exige
    // usuário autenticado) e os campos do formulário nunca aparecem.
    //
    // Rota e campos conforme AutenticacaoController/EntrarViewModel reais:
    // GET/POST Autenticacao/Entrar, campos "Email" e "Senha".
    private async Task EntrarComNovoUsuarioAsync()
    {
        string email = $"e2e-{Guid.NewGuid():N}@teste.com";
        const string senha = "SenhaForte123!";

        await RegistrarUsuarioAsync(email, senha);

        await Page.GotoAsync($"{UrlBase}/Autenticacao/Entrar");
        await Page.Locator("input[name='Email']").FillAsync(email);
        await Page.Locator("input[name='Senha']").FillAsync(senha);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Entrar" }).ClickAsync();
    }

    #region Testes de Exclusão

    [TestMethod]
    public async Task CT_PRO_013_DeveExcluir_Produto_SemPedidosVinculados()
    {
        // CT-PRO-013: Excluir produto sem pedidos vinculados [POSITIVO]
        await EntrarComNovoUsuarioAsync();

        await CadastrarProdutoAsync("Refrigerante Lata", "6.00");

        await _listarPage.ExcluirAsync("Refrigerante Lata");
        await Expect(_excluirPage.MensagemConfirmacao).ToBeVisibleAsync();
        await _excluirPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(_listarPage.Url);
        await Expect(_listarPage.ObterLinhaPorNome("Refrigerante Lata")).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CT_PRO_014_DeveExibirErro_AoExcluir_ProdutoVinculadoAPedidosExistentes()
    {
        // CT-PRO-014: Excluir produto vinculado a pedidos existentes [NEGATIVO]
        //
        // ATENÇÃO: este teste depende dos módulos de Mesa, Garçom, Conta e
        // PedidoConta (implementados pelo Pedro/Marco em outra parte do time).
        // Os seletores em CriarPedidoVinculadoAoProdutoAsync usam os atributos
        // `name` dos <select>/<input> (MesaId, GarcomId, ProdutoId, Quantidade),
        // que são os nomes de propriedade dos ViewModels — ajuste aqui se as
        // views reais desses módulos tiverem ids/labels diferentes.

        await EntrarComNovoUsuarioAsync();

        await CadastrarProdutoAsync("Cerveja Long Neck", "12.50");

        Guid contaId = await CriarPedidoVinculadoAoProdutoAsync("Cerveja Long Neck");

        await _listarPage.IrParaAsync();
        await _listarPage.ExcluirAsync("Cerveja Long Neck");
        await Expect(_excluirPage.MensagemConfirmacao).ToBeVisibleAsync();
        await _excluirPage.ConfirmarAsync();

        // A exclusão deve falhar: o produto ainda aparece na listagem e uma
        // mensagem de erro é exibida (TempData["MensagemErro"] no Listar.cshtml).
        await Expect(Page).ToHaveURLAsync(_listarPage.Url);

        ILocator mensagemAlerta = Page.Locator(".alert-danger, .text-danger");
        await Expect(Page.GetByRole(AriaRole.Alert).First).ToBeVisibleAsync();

        await Expect(_listarPage.ObterLinhaPorNome("Cerveja Long Neck")).ToBeVisibleAsync();
    }

    #endregion

    #region Helpers

    private async Task CadastrarProdutoAsync(string nome, string precoVenda)
    {
        await _formPage.IrParaCadastrarAsync();
        await _formPage.PreencherFormularioAsync(nome, precoVenda);
        await _formPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(_listarPage.Url);
    }

    // Cria Mesa -> Garçom -> Conta aberta -> Pedido com o produto informado,
    // navegando diretamente pelas telas (sem page objects próprios, já que
    // esses módulos não são deste arquivo). Retorna o Id da conta criada,
    // caso precise ser usado por outros testes.
  private async Task<Guid> CriarPedidoVinculadoAoProdutoAsync(string nomeProduto)
{
    // 1. Mesa
    await Page.GotoAsync($"{UrlBase}/Mesa/Cadastrar");
    await Page.Locator("input[name=Numero]").FillAsync("99");
    await Page.Locator("input[name=QuantidadeLugares]").FillAsync("4");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Cadastrar" }).ClickAsync();

    // 2. Garçom
    await Page.GotoAsync($"{UrlBase}/Garcom/Cadastrar");
    await Page.Locator("input[name=Nome]").FillAsync("Garçom Teste E2E");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Cadastrar" }).ClickAsync();

    // 3. Abrir conta
    await Page.GotoAsync($"{UrlBase}/Conta/Abrir");
    await Page.GetByLabel("Mesa").SelectOptionAsync(new SelectOptionValue { Label = "Mesa 99" });
    await Page.GetByLabel("Garçom").SelectOptionAsync(new SelectOptionValue { Label = "Garçom Teste E2E" });
    await Page.GetByLabel("Nome do cliente").FillAsync("Cliente Teste E2E");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Abrir Conta" }).ClickAsync();

    // 4. Localiza a conta e navega para Detalhes
    ILocator abaTodas = Page.Locator("#todas");
    ILocator linhaConta = abaTodas.Locator("tr", new LocatorLocatorOptions { HasText = "Cliente Teste E2E" });
    string? href = await linhaConta.Locator("a[title='Visualizar detalhes']").GetAttributeAsync("href");
    await Page.GotoAsync($"{UrlBase}{href}");

    // 5. Captura o ID da conta a partir da URL (/Conta/Detalhes/{id})
    Uri urlDetalhes = new(Page.Url);
    Guid contaId = Guid.Parse(urlDetalhes.Segments.Last());

    // 6. Navega para a tela de adicionar pedido e seleciona o produto pelo value
    await Page.GotoAsync($"{UrlBase}/PedidoConta/Adicionar?contaId={contaId}");

    string? produtoId = await Page.Locator("select[name=ProdutoId] option", new() { HasText = nomeProduto })
                                  .GetAttributeAsync("value");

    await Page.Locator("select[name=ProdutoId]").SelectOptionAsync(produtoId);
    await Page.Locator("input[name=Quantidade]").FillAsync("1");
    await Page.GetByRole(AriaRole.Button, new() { Name = "Adicionar" }).ClickAsync();

    return contaId;
}

    #endregion
}
