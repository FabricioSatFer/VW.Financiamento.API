using Financiamento.Application.DTOs;
using Financiamento.Application.Services;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;
using Moq;

namespace Financiamento.Tests
{
    public class PagamentosServicesTests
    {
        [Fact]
        public async Task RegistrarPagamento_RetornaPagamentoDtoComStatusEmDia()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 50000,
                TaxaMensal = 2.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            mockPagamentosRepo.Setup(r => r.Add(It.IsAny<Pagamento>())).Returns((Pagamento p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 4, 13)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal(contratoId, result.ContratoId);
            Assert.Equal(1, result.ParcelaNumero);
            Assert.True(result.ValorOriginalParcela > 0);
            Assert.Equal(0, result.ValorDesconto);
            Assert.Equal(0, result.ValorJuros);
            Assert.Equal(0, result.ValorMulta);
            Assert.Equal(result.ValorOriginalParcela, result.ValorPago);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataPagamento);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataVencimento);
            Assert.Equal((int)StatusPagamento.EmDia, result.Status);
            Assert.Equal(0, result.DiasAntecipacao);
            Assert.Equal(0, result.DiasAtraso);
        }

        [Fact]
        public async Task RegistrarPagamento_RetornaPagamentoDtoComStatusEmDia_DentroJanelaAntecipacao()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 50000,
                TaxaMensal = 2.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            mockPagamentosRepo.Setup(r => r.Add(It.IsAny<Pagamento>())).Returns((Pagamento p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 4, 8)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal((int)StatusPagamento.EmDia, result.Status);
            Assert.Equal(0, result.ValorDesconto);
            Assert.Equal(0, result.DiasAntecipacao);
        }

        [Fact]
        public async Task RegistrarPagamento_RetornaPagamentoDtoComStatusEmAtraso()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 50000,
                TaxaMensal = 2.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            mockPagamentosRepo.Setup(r => r.Add(It.IsAny<Pagamento>())).Returns((Pagamento p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 4, 23)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal(contratoId, result.ContratoId);
            Assert.Equal(1, result.ParcelaNumero);
            Assert.True(result.ValorMulta > 0);
            Assert.True(result.ValorJuros > 0);
            Assert.Equal(0, result.ValorDesconto);
            Assert.Equal(result.ValorOriginalParcela + result.ValorMulta + result.ValorJuros, result.ValorPago);
            Assert.Equal(new DateTime(2026, 4, 23).ToUniversalTime(), result.DataPagamento);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataVencimento);
            Assert.Equal((int)StatusPagamento.EmAtraso, result.Status);
            Assert.Equal(0, result.DiasAntecipacao);
            Assert.Equal(10, result.DiasAtraso);

            var multaEsperada = Math.Round(result.ValorOriginalParcela * 0.02m, 2);
            var jurosEsperados = Math.Round(result.ValorOriginalParcela * 0.001m * 10, 2);
            Assert.Equal(multaEsperada, result.ValorMulta);
            Assert.Equal(jurosEsperados, result.ValorJuros);
        }

        [Fact]
        public async Task RegistrarPagamento_RetornaPagamentoDtoComStatusAntecipado()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 50000,
                TaxaMensal = 2.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            mockPagamentosRepo.Setup(r => r.Add(It.IsAny<Pagamento>())).Returns((Pagamento p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 3, 26)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal(contratoId, result.ContratoId);
            Assert.Equal(1, result.ParcelaNumero);
            Assert.True(result.ValorDesconto > 0);
            Assert.Equal(0, result.ValorJuros);
            Assert.Equal(0, result.ValorMulta);
            Assert.Equal(result.ValorOriginalParcela - result.ValorDesconto, result.ValorPago);
            Assert.Equal(new DateTime(2026, 3, 26).ToUniversalTime(), result.DataPagamento);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataVencimento);
            Assert.Equal((int)StatusPagamento.Antecipado, result.Status);
            Assert.Equal(18, result.DiasAntecipacao);
            Assert.Equal(0, result.DiasAtraso);

            var descontoEsperado = Math.Round(result.ValorOriginalParcela * 0.005m, 2);
            Assert.Equal(descontoEsperado, result.ValorDesconto);
        }

        [Fact]
        public async Task RegistrarPagamento_LancaExceptionQuandoContratoNaoEncontrado()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync((Contrato?)null);
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 4, 13)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.RegistrarPagamento(contratoId, input));
        }

        [Fact]
        public async Task RegistrarPagamento_LancaExceptionQuandoParcelaInvalida()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 50000,
                TaxaMensal = 2.5m,
                PrazoMeses = 48,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 50,
                DataPagamento = new DateTime(2026, 4, 13)
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.RegistrarPagamento(contratoId, input));
        }

        [Fact]
        public async Task GetByContrato_RetornaListaDePagamentoDto()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            var pagamentos = new List<Pagamento>
            {
                new Pagamento
                {
                    Id = Guid.NewGuid(),
                    ContratoId = contratoId,
                    ParcelaNumero = 1,
                    ValorOriginalParcela = 1200,
                    ValorDesconto = 0,
                    ValorJuros = 0,
                    ValorMulta = 0,
                    ValorPago = 1200,
                    DataPagamento = new DateTime(2026, 4, 13),
                    DataVencimento = new DateTime(2026, 4, 13),
                    Status = (int)StatusPagamento.EmDia,
                    DiasAntecipacao = 0,
                    DiasAtraso = 0
                },
                new Pagamento
                {
                    Id = Guid.NewGuid(),
                    ContratoId = contratoId,
                    ParcelaNumero = 2,
                    ValorOriginalParcela = 1200,
                    ValorDesconto = 0,
                    ValorJuros = 0,
                    ValorMulta = 0,
                    ValorPago = 1200,
                    DataPagamento = new DateTime(2026, 5, 13),
                    DataVencimento = new DateTime(2026, 5, 13),
                    Status = (int)StatusPagamento.EmDia,
                    DiasAntecipacao = 0,
                    DiasAtraso = 0
                }
            };
            mockPagamentosRepo.Setup(r => r.GetByContrato(contratoId)).Returns(pagamentos);
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);

            var result = await service.GetByContrato(contratoId);

            Assert.NotNull(result);
            Assert.Equal(2, System.Linq.Enumerable.Count(result));
        }

        [Fact]
        public async Task CalcularValorParcela_CalculaCorretamenteComTaxa()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 50000,
                TaxaMensal = 2m,
                PrazoMeses = 12,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            mockPagamentosRepo.Setup(r => r.Add(It.IsAny<Pagamento>())).Returns((Pagamento p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 4, 13)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.True(result.ValorOriginalParcela > 4000);
            Assert.True(result.ValorOriginalParcela < 5000);
        }

        [Fact]
        public async Task CalcularValorParcela_CalculaCorretamenteSemTaxa()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
                ValorTotal = 12000,
                TaxaMensal = 0,
                PrazoMeses = 12,
                DataVencimentoPrimeiraParcela = new DateTime(2026, 4, 13)
            });
            var mockPagamentosRepo = new Mock<IPagamentosRepository>();
            mockPagamentosRepo.Setup(r => r.Add(It.IsAny<Pagamento>())).Returns((Pagamento p) =>
            {
                p.Id = Guid.NewGuid();
                return p;
            });
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);
            var input = new PagamentoCreateDto
            {
                ParcelaNumero = 1,
                DataPagamento = new DateTime(2026, 4, 13)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.Equal(1000, result.ValorOriginalParcela);
        }
    }
}
