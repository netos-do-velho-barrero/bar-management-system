using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloMesa;

public class MesaListarPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string Url { get; } = $"{urlBase}/Mesa/Listar";

    public ILocator ObterLinhaPorNumero(int numero) =>
        _page.GetByText($"Mesa {numero}", new() { Exact = false });

    public async Task IrParaAsync() => await _page.GotoAsync(Url);
}
