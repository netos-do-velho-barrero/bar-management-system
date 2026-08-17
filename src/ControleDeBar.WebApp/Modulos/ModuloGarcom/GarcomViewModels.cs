using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloGarcom;

public record CadastrarGarcomViewModel
{
    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres."
    )]
    public string Nome { get; init; } = string.Empty;
}

public record EditarGarcomViewModel
{
    public Guid Id { get; init; }

    [Required(ErrorMessage = "O campo \"Nome\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome\" deve conter entre 2 e 100 caracteres."
    )]
    public string Nome { get; init; } = string.Empty;
}

public record ListarGarcomViewModel
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;
}

public record DetalhesGarcomViewModel
{
    public Guid Id { get; init; }

    public string Nome { get; init; } = string.Empty;
}
