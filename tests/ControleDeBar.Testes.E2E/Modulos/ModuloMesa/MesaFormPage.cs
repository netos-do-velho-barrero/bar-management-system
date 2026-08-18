using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

public class MesaFormPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string UrlCadastrar { get; } = $"{urlBase}/Mesa/Cadastrar";

    public ILocator CampoNumero => _page.Locator("input[name='Numero']");
    public ILocator CampoQuantidadeLugares => _page.Locator("input[name='QuantidadeLugares']");
    public ILocator BotaoConfirmar => _page.GetByRole(AriaRole.Button, new() { Name = "Cadastrar" });
    public ILocator MensagensErro => _page.Locator(".field-validation-error, .validation-summary-errors, .text-danger");

    public async Task IrParaCadastrarAsync() => await _page.GotoAsync(UrlCadastrar);

    public async Task PreencherAsync(int numero, int quantidadeLugares)
    {
        await CampoNumero.FillAsync(numero > 0 ? numero.ToString() : string.Empty);
        await CampoQuantidadeLugares.FillAsync(quantidadeLugares.ToString());
    }

    public async Task ConfirmarAsync() => await BotaoConfirmar.ClickAsync();
}
