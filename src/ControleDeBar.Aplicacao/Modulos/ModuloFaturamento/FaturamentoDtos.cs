namespace ControleDeBar.Aplicacao.Modulos.ModuloFaturamento;

public record FaturamentoDto(
    DateTime Data,
    decimal ValorTotal,
    int QuantidadeContas,
    List<ItemFaturamentoContaDto> ContasFechadas
);

public record ItemFaturamentoContaDto(
    Guid Id,
    int NumeroMesa,
    string NomeGarcom,
    string NomeCliente,
    DateTime DataAbertura,
    DateTime? DataFechamento,
    decimal ValorTotal
);
