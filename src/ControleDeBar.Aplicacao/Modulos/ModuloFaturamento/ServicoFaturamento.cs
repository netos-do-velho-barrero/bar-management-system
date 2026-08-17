using ControleDeBar.Dominio.Modulos.ModuloFaturamento;

namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public class ServicoFaturamento(
    IRepositorioFaturamento repositorioFaturamento
)
{
    public FaturamentoDto ObterFaturamentoDiario(DateTime data)
    {
        Faturamento faturamento = repositorioFaturamento.ObterFaturamentoDiario(data);

        return new FaturamentoDto(
            faturamento.Data,
            faturamento.ValorTotal,
            faturamento.QuantidadeContasFechadas
        );
    }
}
