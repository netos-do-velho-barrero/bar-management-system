using System.ComponentModel.DataAnnotations;

namespace ControleDeBar.WebApp.Modulos.ModuloPedidoConta;

// Não existe uma "tela de listagem" própria para PedidoConta — os pedidos são
// exibidos e gerenciados dentro da tela de Detalhes da Conta (ver ContaController).
// Por isso os ViewModels aqui carregam ContaId, usado só para redirecionar de
// volta para Conta/Detalhes depois de cada ação (Adicionar/Editar/Remover).

public record AdicionarPedidoContaViewModel
{
    [Required(ErrorMessage = "A conta é obrigatória.")]
    public Guid ContaId { get; init; }

    [Required(ErrorMessage = "Selecione um produto.")]
    public Guid ProdutoId { get; init; }

    [Required(ErrorMessage = "O campo \"Quantidade\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Quantidade\" deve ser um número positivo.")]
    public int Quantidade { get; init; }
}

public record EditarQuantidadePedidoContaViewModel
{
    public Guid Id { get; init; }

    [Required(ErrorMessage = "A conta é obrigatória.")]
    public Guid ContaId { get; init; }

    [Required(ErrorMessage = "O campo \"Quantidade\" deve ser preenchido.")]
    [Range(1, int.MaxValue, ErrorMessage = "O campo \"Quantidade\" deve ser um número positivo.")]
    public int Quantidade { get; init; }
}

public record ListarPedidoContaViewModel
{
    public Guid Id { get; init; }
    public string NomeProduto { get; init; } = string.Empty;
    public int Quantidade { get; init; }
    public decimal PrecoUnitario { get; init; }
    public decimal Subtotal { get; init; }
}

public record OpcaoProdutoPedidoViewModel
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public decimal PrecoVenda { get; init; }
}
