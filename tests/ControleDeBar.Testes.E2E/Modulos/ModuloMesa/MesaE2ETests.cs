using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

[TestClass]
public sealed class MesaE2ETests : E2ETestsBase
{
    private MesaFormPage _mesaFormPage = null!;
    private MesaListarPage _mesaListarPage = null!;

    [TestInitialize]
    public void SetupPages()
    {
        _mesaFormPage = new MesaFormPage(Page, UrlBase);
        _mesaListarPage = new MesaListarPage(Page, UrlBase);
    }

    private async Task EntrarComNovoUsuarioAsync()
    {
        string email = $"e2e-{Guid.NewGuid():N}@teste.com";
        const string senha = "SenhaForte123!";

        await RegistrarUsuarioAsync(email, senha);

        await Page.GotoAsync($"{UrlBase}/Autenticacao/Entrar");
        await Page.Locator("input[name='Email']").FillAsync(email);
        await Page.Locator("input[name='Senha']").FillAsync(senha);
        await Page.GetByRole(
            AriaRole.Button,
            new() { Name = "Entrar" }
        ).ClickAsync();
    }

    [TestMethod]
    public async Task CT_MES_001_DeveCadastrarMesaComSucesso()
    {
        await EntrarComNovoUsuarioAsync();

        await _mesaFormPage.IrParaCadastrarAsync();
        await _mesaFormPage.PreencherAsync(10, 4);
        await _mesaFormPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(_mesaListarPage.Url);
        await Expect(
            _mesaListarPage.ObterLinhaPorNumero(10)
        ).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CT_MES_002_DeveExibirErro_AoCadastrarMesaSemNumero()
    {
        await EntrarComNovoUsuarioAsync();

        await _mesaFormPage.IrParaCadastrarAsync();
        await _mesaFormPage.PreencherAsync(0, 4);
        await _mesaFormPage.ConfirmarAsync();

        await Expect(
            Page.GetByText("The value '' is invalid.")
        ).ToBeVisibleAsync();
    }
}
