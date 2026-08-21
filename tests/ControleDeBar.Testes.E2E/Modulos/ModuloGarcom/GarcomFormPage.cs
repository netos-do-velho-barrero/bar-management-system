using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloGarcom;

public class GarcomFormPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string UrlCadastrar { get; } =
        $"{urlBase}/Garcom/Cadastrar";

    public ILocator CampoNome =>
        _page.Locator("input[name='Nome']");

    public ILocator BotaoConfirmar =>
        _page.GetByRole(
            AriaRole.Button,
            new() { Name = "Confirmar" }
        );

    public ILocator MensagensErro =>
        _page.Locator(
            ".field-validation-error, .validation-summary-errors, .text-danger"
        );

    public async Task IrParaCadastrarAsync() =>
        await _page.GotoAsync(UrlCadastrar);

    public async Task PreencherAsync(string nome)
    {
        await CampoNome.FillAsync(nome);
    }

    public async Task ConfirmarAsync() =>
        await BotaoConfirmar.ClickAsync();
}
