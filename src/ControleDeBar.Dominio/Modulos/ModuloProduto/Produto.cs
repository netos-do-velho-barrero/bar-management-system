using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;

namespace ControleDeBar.Dominio.Modulos.ModuloProduto;

public class Produto : EntidadeBase<Produto>, IEntidadeDoUsuario
{
    public string Nome { get; set; } = string.Empty;
    public decimal PrecoVenda { get; set; }

    public List<PedidoConta> Pedidos { get; set; } = [];

    public Guid UserId { get; set; }

    public Produto()
    {
    }

    public Produto(string nome, decimal precoVenda) : this()
    {
        Nome = nome;
        PrecoVenda = precoVenda;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(Nome) || Nome.Length < 2 || Nome.Length > 100)
            erros.Add("O campo \"Nome\" deve conter entre 2 e 100 caracteres.");

        if (PrecoVenda <= 0)
            erros.Add("O campo \"Preço de venda\" deve ser maior que zero.");

        return erros;
    }

    public override void Atualizar(Produto entidadeAtualizada)
    {
        Nome = entidadeAtualizada.Nome;
        PrecoVenda = entidadeAtualizada.PrecoVenda;
    }
}
