using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloPedidoConta;

namespace ControleDeBar.Dominio.Modulos.ModuloConta;

public class Conta : EntidadeBase<Conta>, IEntidadeDoUsuario
{
    public Guid MesaId { get; set; }

    public Mesa Mesa { get; set; } = null!;

    public Guid GarcomId { get; set; }

    public Garcom Garcom { get; set; } = null!;

    public string NomeCliente { get; set; } = string.Empty;

    public DateTime DataAbertura { get; set; }

    public DateTime? DataFechamento { get; set; }

    public SituacaoConta Situacao { get; set; } = SituacaoConta.Aberta;

    public List<PedidoConta> Pedidos { get; set; } = [];

    public Guid UserId { get; set; }

    public decimal ValorTotal =>
        Pedidos.Sum(p => p.Subtotal);

    public Conta()
    {
    }

    public Conta(
        Mesa mesa,
        Garcom garcom,
        string nomeCliente
    ) : this()
    {
        if (mesa is not null)
        {
            Mesa = mesa;
            MesaId = mesa.Id;
        }

        if (garcom is not null)
        {
            Garcom = garcom;
            GarcomId = garcom.Id;
        }

        NomeCliente = nomeCliente;

        DataAbertura = DateTime.Now;

        Situacao = SituacaoConta.Aberta;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (
            string.IsNullOrWhiteSpace(NomeCliente) ||
            NomeCliente.Length < 2 ||
            NomeCliente.Length > 100
        )
        {
            erros.Add(
                "O campo \"Cliente\" deve conter entre 2 e 100 caracteres."
            );
        }

        if (Mesa is null)
            erros.Add("A mesa deve ser informada.");

        if (Garcom is null)
            erros.Add("O garçom deve ser informado.");

        return erros;
    }

    public override void Atualizar(Conta entidadeAtualizada)
    {
        Mesa = entidadeAtualizada.Mesa;
        MesaId = entidadeAtualizada.MesaId;

        Garcom = entidadeAtualizada.Garcom;
        GarcomId = entidadeAtualizada.GarcomId;

        NomeCliente = entidadeAtualizada.NomeCliente;
    }

    public void Fechar()
    {
        Situacao = SituacaoConta.Fechada;
        DataFechamento = DateTime.Now;
    }
}
