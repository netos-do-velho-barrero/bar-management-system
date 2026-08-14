using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloMesa;

public record CadastrarMesaViewModel
{
    [Required(ErrorMessage = "O campo \"Número da mesa\" deve ser preenchido.")]
    public int Numero { get; init; }

    [Required(ErrorMessage = "O campo \"Quantidade de lugares\" deve ser preenchido.")]
    public int QuantidadeLugares { get; init; }
}

public record EditarMesaViewModel
{
    public Guid Id { get; init; }

    [Required(ErrorMessage = "O campo \"Número da mesa\" deve ser preenchido.")]
    public int Numero { get; init; }

    [Required(ErrorMessage = "O campo \"Quantidade de lugares\" deve ser preenchido.")]
    public int QuantidadeLugares { get; init; }
}

public record ListarMesaViewModel
{
    public Guid Id { get; init; }
    public int Numero { get; init; }
    public int QuantidadeLugares { get; init; }
    public string Status { get; init; } = string.Empty;
}

public record DetalhesMesaViewModel
{
    public Guid Id { get; init; }
    public int Numero { get; init; }
    public int QuantidadeLugares { get; init; }
    public string Status { get; init; } = string.Empty;
}

