using ControleDeBar.Testes.E2E.Compartilhado;
using ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;
using ControleDeBar.Testes.E2E.Modulos.ModuloMesa;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

[TestClass]
public sealed class ContaE2ETests : E2ETestsBase
{
    private MesaFormPage _mesaFormPage = null!;
    private GarcomFormPage _garcomFormPage = null!;
    private ContaAbrirPage _contaAbrirPage = null!;
    private ContaListarPage _contaListarPage = null!;

    [TestInitialize]
    public void SetupPages()
    {
        _mesaFormPage = new MesaFormPage(Page, UrlBase);
        _garcomFormPage = new GarcomFormPage(Page, UrlBase);
        _contaAbrirPage = new ContaAbrirPage(Page, UrlBase);
        _contaListarPage = new ContaListarPage(Page, UrlBase);
    }

    private async Task EntrarComNovoUsuarioAsync()
    {
        string email =
            $"e2e-{Guid.NewGuid():N}@teste.com";

        const string senha = "SenhaForte123!";

        await RegistrarUsuarioAsync(email, senha);

        await Page.GotoAsync(
            $"{UrlBase}/Autenticacao/Entrar"
        );

        await Page
            .Locator("input[name='Email']")
            .FillAsync(email);

        await Page
            .Locator("input[name='Senha']")
            .FillAsync(senha);

        await Page
            .GetByRole(
                AriaRole.Button,
                new() { Name = "Entrar" }
            )
            .ClickAsync();
    }

    [TestMethod]
    public async Task CT_CTA_001_DeveAbrirEFecharContaComSucesso()
    {
        await EntrarComNovoUsuarioAsync();

        await _mesaFormPage.IrParaCadastrarAsync();
        await _mesaFormPage.PreencherAsync(15, 4);
        await _mesaFormPage.ConfirmarAsync();

        await _garcomFormPage.IrParaCadastrarAsync();
        await _garcomFormPage.PreencherAsync("Garçom E2E");
        await _garcomFormPage.ConfirmarAsync();

        await _contaAbrirPage.IrParaAsync();

        await _contaAbrirPage.PreencherAsync(
            "Mesa 15",
            "Garçom E2E",
            "Cliente E2E Teste"
        );

        await _contaAbrirPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(
            _contaListarPage.Url
        );

        await Expect(
            _contaListarPage
                .ObterLinhaPorCliente("Cliente E2E Teste")
        ).ToBeVisibleAsync();

        string? hrefFechar =
            await _contaListarPage
                .ObterLinhaPorCliente("Cliente E2E Teste")
                .Locator("a[title='Fechar conta']")
                .GetAttributeAsync("href");

        Assert.IsNotNull(hrefFechar);

        await Page.GotoAsync(
            $"{UrlBase}{hrefFechar}"
        );

        await Page
            .GetByRole(
                AriaRole.Button,
                new() { Name = "Confirmar Fechamento" }
            )
            .ClickAsync();

        await Expect(Page).ToHaveURLAsync(
            $"{UrlBase}/Conta/Listar"
        );

        await Expect(
            _contaListarPage
                .ObterLinhaPorCliente("Cliente E2E Teste")
        ).ToBeVisibleAsync();
    }
}
