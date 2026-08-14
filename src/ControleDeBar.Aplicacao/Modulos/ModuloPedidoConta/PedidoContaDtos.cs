namespace ControleDeBar.Aplicacao.Modulos.ModuloPedidoConta;

public record ListarPedidoContaDto(
    Guid Id,
    string NomeProduto,
    int Quantidade,
    decimal PrecoUnitario,
    decimal Subtotal
);

public record AdicionarPedidoContaDto(
    Guid ContaId,
    Guid ProdutoId,
    int Quantidade
);

public record EditarQuantidadePedidoContaDto(
    Guid Id,
    int Quantidade
);

public record OpcaoProdutoPedidoDto(
    Guid Id,
    string Nome,
    decimal PrecoVenda
);
 