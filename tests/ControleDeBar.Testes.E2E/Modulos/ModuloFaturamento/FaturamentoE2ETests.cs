using ControleDeBar.Testes.E2E.Compartilhado;
using Microsoft.Playwright;

namespace ControleDeBar.Testes.E2E.Modulos.ModuloFaturamento;

[TestClass]
public sealed class FaturamentoE2ETests : E2ETestsBase
{
    private FaturamentoDiarioPage _faturamentoDiarioPage = null!;

    [TestInitialize]
    public void SetupPages()
    {
        _faturamentoDiarioPage = new FaturamentoDiarioPage(Page, UrlBase);
    }

    [TestMethod]
    public async Task CT_FAT_007_DeveRedirecionarParaLogin_AoTentarVisualizarFaturamentoSemAutenticacao()
    {
        // CT-FAT-007: Tentar visualizar o faturamento sem autenticação
        // Arranjo — nenhum usuário logado; cada teste começa com contexto novo
        // (TestApplicationFactory + PageTest isolam sessão automaticamente)

        // Ação
        await _faturamentoDiarioPage.IrParaAsync();

        // Asserção
        await Expect(Page).ToHaveURLAsync(
            new System.Text.RegularExpressions.Regex(@"/Autenticacao/Entrar")
        );
    }
}
