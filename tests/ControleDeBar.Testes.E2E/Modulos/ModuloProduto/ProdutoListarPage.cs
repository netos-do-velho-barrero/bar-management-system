using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

public class ProdutoListarPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string Url { get; } = $"{urlBase}/Produto/Listar";

    public async Task IrParaAsync() => await _page.GotoAsync(Url);

    public ILocator ObterLinhaPorNome(string nome) => _page.GetByText(nome);

    public async Task ExcluirAsync(string nome)
    {
        ILocator linha = _page.Locator(".card", new PageLocatorOptions { HasText = nome });
        await linha.GetByRole(AriaRole.Link, new() { Name = "Excluir" }).ClickAsync();
    }
}
