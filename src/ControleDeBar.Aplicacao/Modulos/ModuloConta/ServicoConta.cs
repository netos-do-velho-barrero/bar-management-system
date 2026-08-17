using FluentResults;
using ControleDeBar.Aplicacao.Compartilhado;
using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Compartilhado.Identity;
using Microsoft.Extensions.Logging;

namespace ControleDeBar.Aplicacao.Modulos.ModuloConta;

public class ServicoConta(
    IRepositorioConta repositorioConta,
    IRepositorioMesa repositorioMesa,
    IRepositorioGarcom repositorioGarcom,
    IProvedorDeUsuario provedorDeUsuario,
    ILogger<ServicoConta>? logger = null
) : ServicoBase<Conta>
{
    public Result Abrir(AbrirContaDto dto)
    {
        Mesa? mesaSemFiltro = repositorioMesa.SelecionarPorIdSemFiltro(dto.MesaId);

        if (mesaSemFiltro is not null && mesaSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Mesa),
                provedorDeUsuario.Id,
                mesaSemFiltro.UserId,
                nameof(dto.MesaId)
            );

            return Falha(
                nameof(dto.MesaId),
                "Esta mesa não pertence ao seu bar."
            );
        }

        Result<Mesa> resultadoMesa = SelecionarMesa(dto.MesaId);

        if (resultadoMesa.IsFailed)
            return resultadoMesa.ToResult();

        Result<Garcom> resultadoGarcom = SelecionarGarcom(dto.GarcomId);

        if (resultadoGarcom.IsFailed)
            return resultadoGarcom.ToResult();

        Mesa mesa = resultadoMesa.Value;
        Garcom garcom = resultadoGarcom.Value;

        // Validação Multi-Tenancy: Mesa pertence ao mesmo bar?
        if (mesa.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Mesa),
                provedorDeUsuario.Id,
                mesa.UserId,
                nameof(dto.MesaId)
            );

            return Falha(
                nameof(dto.MesaId),
                "Esta mesa não pertence ao seu bar."
            );
        }

        // Validação Multi-Tenancy: Garçom pertence ao mesmo bar?
        if (garcom.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Garcom),
                provedorDeUsuario.Id,
                garcom.UserId,
                nameof(dto.GarcomId)
            );

            return Falha(
                nameof(dto.GarcomId),
                "Este garçom não pertence ao seu bar."
            );
        }

        if (mesa.Status == StatusMesa.Ocupada)
            return Falha(
                nameof(dto.MesaId),
                "Esta mesa já possui uma conta aberta."
            );

        Conta novaConta = new(
            mesa,
            garcom,
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
        Conta? contaExistenteSemFiltro =
            repositorioConta.SelecionarPorIdSemFiltro(dto.Id);

        if (contaExistenteSemFiltro is not null && contaExistenteSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Conta),
                provedorDeUsuario.Id,
                contaExistenteSemFiltro.UserId,
                nameof(dto.Id)
            );

            return Falha(
                string.Empty,
                "Esta conta não pertence ao seu bar."
            );
        }

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
        Garcom novoGarcom = resultadoGarcom.Value;

        // Validação Multi-Tenancy: Mesa pertence ao mesmo bar?
        if (novaMesa.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Mesa),
                provedorDeUsuario.Id,
                novaMesa.UserId,
                nameof(dto.MesaId)
            );

            return Falha(
                nameof(dto.MesaId),
                "Esta mesa não pertence ao seu bar."
            );
        }

        // Validação Multi-Tenancy: Garçom pertence ao mesmo bar?
        if (novoGarcom.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Garcom),
                provedorDeUsuario.Id,
                novoGarcom.UserId,
                nameof(dto.GarcomId)
            );

            return Falha(
                nameof(dto.GarcomId),
                "Este garçom não pertence ao seu bar."
            );
        }

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
            novoGarcom,
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
        Conta? contaSemFiltro =
            repositorioConta.SelecionarPorIdSemFiltro(id);

        if (contaSemFiltro is not null && contaSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Conta),
                provedorDeUsuario.Id,
                contaSemFiltro.UserId,
                nameof(id)
            );

            return Falha(
                string.Empty,
                "Esta conta não pertence ao seu bar."
            );
        }

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

        conta.Fechar();

        repositorioConta.Editar(id, conta);

        repositorioMesa.AlterarStatus(
            conta.MesaId,
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
        Conta? contaSemFiltro =
            repositorioConta.SelecionarPorIdSemFiltro(id);

        if (contaSemFiltro is not null && contaSemFiltro.UserId != provedorDeUsuario.Id)
        {
            logger?.LogWarning(
                "Tentativa de acesso a dados de outro bar. Entidade: {Entidade} | Usuario: {UsuarioAtual} | UserIdEntidade: {UserIdEntidade} | Campo: {Campo}",
                nameof(Conta),
                provedorDeUsuario.Id,
                contaSemFiltro.UserId,
                nameof(id)
            );

            return Result.Fail(
                "Esta conta não pertence ao seu bar."
            );
        }

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
