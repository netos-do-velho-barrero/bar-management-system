using FluentResults;
using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public class ServicoConta(
    IRepositorioConta repositorioConta,
    IRepositorioMesa repositorioMesa,
    IRepositorioGarcom repositorioGarcom
) : ServicoBase<Conta>
{
    public Result Abrir(AbrirContaDto dto)
    {
        Result<Mesa> resultadoMesa = SelecionarMesa(dto.MesaId);

        if (resultadoMesa.IsFailed)
            return resultadoMesa.ToResult();

        Result<Garcom> resultadoGarcom = SelecionarGarcom(dto.GarcomId);

        if (resultadoGarcom.IsFailed)
            return resultadoGarcom.ToResult();

        Mesa mesa = resultadoMesa.Value;

        if (mesa.Status == StatusMesa.Ocupada)
            return Falha(
                nameof(dto.MesaId),
                "Esta mesa já possui uma conta aberta."
            );

        Conta novaConta = new(
            mesa,
            resultadoGarcom.Value,
            dto.NomeCliente
        );

        Result resultadoValidacao = ValidarEntidade(novaConta);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        repositorioConta.Cadastrar(novaConta);

        repositorioMesa.AlterarStatus(
            mesa.Id,
            StatusMesa.Ocupada
        );

        return Result.Ok();
    }

    public Result Editar(EditarContaDto dto)
    {
        Conta? contaExistente =
            repositorioConta.SelecionarPorId(dto.Id);

        if (contaExistente == null)
            return Falha(
                string.Empty,
                "Conta não encontrada."
            );

        if (contaExistente.Situacao == SituacaoConta.Fechada)
            return Falha(
                string.Empty,
                "Não é possível editar uma conta fechada."
            );

        Result<Mesa> resultadoMesa =
            SelecionarMesa(dto.MesaId);

        if (resultadoMesa.IsFailed)
            return resultadoMesa.ToResult();

        Result<Garcom> resultadoGarcom =
            SelecionarGarcom(dto.GarcomId);

        if (resultadoGarcom.IsFailed)
            return resultadoGarcom.ToResult();

        Mesa novaMesa = resultadoMesa.Value;

        if (
            novaMesa.Status == StatusMesa.Ocupada &&
            novaMesa.Id != contaExistente.MesaId
        )
        {
            return Falha(
                nameof(dto.MesaId),
                "Esta mesa já possui uma conta aberta."
            );
        }

        Conta contaAtualizada = new(
            novaMesa,
            resultadoGarcom.Value,
            dto.NomeCliente
        );

        Result resultadoValidacao =
            ValidarEntidade(contaAtualizada);

        if (resultadoValidacao.IsFailed)
            return resultadoValidacao;

        bool conseguiuEditar =
            repositorioConta.Editar(
                dto.Id,
                contaAtualizada
            );

        if (!conseguiuEditar)
            return Falha(
                string.Empty,
                "Conta não encontrada."
            );

        if (contaExistente.MesaId != novaMesa.Id)
        {
            repositorioMesa.AlterarStatus(
                contaExistente.MesaId,
                StatusMesa.Livre
            );

            repositorioMesa.AlterarStatus(
                novaMesa.Id,
                StatusMesa.Ocupada
            );
        }

        return Result.Ok();
    }

    public Result Fechar(Guid id)
    {
        Conta? conta =
            repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Falha(
                string.Empty,
                "Conta não encontrada."
            );

        if (conta.Situacao == SituacaoConta.Fechada)
            return Falha(
                string.Empty,
                "Esta conta já está fechada."
            );

        repositorioConta.AlterarSituacao(
            id,
            SituacaoConta.Fechada
        );

        repositorioMesa.AlterarStatus(
            conta.Mesa.Id,
            StatusMesa.Livre
        );

        return Result.Ok();
    }

    public List<ListarContaDto> SelecionarTodos()
    {
        return repositorioConta
            .SelecionarTodos()
            .Select(c => new ListarContaDto(
                c.Id,
                c.Mesa.Numero,
                c.Garcom.Nome,
                c.NomeCliente,
                c.DataAbertura,
                c.Situacao.ToString(),
                c.ValorTotal
            ))
            .ToList();
    }

    public Result<DetalhesContaDto> SelecionarPorId(Guid id)
    {
        Conta? conta =
            repositorioConta.SelecionarPorId(id);

        if (conta == null)
            return Result.Fail(
                "Conta não encontrada."
            );

        List<ItemPedidoDaContaDto> itens =
            conta.Pedidos
                .Select(p => new ItemPedidoDaContaDto(
                    p.Id,
                    p.Produto.Nome,
                    p.Quantidade,
                    p.PrecoUnitario,
                    p.Subtotal
                ))
                .ToList();

        return Result.Ok(
            new DetalhesContaDto(
                conta.Id,
                conta.Mesa.Id,
                conta.Mesa.Numero,
                conta.Garcom.Id,
                conta.Garcom.Nome,
                conta.NomeCliente,
                conta.DataAbertura,
                conta.Situacao.ToString(),
                conta.ValorTotal,
                itens
            )
        );
    }

    public List<OpcaoMesaContaDto> SelecionarMesasDisponiveis(
    Guid? contaId = null
)
    {
        Guid? mesaDaContaId = null;

        if (contaId.HasValue)
        {
            Conta? conta =
                repositorioConta.SelecionarPorId(contaId.Value);

            mesaDaContaId = conta?.MesaId;
        }

        return repositorioMesa
            .SelecionarTodos()
            .Where(m =>
                m.Status == StatusMesa.Livre ||
                m.Id == mesaDaContaId
            )
            .Select(m => new OpcaoMesaContaDto(
                m.Id,
                m.Numero
            ))
            .ToList();
    }

    public List<OpcaoGarcomContaDto> SelecionarGarcons()
    {
        return repositorioGarcom
            .SelecionarTodos()
            .Select(g => new OpcaoGarcomContaDto(
                g.Id,
                g.Nome
            ))
            .ToList();
    }

    private Result<Mesa> SelecionarMesa(Guid mesaId)
    {
        Mesa? mesa =
            repositorioMesa.SelecionarPorId(mesaId);

        if (mesa == null)
        {
            return Result.Fail<Mesa>(
                new Error("Selecione uma mesa válida.")
                    .WithMetadata(
                        "Campo",
                        nameof(AbrirContaDto.MesaId)
                    )
            );
        }

        return Result.Ok(mesa);
    }

    private Result<Garcom> SelecionarGarcom(Guid garcomId)
    {
        Garcom? garcom =
            repositorioGarcom.SelecionarPorId(garcomId);

        if (garcom == null)
        {
            return Result.Fail<Garcom>(
                new Error("Selecione um garçom válido.")
                    .WithMetadata(
                        "Campo",
                        nameof(AbrirContaDto.GarcomId)
                    )
            );
        }

        return Result.Ok(garcom);
    }
}
