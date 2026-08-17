namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public record FaturamentoDto(
    DateTime Data,
    decimal TotalFaturado,
    int QuantidadeContas
);
