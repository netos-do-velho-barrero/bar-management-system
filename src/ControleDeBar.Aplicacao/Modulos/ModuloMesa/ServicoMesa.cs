using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using FluentResults;

namespace ControleDeBar.Aplicacao.Modulos.ModuloMesa;

public class ServicoMesa(
    IRepositorioMesa repositorioMesa
) : ServicoBase<Mesa>
{
    public Result Cadastrar(CadastrarMesaDto dto)
    {
        if (ExisteMesaComMesmoNumero(dto.Numero))
            return Falha(nameof(dto.Numero), "Já existe uma mesa com este número.");

        Mesa novaMesa = new(
            dto.Numero,
            dto.QuantidadeLugares
        );

        Result resultadoValidacao = ValidarEntidade(novaMesa);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioMesa.Cadastrar(novaMesa);

        return Result.Ok();
    }

    public Result Editar(EditarMesaDto dto)
    {
        if (ExisteMesaComMesmoNumero(dto.Numero, dto.Id))
            return Falha(nameof(dto.Numero), "Já existe uma mesa com este número.");

        Mesa mesaAtualizada = new(
            dto.Numero,
            dto.QuantidadeLugares
        );

        Result resultadoValidacao = ValidarEntidade(mesaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioMesa.Editar(
            dto.Id,
            mesaAtualizada
        );

        if (!conseguiuEditar)
            return Falha(string.Empty, "Mesa não encontrada.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null)
            return Falha(string.Empty, "Mesa não encontrada.");

        if (mesa.Status == StatusMesa.Ocupada)
            return Falha(
                string.Empty,
                "Não é possível excluir uma mesa ocupada."
            );

        repositorioMesa.Excluir(id);

        return Result.Ok();
    }

    public List<ListarMesaDto> SelecionarTodos()
    {
        return repositorioMesa
            .SelecionarTodos()
            .Select(m => new ListarMesaDto(
                m.Id,
                m.Numero,
                m.QuantidadeLugares,
                m.Status.ToString()
            ))
            .ToList();
    }

    public Result<DetalhesMesaDto> SelecionarPorId(Guid id)
    {
        Mesa? mesa = repositorioMesa.SelecionarPorId(id);

        if (mesa == null)
            return Result.Fail("Mesa não encontrada.");

        return Result.Ok(
            new DetalhesMesaDto(
                mesa.Id,
                mesa.Numero,
                mesa.QuantidadeLugares,
                mesa.Status.ToString()
            )
        );
    }

    private bool ExisteMesaComMesmoNumero(
        int numero,
        Guid? idIgnorado = null
    )
    {
        return repositorioMesa
            .SelecionarTodos()
            .Any(m =>
                m.Numero == numero &&
                m.Id != idIgnorado
            );
    }
}
