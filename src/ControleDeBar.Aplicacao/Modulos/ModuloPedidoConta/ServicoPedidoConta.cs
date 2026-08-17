using FluentResults;
using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;

public class ServicoPedidoConta(
    IRepositorioPedidoConta repositorioPedidoConta,
    IRepositorioConta repositorioConta,
    IRepositorioProduto repositorioProduto
) : ServicoBase<PedidoConta>
{
    public Result Adicionar(AdicionarPedidoContaDto dto)
    {
        Conta? conta = repositorioConta.SelecionarPorId(dto.ContaId);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.Situacao == SituacaoConta.Fechada)
            return Falha(string.Empty, "Não é possível adicionar pedidos a uma conta fechada.");

        Produto? produto = repositorioProduto.SelecionarPorId(dto.ProdutoId);

        if (produto == null)
        {
            return Falha(nameof(dto.ProdutoId), "Selecione um produto válido.");
        }

        PedidoConta novoPedido = new(produto, dto.Quantidade)
        {
            Conta = conta
        };

        Result resultadoValidacao = ValidarEntidade(novoPedido);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioPedidoConta.Cadastrar(novoPedido);

        return Result.Ok();
    }

    public Result EditarQuantidade(EditarQuantidadePedidoContaDto dto)
    {
        PedidoConta? pedido = repositorioPedidoConta.SelecionarPorId(dto.Id);

        if (pedido == null)
            return Falha(string.Empty, "Pedido não encontrado.");

        if (pedido.Conta?.Situacao == SituacaoConta.Fechada)
            return Falha(string.Empty, "Não é possível alterar pedidos de uma conta fechada.");

        if (dto.Quantidade <= 0)
            return Falha(nameof(dto.Quantidade), "O campo \"Quantidade\" deve ser um número positivo.");

        PedidoConta pedidoAtualizado = new() { Quantidade = dto.Quantidade };

        bool conseguiuEditar = repositorioPedidoConta.Editar(dto.Id, pedidoAtualizado);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Pedido não encontrado.");

        return Result.Ok();
    }

    public Result Remover(Guid id)
    {
        PedidoConta? pedido = repositorioPedidoConta.SelecionarPorId(id);

        if (pedido == null)
            return Falha(string.Empty, "Pedido não encontrado.");

        if (pedido.Conta?.Situacao == SituacaoConta.Fechada)
            return Falha(string.Empty, "Não é possível remover pedidos de uma conta fechada.");

        repositorioPedidoConta.Excluir(id);

        return Result.Ok();
    }

    public List<ListarPedidoContaDto> SelecionarPorConta(Guid contaId)
    {
        return repositorioPedidoConta
            .Filtrar(p => p.Conta.Id == contaId)
            .Select(p => new ListarPedidoContaDto(p.Id, p.Produto.Nome, p.Quantidade, p.PrecoUnitario, p.Subtotal))
            .ToList();
    }

    public List<OpcaoProdutoPedidoDto> SelecionarProdutosDisponiveis()
    {
        return repositorioProduto
            .SelecionarTodos()
            .Select(p => new OpcaoProdutoPedidoDto(p.Id, p.Nome, p.PrecoVenda))
            .ToList();
    }
}
