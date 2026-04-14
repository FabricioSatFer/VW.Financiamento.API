using Moq;
using Xunit;
using Financiamento.Application.Services;
using System;
using System.Threading.Tasks;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;

namespace Financiamento.Tests
{
    public class ClientesServicesTests
    {
        [Fact]
        public async Task GetResumoCliente_RetornaResumoCorreto()
        {
            var cpfCnpj = "217.473.860-06";
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.GetByCliente(cpfCnpj)).ReturnsAsync(new List<Contrato>
            {
                new Contrato
                {
                    Id = Guid.NewGuid(),
                    ClienteCpfCnpj = cpfCnpj,
                    PrazoMeses = 12,
                    Pagamentos = new List<Pagamento>
                    {
                        new Pagamento { Status = (int)StatusPagamento.EmDia },
                        new Pagamento { Status = (int)StatusPagamento.EmAtraso },
                        new Pagamento { Status = (int)StatusPagamento.EmDia }
                    }
                }
            });
            var service = new ClientesServices(mockRepo.Object);

            var result = await service.GetResumoCliente(cpfCnpj);

            Assert.NotNull(result);
            Assert.Equal(cpfCnpj, result.ClienteCpfCnpj);
            Assert.Equal(1, result.QuantidadeContratosAtivos);
            Assert.Equal(12, result.TotalParcelas);
            Assert.Equal(3, result.ParcelasPagas);
            Assert.Equal(1, result.ParcelasEmAtraso);
            Assert.Equal(9, result.ParcelasAVencer);
            Assert.Equal(25m, Math.Round(result.PercentualPagasEmDia, 2));
        }

        [Fact]
        public async Task GetResumoCliente_RetornaResumoVazioQuandoSemContratos()
        {
            var cpfCnpj = "836.434.820-51";
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.GetByCliente(cpfCnpj)).ReturnsAsync(new List<Contrato>());
            var service = new ClientesServices(mockRepo.Object);

            var result = await service.GetResumoCliente(cpfCnpj);

            Assert.NotNull(result);
            Assert.Equal(cpfCnpj, result.ClienteCpfCnpj);
            Assert.Equal(0, result.QuantidadeContratosAtivos);
            Assert.Equal(0, result.TotalParcelas);
            Assert.Equal(0, result.ParcelasPagas);
            Assert.Equal(0, result.ParcelasEmAtraso);
            Assert.Equal(0, result.ParcelasAVencer);
            Assert.Equal(0, result.PercentualPagasEmDia);

        }
    }
}
