using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloFaturamento;
using ControleDeBar.Infra.Compartilhado.Orm;
using Microsoft.EntityFrameworkCore;

namespace ControleDeBar.Infra.Modulos.ModuloFaturamento;

public sealed class RepositorioFaturamentoEmOrm(
    ControleDeBarDbContext dbContext,
    IProvedorDeUsuario provedorDeUsuario
) : RepositorioBaseEmOrm<Conta>(
    dbContext,
    provedorDeUsuario
), IRepositorioFaturamento
{
    public Faturamento ObterFaturamentoDiario(DateTime data)
{
    List<Conta> contasFechadas = RegistrosDoUsuario()
        .Include(c => c.Pedidos)
            .ThenInclude(p => p.Produto)
        .Where(c => c.Situacao == SituacaoConta.Fechada
                 && c.DataAbertura.Date == data.Date)
        .ToList();

    decimal totalFaturado = contasFechadas.Sum(c => c.ValorTotal);

    // Passe contasFechadas.Count no 3º argumento (int)
    return new Faturamento(data, totalFaturado, contasFechadas.Count);
}
}
