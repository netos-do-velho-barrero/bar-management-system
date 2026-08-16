using System.Linq.Expressions;
using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Compartilhado.Orm;

public abstract class RepositorioBaseEmOrm<T>(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) where T : EntidadeBase<T>
{
    protected readonly DbSet<T> registros = dbContext.Set<T>();

    protected readonly IProvedorDeUsuario provedorDeUsuario = provedorDeUsuario;

    public void Cadastrar(T entidade)
    {
        if (entidade is IEntidadeDoUsuario entidadeDoUsuario)
        {
            Guid userId = ObterUsuarioAutenticado();

            entidadeDoUsuario.UserId = userId;
        }

        registros.Add(entidade);

        dbContext.SaveChanges();
    }

    public bool Editar(Guid idSelecionado, T entidadeAtualizada)
    {
        T? registroSelecionado = SelecionarPorId(idSelecionado);

        if (registroSelecionado == null)
            return false;

        registroSelecionado.Atualizar(entidadeAtualizada);

        dbContext.SaveChanges();

        return true;
    }

    public bool Excluir(Guid idSelecionado)
    {
        T? registroSelecionado = SelecionarPorId(idSelecionado);

        if (registroSelecionado == null)
            return false;

        registros.Remove(registroSelecionado);

        dbContext.SaveChanges();

        return true;
    }

    public virtual T? SelecionarPorId(Guid idSelecionado)
    {
        return registros
            .Where(FiltroDoUsuarioAtual())
            .SingleOrDefault(c => c.Id == idSelecionado);
    }

    public virtual List<T> SelecionarTodos()
    {
        return registros
            .Where(FiltroDoUsuarioAtual())
            .ToList();
    }

    public virtual List<T> Filtrar(Func<T, bool> filtro)
    {
        return registros
            .Where(FiltroDoUsuarioAtual())
            .Where(filtro)
            .ToList();
    }

    private Expression<Func<T, bool>> FiltroDoUsuarioAtual()
    {
        if (!typeof(IEntidadeDoUsuario).IsAssignableFrom(typeof(T)))
            return _ => true;

        Guid userId = ObterUsuarioAutenticado();

        ParameterExpression parametro =
            Expression.Parameter(typeof(T), "entidade");

        MemberExpression userIdDaEntidade =
            Expression.Property(
                parametro,
                nameof(IEntidadeDoUsuario.UserId)
            );

        ConstantExpression userIdAtual =
            Expression.Constant(userId);

        BinaryExpression igualdade =
            Expression.Equal(userIdDaEntidade, userIdAtual);

        return Expression.Lambda<Func<T, bool>>(
            igualdade,
            parametro
        );
    }

    private Guid ObterUsuarioAutenticado()
    {
        if (!provedorDeUsuario.EstaAutenticado || provedorDeUsuario.Id == null)
        {
            throw new InvalidOperationException(
                "Não existe um usuário autenticado."
            );
        }

        return provedorDeUsuario.Id.Value;
    }
}
