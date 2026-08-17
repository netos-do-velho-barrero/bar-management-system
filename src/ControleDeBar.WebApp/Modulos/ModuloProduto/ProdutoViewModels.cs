using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloProduto;

public record CadastrarProdutoViewModel
{
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres."
    )]
    public string Nome { get; init; } = string.Empty;

    [Required(ErrorMessage = "O campo \"Preço de venda\" deve ser preenchido.")]
    [Range(
        0.01,
        double.MaxValue,
        ErrorMessage = "O campo \"Preço de venda\" deve ser maior que zero."
    )]
    public decimal PrecoVenda { get; init; }
}

public record EditarProdutoViewModel
{
    public Guid Id { get; init; }

    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres."
    )]
    public string Nome { get; init; } = string.Empty;

    [Required(ErrorMessage = "O campo \"Preço de venda\" deve ser preenchido.")]
    [Range(
        0.01,
        double.MaxValue,
        ErrorMessage = "O campo \"Preço de venda\" deve ser maior que zero."
    )]
    public decimal PrecoVenda { get; init; }
}

public record ListarProdutoViewModel
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public decimal PrecoVenda { get; init; }
}

public record DetalhesProdutoViewModel
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public decimal PrecoVenda { get; init; }
}
