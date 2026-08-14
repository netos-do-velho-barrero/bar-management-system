using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloProduto;

namespace ControleDeBar.Dominio.Modulos.ModuloPedidoConta;

public class PedidoConta : EntidadeBase<PedidoConta>, IEntidadeDoUsuario
{
    public Produto Produto { get; set; } = null!;
    public int Quantidade { get; set; }


    public decimal PrecoUnitario { get; set; }

    public Conta Conta { get; set; } = null!;

    public Guid UserId { get; set; }


    public decimal Subtotal => PrecoUnitario * Quantidade;

    public PedidoConta()
    {
    }

    public PedidoConta(Produto produto, int quantidade) : this()
    {
        Produto = produto;
        Quantidade = quantidade;
        PrecoUnitario = produto.PrecoVenda;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (Produto is null)
            erros.Add("O campo \"Produto\" deve ser preenchido.");

        if (Quantidade <= 0)
            erros.Add("O campo \"Quantidade\" deve ser um número positivo.");

        return erros;
    }

    public override void Atualizar(PedidoConta entidadeAtualizada)
    {
        Quantidade = entidadeAtualizada.Quantidade;
    }
}
