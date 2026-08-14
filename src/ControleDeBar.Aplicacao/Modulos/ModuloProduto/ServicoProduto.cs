using FluentResults;
using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloProduto;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;

namespace ControleDeBar.Aplicacao.Modulos.ModuloProduto;

public class ServicoProduto(
    IRepositorioProduto repositorioProduto,
    IRepositorioPedidoConta repositorioPedidoConta
) : ServicoBase<Produto>
{
    public Result Cadastrar(CadastrarProdutoDto dto)
    {
        if (ExisteProdutoComMesmoNome(dto.Nome))
            return Falha(nameof(dto.Nome), "Já existe um produto com este nome.");

        Produto novoProduto = new(dto.Nome, dto.PrecoVenda);

        Result resultadoValidacao = ValidarEntidade(novoProduto);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioProduto.Cadastrar(novoProduto);

        return Result.Ok();
    }

    public Result Editar(EditarProdutoDto dto)
    {
        if (ExisteProdutoComMesmoNome(dto.Nome, dto.Id))
            return Falha(nameof(dto.Nome), "Já existe um produto com este nome.");

        Produto produtoAtualizado = new(dto.Nome, dto.PrecoVenda);

        Result resultadoValidacao = ValidarEntidade(produtoAtualizado);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        // A alteração de preço aqui não afeta PedidoConta já registrados,
        // pois PedidoConta.PrecoUnitario é um snapshot tirado no momento da inclusão.
        bool conseguiuEditar = repositorioProduto.Editar(dto.Id, produtoAtualizado);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Produto não encontrado.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return Falha(string.Empty, "Produto não encontrado.");

        if (PossuiPedidosVinculados(id))
            return Falha(string.Empty, "Não é possível excluir este produto, pois existem pedidos vinculados a ele.");

        repositorioProduto.Excluir(id);

        return Result.Ok();
    }

    public List<ListarProdutoDto> SelecionarTodos()
    {
        return repositorioProduto
            .SelecionarTodos()
            .Select(p => new ListarProdutoDto(p.Id, p.Nome, p.PrecoVenda))
            .ToList();
    }

    public Result<DetalhesProdutoDto> SelecionarPorId(Guid id)
    {
        Produto? produto = repositorioProduto.SelecionarPorId(id);

        if (produto == null)
            return Result.Fail("Produto não encontrado.");

        return Result.Ok(new DetalhesProdutoDto(produto.Id, produto.Nome, produto.PrecoVenda));
    }

    private bool ExisteProdutoComMesmoNome(string nome, Guid? idIgnorado = null)
    {
        string nomeNormalizado = NormalizarNome(nome);

        return repositorioProduto
            .SelecionarTodos()
            .Any(p => p.Id != idIgnorado && NormalizarNome(p.Nome) == nomeNormalizado);
    }

    private static string NormalizarNome(string nome)
    {
        return nome.Trim().ToLowerInvariant();
    }

    private bool PossuiPedidosVinculados(Guid produtoId)
    {
        return repositorioPedidoConta
            .SelecionarTodos()
            .Any(p => p.Produto.Id == produtoId);
    }
}
