using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloPedidoConta;

public class ContaDetalhesPage(IPage page, string urlBase)
{
    private readonly IPage _page = page;
    private readonly string _urlBase = urlBase;

    public string Url(Guid contaId) => $"{_urlBase}/Conta/Detalhes/{contaId}";

    // ValorTotal fica no <tfoot>, na célula com classe "fw-bold", formatado
    // via ToString("C") (Views/Conta/_PedidosPartial ou equivalente).
    public ILocator ValorTotal => _page.Locator("tfoot td.fw-bold");

    // Mensagem exibida quando a conta ainda não tem nenhum pedido.
    public ILocator MensagemSemPedidos => _page.GetByText("Nenhum pedido nesta conta ainda.");

    private ILocator LinhaDoProduto(string nomeProduto) =>
        _page.Locator("tbody tr", new PageLocatorOptions { HasText = nomeProduto });

    private ILocator CampoQuantidade(string nomeProduto) =>
        LinhaDoProduto(nomeProduto).Locator("input[name='Quantidade']");

    private ILocator BotaoAtualizarQuantidade(string nomeProduto) =>
        LinhaDoProduto(nomeProduto).Locator("button[title='Atualizar quantidade']");

    private ILocator BotaoRemover(string nomeProduto) =>
        LinhaDoProduto(nomeProduto).GetByRole(AriaRole.Button, new() { Name = "Remover" });

    public async Task IrParaAsync(Guid contaId) => await _page.GotoAsync(Url(contaId));

    public async Task AlterarQuantidadeAsync(string nomeProduto, int novaQuantidade)
    {
        await CampoQuantidade(nomeProduto).FillAsync(novaQuantidade.ToString());
        await BotaoAtualizarQuantidade(nomeProduto).ClickAsync();
    }

    // O botão de remover dispara um confirm() nativo (onsubmit="return confirm(...)").
    // Sem esse handler, o Playwright descarta o diálogo automaticamente e o
    // formulário nunca é enviado.
    public async Task RemoverAsync(string nomeProduto)
    {
        void AceitarDialogo(object? sender, IDialog dialog) => dialog.AcceptAsync();

        _page.Dialog += AceitarDialogo;

        try
        {
            await BotaoRemover(nomeProduto).ClickAsync();
            await _page.WaitForURLAsync(url => url.Contains("/Conta/Detalhes/"));
        }
        finally
        {
            _page.Dialog -= AceitarDialogo;
        }
    }
}
