using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloProduto;

public class ProdutoFormPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;

    public string UrlCadastrar { get; } = $"{urlBase}/Produto/Cadastrar";

    public ILocator CampoNome => _page.GetByLabel("Nome");
    public ILocator CampoPrecoVenda => _page.GetByLabel("Preço de Venda");
    public ILocator BotaoConfirmar => _page.GetByRole(AriaRole.Button, new() { Name = "Confirmar" });
    public ILocator MensagensErro => _page.Locator(".field-validation-error, .validation-summary-errors, .text-danger");

    public async Task IrParaCadastrarAsync() => await _page.GotoAsync(UrlCadastrar);

    public async Task PreencherFormularioAsync(string nome, string precoVenda)
    {
        await CampoNome.FillAsync(nome);
        await CampoPrecoVenda.FillAsync(precoVenda);
    }

    public async Task ConfirmarAsync() => await BotaoConfirmar.ClickAsync();
}
