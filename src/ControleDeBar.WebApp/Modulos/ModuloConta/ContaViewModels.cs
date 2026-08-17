using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloConta;

public record AbrirContaViewModel
{
    [Required(ErrorMessage = "Selecione uma mesa.")]
    public Guid MesaId { get; init; }

    [Required(ErrorMessage = "Selecione um garçom.")]
    public Guid GarcomId { get; init; }

    [Required(ErrorMessage = "O campo \"Nome do cliente\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome do cliente\" deve conter entre 2 e 100 caracteres."
    )]
    public string NomeCliente { get; init; } = string.Empty;
}

public record EditarContaViewModel
{
    public Guid Id { get; init; }

    [Required(ErrorMessage = "Selecione uma mesa.")]
    public Guid MesaId { get; init; }

    [Required(ErrorMessage = "Selecione um garçom.")]
    public Guid GarcomId { get; init; }

    [Required(ErrorMessage = "O campo \"Nome do cliente\" deve ser preenchido.")]
    [StringLength(
        100,
        MinimumLength = 2,
        ErrorMessage = "O campo \"Nome do cliente\" deve conter entre 2 e 100 caracteres."
    )]
    public string NomeCliente { get; init; } = string.Empty;
}

public record ListarContaViewModel
{
    public Guid Id { get; init; }
    public int NumeroMesa { get; init; }
    public string NomeGarcom { get; init; } = string.Empty;
    public string NomeCliente { get; init; } = string.Empty;
    public DateTime DataAbertura { get; init; }
    public string Situacao { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }
}

public record DetalhesContaViewModel
{
    public Guid Id { get; init; }
    public Guid MesaId { get; init; }
    public int NumeroMesa { get; init; }
    public Guid GarcomId { get; init; }
    public string NomeGarcom { get; init; } = string.Empty;
    public string NomeCliente { get; init; } = string.Empty;
    public DateTime DataAbertura { get; init; }
    public string Situacao { get; init; } = string.Empty;
    public decimal ValorTotal { get; init; }

    public List<ItemPedidoDaContaViewModel> Pedidos { get; init; } = [];
}

public record ItemPedidoDaContaViewModel
{
    public Guid Id { get; init; }
    public string NomeProduto { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal Subtotal { get; init; }
}

public record OpcaoMesaContaViewModel
{
    public Guid Id { get; init; }
    public int Numero { get; init; }
}

public record OpcaoGarcomContaViewModel
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
}
