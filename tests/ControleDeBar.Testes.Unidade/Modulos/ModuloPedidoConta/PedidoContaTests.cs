using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Testes.Unidade.Modulos.ModuloPedidoConta;

[TestClass]
public sealed class PedidoContaTests
{
    #region Testes de Validação (Quantidade)

    [TestMethod]
    public void Validar_SemInformarQuantidade_DeveRetornarErro()
    {
        // CT-PED-002: Adicionar produto sem informar a quantidade
        // Arranjo — "sem informar" equivale ao valor default do int (0),
        // já que Quantidade não é nullable no domínio.
        Produto produto = new Produto("Cerveja Long Neck", 12.50m);
        PedidoConta pedido = new PedidoConta(produto, quantidade: default);

        // Ação
        List<string> erros = pedido.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade\" deve ser um número positivo.", erros.First());
    }

    [TestMethod]
    public void Validar_ComQuantidadeIgualAZero_DeveRetornarErro()
    {
        // CT-PED-003: Adicionar produto com quantidade igual a zero
        // Arranjo
        Produto produto = new Produto("Cerveja Long Neck", 12.50m);
        PedidoConta pedido = new PedidoConta(produto, quantidade: 0);

        // Ação
        List<string> erros = pedido.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade\" deve ser um número positivo.", erros.First());
    }

    [TestMethod]
    public void Validar_ComQuantidadeNegativa_DeveRetornarErro()
    {
        // CT-PED-004: Adicionar produto com quantidade negativa
        // Arranjo
        Produto produto = new Produto("Cerveja Long Neck", 12.50m);
        PedidoConta pedido = new PedidoConta(produto, quantidade: -3);

        // Ação
        List<string> erros = pedido.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade\" deve ser um número positivo.", erros.First());
    }

    #endregion

    #region Testes de Validação (Produto)

    [TestMethod]
    public void Validar_SemSelecionarProduto_DeveRetornarErro()
    {
        // CT-PED-005: Adicionar pedido sem selecionar o produto
        // Arranjo — Quantidade válida para isolar o erro apenas no campo Produto.
        PedidoConta pedido = new PedidoConta
        {
            Quantidade = 2
        };

        // Ação
        List<string> erros = pedido.Validar();

        // Asserção
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Produto\" deve ser preenchido.", erros.First());
    }

    #endregion

    #region Testes de Cálculo

    [TestMethod]
    public void Subtotal_DeveSerCalculadoPelaMultiplicacaoDePrecoUnitarioEQuantidade()
    {
        // CT-PED-010: Calcular o subtotal do item pela multiplicação de preço unitário e quantidade
        // Arranjo
        Produto produto = new Produto("Caipirinha", 19.90m);
        PedidoConta pedido = new PedidoConta(produto, quantidade: 3);

        // Ação
        decimal subtotal = pedido.Subtotal;

        // Asserção
        Assert.AreEqual(59.70m, subtotal);
    }

    [TestMethod]
    public void Atualizar_ComQuantidadeAlteradaParaZero_DevePassarAValidarComErro()
    {
        // CT-PED-012 (nível de domínio): Alterar a quantidade de um pedido para zero
        // Arranjo
        Produto produto = new Produto("Cerveja Long Neck", 12.50m);
        PedidoConta pedidoExistente = new PedidoConta(produto, quantidade: 5);
        PedidoConta pedidoAtualizado = new PedidoConta { Quantidade = 0 };

        // Ação
        pedidoExistente.Atualizar(pedidoAtualizado);
        List<string> erros = pedidoExistente.Validar();

        // Asserção
        Assert.AreEqual(0, pedidoExistente.Quantidade);
        Assert.AreEqual(1, erros.Count);
        Assert.AreEqual("O campo \"Quantidade\" deve ser um número positivo.", erros.First());
    }

    #endregion
}
