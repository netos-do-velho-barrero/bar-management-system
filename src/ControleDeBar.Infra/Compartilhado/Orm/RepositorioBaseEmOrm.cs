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

    protected readonly ControleDeBarDbContext dbContext = dbContext;

    protected IQueryable<T> RegistrosDoUsuario()
    {
        if (!typeof(IEntidadeDoUsuario).IsAssignableFrom(typeof(T)))
            return registros;

        if (!provedorDeUsuario.EstaAutenticado ||
            provedorDeUsuario.Id is null)
        {
            throw new InvalidOperationException(
                "Não existe um usuário autenticado."
            );
        }

        Guid userId = provedorDeUsuario.Id.Value;

        ParameterExpression parametro =
            Expression.Parameter(typeof(T), "entidade");

        MemberExpression propriedadeUserId =
            Expression.Property(
                parametro,
                nameof(IEntidadeDoUsuario.UserId)
            );

        ConstantExpression valorUserId =
            Expression.Constant(userId);

        BinaryExpression igualdade =
            Expression.Equal(
                propriedadeUserId,
                valorUserId
            );

        Expression<Func<T, bool>> filtro =
            Expression.Lambda<Func<T, bool>>(
                igualdade,
                parametro
            );

        return registros.Where(filtro);
    }

    public void Cadastrar(T entidade)
    {
        if (entidade is IEntidadeDoUsuario entidadeDoUsuario)
        {
            if (!provedorDeUsuario.EstaAutenticado ||
                provedorDeUsuario.Id is null)
            {
                throw new InvalidOperationException(
                    "Não existe um usuário autenticado."
                );
            }

            entidadeDoUsuario.UserId =
                provedorDeUsuario.Id.Value;
        }

        registros.Add(entidade);

        dbContext.SaveChanges();
    }

    public bool Editar(
        Guid idSelecionado,
        T entidadeAtualizada
    )
    {
        T? registroSelecionado =
            SelecionarPorId(idSelecionado);

        if (registroSelecionado is null)
            return false;

        registroSelecionado.Atualizar(entidadeAtualizada);

        dbContext.SaveChanges();

        return true;
    }

    public bool Excluir(Guid idSelecionado)
    {
        T? registroSelecionado =
            SelecionarPorId(idSelecionado);

        if (registroSelecionado is null)
            return false;

        registros.Remove(registroSelecionado);

        dbContext.SaveChanges();

        return true;
    }

    public virtual T? SelecionarPorId(Guid idSelecionado)
    {
        return RegistrosDoUsuario()
            .SingleOrDefault(e => e.Id == idSelecionado);
    }

    public virtual T? SelecionarPorIdSemFiltro(Guid idSelecionado)
    {
        return registros
            .SingleOrDefault(e => e.Id == idSelecionado);
    }

    public virtual List<T> SelecionarTodos()
    {
        return RegistrosDoUsuario()
            .ToList();
    }

    public virtual List<T> Filtrar(
        Func<T, bool> filtro
    )
    {
        return RegistrosDoUsuario()
            .AsEnumerable()
            .Where(filtro)
            .ToList();
    }
}
