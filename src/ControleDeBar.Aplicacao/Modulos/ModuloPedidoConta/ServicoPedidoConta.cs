using FluentResults;
using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Compartilhado.Identity;
using Microsoft.Extensions.Logging;

namespace ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;

public class ServicoPedidoConta(
    IRepositorioPedidoConta repositorioPedidoConta,
    IRepositorioConta repositorioConta,
    IRepositorioProduto repositorioProduto,
    IProvedorDeUsuario provedorDeUsuario,
    ILogger<ServicoPedidoConta>? logger = null
) : ServicoBase<PedidoConta>
{
    public Result Adicionar(AdicionarPedidoContaDto dto)
    {
        Conta? contaSemFiltro = repositorioConta.SelecionarPorIdSemFiltro(dto.ContaId);

        if (contaSemFiltro is not null && contaSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Conta),
                provedorDeUsuario.Id,
                contaSemFiltro.UserId,
                nameof(dto.ContaId)
            );

            return Falha(
                nameof(dto.ContaId),
                "Esta conta não pertence ao seu bar."
            );
        }

        Conta? conta = repositorioConta.SelecionarPorId(dto.ContaId);

        if (conta == null)
            return Falha(string.Empty, "Conta não encontrada.");

        if (conta.Situacao == SituacaoConta.Fechada)
            return Falha(string.Empty, "Não é possível adicionar pedidos a uma conta fechada.");

        Produto? produtoSemFiltro = repositorioProduto.SelecionarPorIdSemFiltro(dto.ProdutoId);

        if (produtoSemFiltro is not null && produtoSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Produto),
                provedorDeUsuario.Id,
                produtoSemFiltro.UserId,
                nameof(dto.ProdutoId)
            );

            return Falha(
                nameof(dto.ProdutoId),
                "Este produto não pertence ao seu bar."
            );
        }

        Produto? produto = repositorioProduto.SelecionarPorId(dto.ProdutoId);

        if (produto == null)
        {
            return Falha(nameof(dto.ProdutoId), "Selecione um produto válido.");
        }

        // Validação Multi-Tenancy: Produto pertence ao mesmo bar?
        if (produto.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Produto),
                provedorDeUsuario.Id,
                produto.UserId,
                nameof(dto.ProdutoId)
            );

            return Falha(
                nameof(dto.ProdutoId),
                "Este produto não pertence ao seu bar."
            );
        }

        // Validação Multi-Tenancy: Conta pertence ao mesmo bar?
        if (conta.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Conta),
                provedorDeUsuario.Id,
                conta.UserId,
                nameof(dto.ContaId)
            );

            return Falha(
                nameof(dto.ContaId),
                "Esta conta não pertence ao seu bar."
            );
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
        PedidoConta? pedidoSemFiltro = repositorioPedidoConta.SelecionarPorIdSemFiltro(dto.Id);

        if (pedidoSemFiltro is not null && pedidoSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(PedidoConta),
                provedorDeUsuario.Id,
                pedidoSemFiltro.UserId,
                nameof(dto.Id)
            );

            return Falha(string.Empty, "Este pedido não pertence ao seu bar.");
        }

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
        PedidoConta? pedidoSemFiltro = repositorioPedidoConta.SelecionarPorIdSemFiltro(id);

        if (pedidoSemFiltro is not null && pedidoSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(PedidoConta),
                provedorDeUsuario.Id,
                pedidoSemFiltro.UserId,
                nameof(id)
            );

            return Falha(string.Empty, "Este pedido não pertence ao seu bar.");
        }

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
