using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloFaturamento;

namespace ControleDeBar.Infra.Modulos.ModuloFaturamento;


public sealed class RepositorioFaturamentoEmOrm(
    IRepositorioConta repositorioConta
) : IRepositorioFaturamento
{
    public Faturamento ObterFaturamentoDiario(DateTime data)
    {
        List<Conta> contasFechadasDoDia = repositorioConta
            .SelecionarTodos()
            .Where(c =>
                c.Situacao == SituacaoConta.Fechada &&
                c.DataAbertura.Date == data.Date
            )
            .ToList();

        return Faturamento.Calcular(data, contasFechadasDoDia);
    }
}
