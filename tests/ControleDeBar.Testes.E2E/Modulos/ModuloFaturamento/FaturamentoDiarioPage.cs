using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloFaturamento;

public class FaturamentoDiarioPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;
    private readonly string _urlBase = urlBase;

    public string Url() => $"{_urlBase}/Faturamento/Diario";

    public async Task IrParaAsync() => await _page.GotoAsync(Url());
}
