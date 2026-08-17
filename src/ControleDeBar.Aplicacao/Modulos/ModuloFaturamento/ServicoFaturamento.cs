using ControleDeBar.Dominio.Modulos.ModuloConta;

namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public class ServicoFaturamento(IRepositorioConta repositorioConta)
{
    public FaturamentoDto ObterFaturamentoDiario(DateTime data)
    {
        var contasFechadasDoDia = repositorioConta.SelecionarTodos()
            .Where(c => c.Situacao == SituacaoConta.Fechada &&
                        c.DataFechamento.HasValue &&
                        c.DataFechamento.Value.Date == data.Date)
            .ToList();

        var itens = contasFechadasDoDia.Select(c => new ItemFaturamentoContaDto(
    c.Id,
    c.Mesa?.Numero ?? 0,
    c.Garcom?.Nome ?? "Não informado",
    string.IsNullOrWhiteSpace(c.NomeCliente) ? "Não informado" : c.NomeCliente,
    c.DataAbertura,
    c.DataFechamento,
    c.ValorTotal
)).ToList();

        return new FaturamentoDto(
            data,
            contasFechadasDoDia.Sum(c => c.ValorTotal),
            contasFechadasDoDia.Count,
            itens
        );
    }
}
