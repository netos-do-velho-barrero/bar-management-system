using System.Text.RegularExpressions;
using GeradorDeProvas.Testes.E2E.Compartilhado;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace GeradorDeProvas.Testes.E2E.Modulos;

[TestClass]
public sealed class AutenticacaoE2ETests : PageTest
{

    private TestApplicationFactory aplicacao = null!;
    private string UrlBase { get; set; } = string.Empty;

    [TestInitialize]
    public async Task InicializarAplicacao()
    {
        aplicacao = new TestApplicationFactory();

        UrlBase = aplicacao.UrlBase;
    }

    [TestCleanup]
    public void LiberarAplicacao()
    {
        try
        {
            if (aplicacao != null)
                aplicacao.Dispose();
        }
        finally
        {
            aplicacao = null!;
        }
    }

    [TestMethod]
    public async Task Deve_Exibir_TelaDeLogin_ParaUsuarioAnonimo()
    {
        // Arrange
        // Act
        await Page.GotoAsync($"{UrlBase}/");

        // Assert
        await Expect(Page).ToHaveTitleAsync(new Regex("Entrar"));
    }

    [TestMethod]
    public async Task Deve_RegistrarEAutenticar_Usuario()
    {
        // Arrange
        const string email = "novo.usuario@teste.local";
        const string senha = "Senha123!";

        await Page.GotoAsync($"{UrlBase}/Autenticacao/Registrar");

        // Act
        await Page.GetByLabel("E-mail").FillAsync(email);
        await Page.GetByLabel("Senha", new() { Exact = true }).FillAsync(senha);
        await Page.GetByLabel("Confirmar Senha").FillAsync(senha);

        await Page.GetByRole(AriaRole.Button, new() { Name = "Criar Conta" }).ClickAsync();

        // Assert
        string rotaAbsoluta = new Uri(Page.Url).AbsolutePath;

        Assert.AreEqual("/", rotaAbsoluta);
    }
}
