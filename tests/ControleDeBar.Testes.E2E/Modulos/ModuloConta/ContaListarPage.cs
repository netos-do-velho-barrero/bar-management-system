using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloConta;

public class ContaListarPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string Url { get; } = $"{urlBase}/Conta/Listar";

    public ILocator ObterLinhaPorCliente(string nomeCliente) =>
        _page.Locator("#todas tr", new() { HasText = nomeCliente });

    public ILocator ObterLinhaHistoricoPorCliente(string nomeCliente) =>
        _page.Locator("#fechadas tr", new() { HasText = nomeCliente });

    public async Task IrParaAsync() => await _page.GotoAsync(Url);

    public async Task FecharAsync(string nomeCliente)
    {
        string? href = await ObterLinhaPorCliente(nomeCliente)
            .Locator("a[title='Fechar conta']")
            .GetAttributeAsync("href");

        Assert.IsNotNull(href);
        await _page.GotoAsync($"{new Uri(_page.Url).GetLeftPart(UriPartial.Authority)}{href}");
    }
}
