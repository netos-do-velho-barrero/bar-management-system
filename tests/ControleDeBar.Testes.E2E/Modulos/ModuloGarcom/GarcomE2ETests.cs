using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;

[TestClass]
public sealed class GarcomE2ETests : E2ETestsBase
{
    private GarcomFormPage _garcomFormPage = null!;

    [TestInitialize]
    public void SetupPages()
    {
        _garcomFormPage = new GarcomFormPage(Page, UrlBase);
    }

    private async Task EntrarComNovoUsuarioAsync()
    {
        string email = $"e2e-{Guid.NewGuid():N}@teste.com";
        const string senha = "SenhaForte123!";

        await RegistrarUsuarioAsync(email, senha);

        await Page.GotoAsync($"{UrlBase}/Autenticacao/Entrar");

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
    public async Task CT_GAR_001_DeveCadastrarGarcomComSucesso()
    {
        await EntrarComNovoUsuarioAsync();

        await _garcomFormPage.IrParaCadastrarAsync();
        await _garcomFormPage.PreencherAsync("João da Silva");
        await _garcomFormPage.ConfirmarAsync();

        await Expect(Page).ToHaveURLAsync(
            $"{UrlBase}/Garcom/Listar"
        );

        await Expect(
            Page.GetByText("João da Silva")
        ).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CT_GAR_002_DeveExibirErro_AoCadastrarGarcomSemNome()
    {
        await EntrarComNovoUsuarioAsync();

        await _garcomFormPage.IrParaCadastrarAsync();
        await _garcomFormPage.ConfirmarAsync();

        await Expect(
            Page.GetByText("O campo \"Nome\" deve ser preenchido.")
        ).ToBeVisibleAsync();
    }
}
