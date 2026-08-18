using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

public class ProdutoExcluirPage(IPage page)
{
    private readonly IPage _page = page;

    // Texto exato do alert em Views/Produto/Excluir.cshtml
    public ILocator MensagemConfirmacao => _page.GetByText("Deseja realmente excluir este produto?", new() { Exact = true });
    public ILocator MensagensErro => _page.Locator(".text-danger, .alert-danger");
    public ILocator BotaoConfirmar => _page.GetByRole(AriaRole.Button, new() { Name = "Confirmar", Exact = true });

    public async Task ConfirmarAsync() => await BotaoConfirmar.ClickAsync();
}
