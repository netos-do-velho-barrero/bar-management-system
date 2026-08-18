using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloPedidoConta;

public class PedidoContaAdicionarPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;
    private readonly string _urlBase = urlBase;

    public string Url(Guid contaId) => $"{_urlBase}/PedidoConta/Adicionar?contaId={contaId}";

    private ILocator SelectProduto => _page.Locator("select[name='ProdutoId']");
    private ILocator CampoQuantidade => _page.Locator("input[name='Quantidade']");
    private ILocator BotaoAdicionar => _page.GetByRole(AriaRole.Button, new() { Name = "Adicionar" });

    public async Task IrParaAsync(Guid contaId) => await _page.GotoAsync(Url(contaId));

    public async Task SelecionarProdutoAsync(string nomeProduto)
    {
        string? produtoId = await SelectProduto
            .Locator("option", new() { HasText = nomeProduto })
            .GetAttributeAsync("value");

        await SelectProduto.SelectOptionAsync(produtoId);
    }

    public async Task PreencherQuantidadeAsync(int quantidade) =>
        await CampoQuantidade.FillAsync(quantidade.ToString());

    public async Task ConfirmarAsync() => await BotaoAdicionar.ClickAsync();
}
