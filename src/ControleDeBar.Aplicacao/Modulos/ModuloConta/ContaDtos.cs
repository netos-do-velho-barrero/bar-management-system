namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public record ListarContaDto(
    Guid Id,
    int NumeroMesa,
    string NomeGarcom,
    string NomeCliente,
    DateTime DataAbertura,
    string Situacao,
    decimal ValorTotal
);

public record AbrirContaDto(
    Guid MesaId,
    Guid GarcomId,
    string NomeCliente
);

public record EditarContaDto(
    Guid Id,
    Guid MesaId,
    Guid GarcomId,
    string NomeCliente
);

public record ItemPedidoDaContaDto(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);

public record DetalhesContaDto(
    Guid Id,
    Guid MesaId,
    int NumeroMesa,
    Guid GarcomId,
    string NomeGarcom,
    string NomeCliente,
    DateTime DataAbertura,
    string Situacao,
    decimal ValorTotal,
    List<ItemPedidoDaContaDto> Pedidos
);

public record OpcaoMesaContaDto(
    Guid Id,
    int Numero
);

public record OpcaoGarcomContaDto(
    Guid Id,
    string Nome
);
