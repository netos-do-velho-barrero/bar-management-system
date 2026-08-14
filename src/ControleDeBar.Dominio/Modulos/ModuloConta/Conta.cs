using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public class Conta : EntidadeBase<Conta>, IEntidadeDoUsuario
{
    public Mesa Mesa { get; set; } = null!;
    public Garcom Garcom { get; set; } = null!;
    public string NomeCliente { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public SituacaoConta Situacao { get; set; } = SituacaoConta.Aberta;

    public List<PedidoConta> Pedidos { get; set; } = [];

    public Guid UserId { get; set; }

    // Calculado automaticamente a partir dos pedidos vinculados — não é persistido.
    public decimal ValorTotal => Pedidos.Sum(p => p.Subtotal);

    public Conta()
    {
    }

    public Conta(Mesa mesa, Garcom garcom, string nomeCliente) : this()
    {
        Mesa = mesa;
        Garcom = garcom;
        NomeCliente = nomeCliente;
        DataAbertura = DateTime.Now;
        Situacao = SituacaoConta.Aberta;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (string.IsNullOrWhiteSpace(NomeCliente) || NomeCliente.Length < 2 || NomeCliente.Length > 100)
            erros.Add("O campo \"Nome do cliente\" deve conter entre 2 e 100 caracteres.");

        if (Mesa is null)
            erros.Add("O campo \"Mesa\" deve ser preenchido.");

        if (Garcom is null)
            erros.Add("O campo \"Garçom\" deve ser preenchido.");

        return erros;
    }

    // Edição de conta só é permitida enquanto ela estiver aberta (regra validada no serviço).
    public override void Atualizar(Conta entidadeAtualizada)
    {
        Mesa = entidadeAtualizada.Mesa;
        Garcom = entidadeAtualizada.Garcom;
        NomeCliente = entidadeAtualizada.NomeCliente;
    }

    public void Fechar()
    {
        Situacao = SituacaoConta.Fechada;
    }
}
