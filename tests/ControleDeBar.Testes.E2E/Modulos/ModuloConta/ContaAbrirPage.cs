using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

public class ContaAbrirPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string Url { get; } =
        $"{urlBase}/Conta/Abrir";

    public ILocator SelectMesa =>
        _page.Locator("select[name='MesaId']");

    public ILocator SelectGarcom =>
        _page.Locator("select[name='GarcomId']");

    public ILocator CampoNomeCliente =>
        _page.Locator("input[name='NomeCliente']");

    public ILocator BotaoAbrir =>
        _page.GetByRole(
            AriaRole.Button,
            new() { Name = "Abrir Conta" }
        );

    public async Task IrParaAsync() =>
        await _page.GotoAsync(Url);

    public async Task PreencherAsync(
        string descricaoMesa,
        string nomeGarcom,
        string nomeCliente)
    {
        await SelectMesa.SelectOptionAsync(
            new SelectOptionValue
            {
                Label = descricaoMesa
            }
        );

        await SelectGarcom.SelectOptionAsync(
            new SelectOptionValue
            {
                Label = nomeGarcom
            }
        );

        await CampoNomeCliente.FillAsync(nomeCliente);
    }

    public async Task ConfirmarAsync() =>
        await BotaoAbrir.ClickAsync();
}
