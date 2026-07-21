using FluentResults;
using GeradorDeProvas.Aplicacao.Compartilhado;
using GeradorDeProvas.Dominio.Modulos.ModuloDisciplina;

namespace GeradorDeProvas.Aplicacao.Modulos.ModuloDisciplina;

public class ServicoDisciplina(
    IRepositorioDisciplina repositorioDisciplina
) : ServicoBase<Disciplina>
{
    public Result Cadastrar(CadastrarDisciplinaDto dto)
    {
        if (ExisteDisciplinaComMesmoNome(dto.Nome))
            return Falha(nameof(dto.Nome), "Já existe uma disciplina com este nome.");

        Disciplina novaDisciplina = new(dto.Nome);

        Result resultadoValidacao = ValidarEntidade(novaDisciplina);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioDisciplina.Cadastrar(novaDisciplina);

        return Result.Ok();
    }

    public Result Editar(EditarDisciplinaDto dto)
    {
        if (ExisteDisciplinaComMesmoNome(dto.Nome, dto.Id))
            return Falha(nameof(dto.Nome), "Já existe uma disciplina com este nome.");

        Disciplina disciplinaAtualizada = new(dto.Nome);

        Result resultadoValidacao = ValidarEntidade(disciplinaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar = repositorioDisciplina.Editar(dto.Id, disciplinaAtualizada);

        if (!conseguiuEditar)
            return Falha(string.Empty, "Disciplina não encontrada.");

        return Result.Ok();
    }

    public Result Excluir(Guid id)
    {
        Disciplina? disciplina = repositorioDisciplina.SelecionarPorId(id);

        if (disciplina == null)
            return Falha(string.Empty, "Disciplina não encontrada.");

        repositorioDisciplina.Excluir(id);

        return Result.Ok();
    }

    public List<ListarDisciplinaDto> SelecionarTodos()
    {
        return repositorioDisciplina
            .SelecionarTodos()
            .Select(d => new ListarDisciplinaDto(d.Id, d.Nome))
            .ToList();
    }

    public Result<DetalhesDisciplinaDto> SelecionarPorId(Guid id)
    {
        Disciplina? disciplina = repositorioDisciplina.SelecionarPorId(id);

        if (disciplina == null)
            return Result.Fail("Disciplina não encontrada.");

        return Result.Ok(new DetalhesDisciplinaDto(disciplina.Id, disciplina.Nome));
    }

    private bool ExisteDisciplinaComMesmoNome(string nome, Guid? idIgnorado = null)
    {
        string nomeNormalizado = NormalizarNome(nome);

        return repositorioDisciplina
            .SelecionarTodos()
            .Any(d => d.Id != idIgnorado && NormalizarNome(d.Nome) == nomeNormalizado);
    }

    private static string NormalizarNome(string nome)
    {
        return nome.Trim().ToLowerInvariant();
    }
}
