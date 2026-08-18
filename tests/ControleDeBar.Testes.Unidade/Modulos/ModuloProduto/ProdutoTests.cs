using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloProduto;

[TestClass]
public sealed class ProdutoTests
{
    #region Testes de Validação no Cadastro (Cenários Positivos)

    [TestMethod]
    public void Validar_ComNomeEPrecoValidos_DevePassarSemErros()
    {
        // CT-PRO-001: Cadastrar produto com nome e preço válidos
        // Arranjo
        Produto produto = new Produto(
            nome: "Cerveja Long Neck",
            precoVenda: 12.50m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(0, erros.Count);
    }

    [TestMethod]
    public void Validar_ComPrecoDecimalDeDuasCasas_DevePassarSemErros()
    {
        // CT-PRO-008: Cadastrar produto com preço decimal (duas casas)
        // Arranjo
        Produto produto = new Produto(
            nome: "Caipirinha",
            precoVenda: 19.90m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(0, erros.Count);
    }

    #endregion

    #region Testes de Validação no Cadastro (Cenários Negativos - Nome)

    [TestMethod]
    public void Validar_SemNome_DeveRetornarErro()
    {
        // CT-PRO-002: Cadastrar produto sem informar o nome
        // Arranjo
        Produto produto = new Produto(
            nome: string.Empty,
            precoVenda: 12.50m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }

    [TestMethod]
    public void Validar_ComNomeAbaixoDoMinimo_DeveRetornarErro()
    {
        // CT-PRO-004: Cadastrar produto com nome abaixo do mínimo (1 caractere)
        // Arranjo
        Produto produto = new Produto(
            nome: "A",
            precoVenda: 12.50m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }

    [TestMethod]
    public void Validar_ComNomeAcimaDoMaximo_DeveRetornarErro()
    {
        // CT-PRO-005: Cadastrar produto com nome acima do máximo (101 caracteres)
        // Arranjo
        Produto produto = new Produto(
            nome: new string('A', 101),
            precoVenda: 12.50m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Nome\" deve conter entre 2 e 100 caracteres.", erros.First());
    }

    #endregion

    #region Testes de Validação no Cadastro (Cenários Negativos - Preço)

    [TestMethod]
    public void Validar_SemPreco_DeveRetornarErro()
    {
        // CT-PRO-003: Cadastrar produto sem informar o preço
        // Arranjo — "sem informar" equivale ao valor default do decimal (0),
        // já que PrecoVenda não é nullable no domínio.
        Produto produto = new Produto(
            nome: "Cerveja Long Neck",
            precoVenda: default
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Preço de venda\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_ComPrecoIgualAZero_DeveRetornarErro()
    {
        // CT-PRO-006: Cadastrar produto com preço igual a zero
        // Arranjo
        Produto produto = new Produto(
            nome: "Cerveja Long Neck",
            precoVenda: 0m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Preço de venda\" deve ser maior que zero.", erros.First());
    }

    [TestMethod]
    public void Validar_ComPrecoNegativo_DeveRetornarErro()
    {
        // CT-PRO-007: Cadastrar produto com preço negativo
        // Arranjo
        Produto produto = new Produto(
            nome: "Cerveja Long Neck",
            precoVenda: -5.00m
        );

        // Ação
        List<string> erros = produto.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Preço de venda\" deve ser maior que zero.", erros.First());
    }

    #endregion
}
