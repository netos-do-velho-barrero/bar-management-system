using ControleDeBar.Dominio.Compartilhado;
using ControleDeBar.Dominio.Compartilhado.Identity;

namespace ControleDeBar.Dominio.Modulos.ModuloMesa;

public class Mesa : EntidadeBase<Mesa>, IEntidadeDoUsuario
{
    public int Numero { get; set; }
    public int QuantidadeLugares { get; set; }
    public StatusMesa Status { get; set; } = StatusMesa.Livre;
    public Guid UserId { get; set; }

    public Mesa()
    {
    }

    public Mesa(int numero, int quantidadeLugares)
    {
        Numero = numero;
        QuantidadeLugares = quantidadeLugares;
    }

    public override List<string> Validar()
    {
        List<string> erros = [];

        if (Numero <= 0)
            erros.Add("O campo \"Número da mesa\" deve ser preenchido com um valor positivo.");

        if (QuantidadeLugares <= 0)
            erros.Add("O campo \"Quantidade de lugares\" deve ser um número positivo.");

        return erros;
    }

    public override void Atualizar(Mesa entidadeAtualizada)
    {
        Numero = entidadeAtualizada.Numero;
        QuantidadeLugares = entidadeAtualizada.QuantidadeLugares;
    }
}

