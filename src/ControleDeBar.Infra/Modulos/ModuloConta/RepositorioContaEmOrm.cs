using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloConta;

public sealed class RepositorioContaEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<Conta>(
    dbContext,
    provedorDeUsuario
), IRepositorioConta
{
    public override Conta? SelecionarPorId(Guid idSelecionado)
    {
        return RegistrosDoUsuario()
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Include(c => c.Pedidos)
                .ThenInclude(p => p.Produto)
            .SingleOrDefault(c => c.Id == idSelecionado);
    }

    public override List<Conta> SelecionarTodos()
    {
        return RegistrosDoUsuario()
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Include(c => c.Pedidos)
            .ThenInclude(p => p.Produto)
            .ToList();
    }

    public void AlterarSituacao(
        Guid contaId,
        SituacaoConta novaSituacao
    )
    {
        Conta? conta = SelecionarPorId(contaId);

        if (conta is null)
            return;

        conta.Situacao = novaSituacao;

        dbContext.SaveChanges();
    }

    public List<Conta> SelecionarAbertas()
    {
        return RegistrosDoUsuario()
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Where(c => c.Situacao == SituacaoConta.Aberta)
            .ToList();
    }

    public List<Conta> SelecionarFechadas()
    {
        return RegistrosDoUsuario()
            .Include(c => c.Mesa)
            .Include(c => c.Garcom)
            .Where(c => c.Situacao == SituacaoConta.Fechada)
            .ToList();
    }
}
