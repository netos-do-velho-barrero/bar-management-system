using System.Text.RegularExpressions;
using ControleDeBar.Testes.E2E.Compartilhado;
using ControleDeBar.Testes.E2E.Modulos.ModuloProduto;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloPedidoConta;

[TestClass]
public sealed class PedidoContaE2ETests : E2ETestsBase
{
    private ProdutoFormPage _produtoFormPage = null!;
    private ProdutoListarPage _produtoListarPage = null!;
    private ContaDetalhesPage _contaDetalhesPage = null!;
    private PedidoContaAdicionarPage _pedidoContaAdicionarPage = null!;

    [TestInitialize]
    public void SetupPages()
    {
        _produtoFormPage = new ProdutoFormPage(Page, UrlBase);
        _produtoListarPage = new ProdutoListarPage(Page, UrlBase);
        _contaDetalhesPage = new ContaDetalhesPage(Page, UrlBase);
        _pedidoContaAdicionarPage = new PedidoContaAdicionarPage(Page, UrlBase);
    }

    // Mesmo helper de autenticação usado em ProdutoE2ETests — duplicado aqui
    // porque cada classe de teste E2E monta seu próprio contexto de usuário.
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

    [TestMethod]
    public async Task CT_PED_017_DeveAtualizarValorTotal_AoAdicionar_Alterar_ERemoverPedidos()
    {
        // CT-PED-017: Atualizar o valor total da conta ao adicionar, alterar ou remover pedidos
        await EntrarComNovoUsuarioAsync();

        // Produto com preço conhecido para facilitar o cálculo esperado do total.
        await _produtoFormPage.IrParaCadastrarAsync();
        await _produtoFormPage.PreencherFormularioAsync("Cerveja Long Neck", "10.00");
        await _produtoFormPage.ConfirmarAsync();

        Guid contaId = await AbrirContaAsync();

        // --- Estado inicial: conta sem pedidos ---
        await _contaDetalhesPage.IrParaAsync(contaId);
        await Expect(_contaDetalhesPage.MensagemSemPedidos).ToBeVisibleAsync();

        // --- Adicionar: total deve refletir 2 x R$10,00 = R$20,00 ---
        await _pedidoContaAdicionarPage.IrParaAsync(contaId);
        await _pedidoContaAdicionarPage.SelecionarProdutoAsync("Cerveja Long Neck");
        await _pedidoContaAdicionarPage.PreencherQuantidadeAsync(2);
        await _pedidoContaAdicionarPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(_contaDetalhesPage.Url(contaId));
        await Expect(_contaDetalhesPage.ValorTotal).ToContainTextAsync(new Regex(@"20[.,]00"));

        // --- Alterar: quantidade 2 -> 5, total deve virar R$50,00 ---
        await _contaDetalhesPage.AlterarQuantidadeAsync("Cerveja Long Neck", 5);
        await Expect(_contaDetalhesPage.ValorTotal).ToContainTextAsync(new Regex(@"50[.,]00"));

        // --- Remover: total volta a zerar e a mensagem de "sem pedidos" reaparece ---
        await _contaDetalhesPage.RemoverAsync("Cerveja Long Neck");
        await Expect(_contaDetalhesPage.MensagemSemPedidos).ToBeVisibleAsync();
    }

    #region Helpers

    // Reproduz o mesmo fluxo usado em ProdutoE2ETests.CriarPedidoVinculadoAoProdutoAsync,
    // mas parando logo após abrir a conta (sem adicionar pedido), já que aqui
    // controlamos cada etapa (adicionar/alterar/remover) separadamente.
    private async Task<Guid> AbrirContaAsync()
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

        // 4. Localiza a conta recém-aberta e navega para Detalhes
        ILocator abaTodas = Page.Locator("#todas");
        ILocator linhaConta = abaTodas.Locator("tr", new LocatorLocatorOptions { HasText = "Cliente Teste E2E" });
        string? href = await linhaConta.Locator("a[title='Visualizar detalhes']").GetAttributeAsync("href");
        await Page.GotoAsync($"{UrlBase}{href}");

        // 5. Captura o ID da conta a partir da URL (/Conta/Detalhes/{id})
        Uri urlDetalhes = new(Page.Url);
        return Guid.Parse(urlDetalhes.Segments.Last());
    }

    #endregion
}
