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
                ValorPago = 1000,
                DataPagamento = new DateTime(2026, 4, 13)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal(contratoId, result.ContratoId);
            Assert.Equal(1, result.ParcelaNumero);
            Assert.Equal(1000, result.ValorPago);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataPagamento);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataVencimento);
            Assert.Equal((int)StatusPagamento.EmDia, result.Status);
        }

        [Fact]
        public async Task RegistrarPagamento_RetornaPagamentoDtoComStatusEmAtraso()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
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
                ValorPago = 1000,
                DataPagamento = new DateTime(2026, 4, 14)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal(contratoId, result.ContratoId);
            Assert.Equal(1, result.ParcelaNumero);
            Assert.Equal(1000, result.ValorPago);
            Assert.Equal(new DateTime(2026, 4, 14).ToUniversalTime(), result.DataPagamento);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataVencimento);
            Assert.Equal((int)StatusPagamento.EmAtraso, result.Status);
        }

        [Fact]
        public async Task RegistrarPagamento_RetornaPagamentoDtoComStatusAntecipado()
        {
            var contratoId = Guid.NewGuid();
            var mockContratosRepo = new Mock<IContratosRepository>();
            mockContratosRepo.Setup(r => r.Get(contratoId)).ReturnsAsync(new Contrato
            {
                Id = contratoId,
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
                ValorPago = 1000,
                DataPagamento = new DateTime(2026, 3, 26)
            };

            var result = await service.RegistrarPagamento(contratoId, input);

            Assert.NotNull(result);
            Assert.Equal(contratoId, result.ContratoId);
            Assert.Equal(1, result.ParcelaNumero);
            Assert.Equal(1000, result.ValorPago);
            Assert.Equal(new DateTime(2026, 3, 26).ToUniversalTime(), result.DataPagamento);
            Assert.Equal(new DateTime(2026, 4, 13).ToUniversalTime(), result.DataVencimento);
            Assert.Equal((int)StatusPagamento.Antecipado, result.Status);

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
                ValorPago = 1000,
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
                    ValorPago = 1000,
                    DataPagamento = new DateTime(2026, 4, 13),
                    DataVencimento = new DateTime(2026, 4, 13),
                    Status = (int)StatusPagamento.EmDia
                },
                new Pagamento
                {
                    Id = Guid.NewGuid(),
                    ContratoId = contratoId,
                    ParcelaNumero = 2,
                    ValorPago = 1000,
                    DataPagamento = new DateTime(2026, 4, 14),
                    DataVencimento = new DateTime(2026, 4, 14),
                    Status = (int)StatusPagamento.EmDia
                }
            };
            mockPagamentosRepo.Setup(r => r.GetByContrato(contratoId)).Returns(pagamentos);
            var service = new PagamentosServices(mockPagamentosRepo.Object, mockContratosRepo.Object);

            var result = await service.GetByContrato(contratoId);

            Assert.NotNull(result);
            Assert.Equal(2, System.Linq.Enumerable.Count(result));
        }
    }
}
