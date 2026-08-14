namespace ControleDeBar.Aplicacao.Modulos.ModuloProduto;

public record ListarProdutoDto(
    Guid Id,
    string Nome,
    decimal PrecoVenda
);

public record CadastrarProdutoDto(
    string Nome,
    decimal PrecoVenda
);

public record EditarProdutoDto(
    Guid Id,
    string Nome,
    decimal PrecoVenda
);

public record DetalhesProdutoDto(
    Guid Id,
    string Nome,
    decimal PrecoVenda
);
 