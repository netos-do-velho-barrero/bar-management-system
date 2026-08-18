using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;

[TestClass]
public sealed class GarcomE2ETests : E2ETestsBase
{
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
    public async Task CT_GAR_001_DeveCadastrarGarcomComSucesso()
    {
        // CT-GAR-001: Cadastrar garçom com nome válido
        await EntrarComNovoUsuarioAsync();

        await Page.GotoAsync($"{UrlBase}/Garcom/Cadastrar");
        await Page.Locator("input[name='Nome']").FillAsync("João da Silva");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cadastrar" }).ClickAsync();

        await Expect(Page).ToHaveURLAsync($"{UrlBase}/Garcom/Listar");
        await Expect(Page.GetByText("João da Silva")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CT_GAR_002_DeveExibirErro_AoCadastrarGarcomSemNome()
    {
        // CT-GAR-002: Cadastrar garçom sem informar o nome
        await EntrarComNovoUsuarioAsync();

        await Page.GotoAsync($"{UrlBase}/Garcom/Cadastrar");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Cadastrar" }).ClickAsync();

        ILocator erro = Page.Locator(".field-validation-error, .text-danger");
        await Expect(erro.First).ToBeVisibleAsync();
    }
}
