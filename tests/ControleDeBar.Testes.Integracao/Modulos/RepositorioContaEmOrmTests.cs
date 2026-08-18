using ControleDeBar.Dominio.Modulos.ModuloConta;
using ControleDeBar.Dominio.Modulos.ModuloGarcom;
using ControleDeBar.Dominio.Modulos.ModuloMesa;
using ControleDeBar.Testes.Integracao.Compartilhado.Orm;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ControleDeBar.Testes.Integracao.Modulos.ModuloConta;

[TestClass]
public sealed class RepositorioContaEmOrmTests : RepositorioBaseEmOrmTests
{
    #region --- CENÁRIOS POSITIVOS ---

    [TestMethod]
    public void Editar_NomeDoClienteEmContaAberta_DeveAtualizarComSucesso()
    {
        Mesa mesa = new Mesa(1, 4) { UserId = userId };
        Garcom garcom = new Garcom("Carlos") { UserId = userId };
        Conta conta = new Conta(mesa, garcom, "Cliente Inicial") { UserId = userId, Situacao = SituacaoConta.Aberta };

        dbContext.Set<Mesa>().Add(mesa);
        dbContext.Set<Garcom>().Add(garcom);
        repositorioConta.Cadastrar(conta);
        dbContext.SaveChanges();

        conta.NomeCliente = "Cliente Atualizado";
        repositorioConta.Editar(conta.Id, conta);
        dbContext.SaveChanges();
        dbContext.ChangeTracker.Clear();

        Conta? contaBuscada = repositorioConta.SelecionarPorId(conta.Id);

        Assert.IsNotNull(contaBuscada);
        Assert.AreEqual("Cliente Atualizado", contaBuscada.NomeCliente);
    }

    [TestMethod]
    public void SelecionarContasAbertas_DeveRetornarApenasAbertas()
    {
        Mesa m1 = new Mesa(1, 4) { UserId = userId };
        Mesa m2 = new Mesa(2, 2) { UserId = userId };
        Garcom g = new Garcom("Carlos") { UserId = userId };

        Conta c1 = new Conta(m1, g, "Cliente 1") { UserId = userId, Situacao = SituacaoConta.Aberta };
        Conta c2 = new Conta(m2, g, "Cliente 2") { UserId = userId, Situacao = SituacaoConta.Fechada };

        dbContext.Set<Mesa>().AddRange(m1, m2);
        dbContext.Set<Garcom>().Add(g);
        repositorioConta.Cadastrar(c1);
        repositorioConta.Cadastrar(c2);
        dbContext.SaveChanges();

        List<Conta> abertas = repositorioConta.SelecionarAbertas();

        Assert.AreEqual(1, abertas.Count);
        Assert.AreEqual("Cliente 1", abertas.First().NomeCliente);
    }

    [TestMethod]
    public void SelecionarContasFechadas_DeveRetornarApenasHistoricoFechado()
    {
        Mesa m1 = new Mesa(1, 4) { UserId = userId };
        Mesa m2 = new Mesa(2, 2) { UserId = userId };
        Garcom g = new Garcom("Carlos") { UserId = userId };

        Conta c1 = new Conta(m1, g, "Cliente 1") { UserId = userId, Situacao = SituacaoConta.Aberta };
        Conta c2 = new Conta(m2, g, "Cliente 2") { UserId = userId, Situacao = SituacaoConta.Fechada };

        dbContext.Set<Mesa>().AddRange(m1, m2);
        dbContext.Set<Garcom>().Add(g);
        repositorioConta.Cadastrar(c1);
        repositorioConta.Cadastrar(c2);
        dbContext.SaveChanges();

        List<Conta> fechadas = repositorioConta.SelecionarFechadas();

        Assert.AreEqual(1, fechadas.Count);
        Assert.AreEqual("Cliente 2", fechadas.First().NomeCliente);
    }

    #endregion
}
