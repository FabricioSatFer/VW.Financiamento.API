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
            mockRepo.Setup(r => r.GetByCliente("21747386006")).ReturnsAsync(new List<Contrato>
            {
                new Contrato
                {
                    Id = Guid.NewGuid(),
                    ClienteCpfCnpj = "21747386006",
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

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(cpfCnpj, result.Value.ClienteCpfCnpj);
            Assert.Equal(1, result.Value.QuantidadeContratosAtivos);
            Assert.Equal(12, result.Value.TotalParcelas);
            Assert.Equal(3, result.Value.ParcelasPagas);
            Assert.Equal(1, result.Value.ParcelasEmAtraso);
            Assert.Equal(9, result.Value.ParcelasAVencer);
            Assert.Equal(25m, Math.Round(result.Value.PercentualPagasEmDia, 2));
        }

        [Fact]
        public async Task GetResumoCliente_RetornaResumoCorretoComContratosSemPagamentos()
        {
            var cpfCnpj = "217.473.860-06";
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.GetByCliente("21747386006")).ReturnsAsync(new List<Contrato>
            {
                new Contrato
                {
                    Id = Guid.NewGuid(),
                    ClienteCpfCnpj = "21747386006",
                    PrazoMeses = 12,
                    Pagamentos = new List<Pagamento>()
                }
            });
            var service = new ClientesServices(mockRepo.Object);

            var result = await service.GetResumoCliente(cpfCnpj);

            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            Assert.Equal(cpfCnpj, result.Value.ClienteCpfCnpj);
            Assert.Equal(1, result.Value.QuantidadeContratosAtivos);
            Assert.Equal(12, result.Value.TotalParcelas);
            Assert.Equal(0, result.Value.ParcelasPagas);
            Assert.Equal(0, result.Value.ParcelasEmAtraso);
            Assert.Equal(12, result.Value.ParcelasAVencer);
            Assert.Equal(0m, result.Value.PercentualPagasEmDia);
        }

        [Fact]
        public async Task GetResumoCliente_RetornaErroQuandoSemContratos()
        {
            var cpfCnpj = "836.434.820-51";
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.GetByCliente("83643482051")).ReturnsAsync(new List<Contrato>());
            var service = new ClientesServices(mockRepo.Object);

            var result = await service.GetResumoCliente(cpfCnpj);

            Assert.False(result.Success);
            Assert.Null(result.Value);
            Assert.NotEmpty(result.Errors);
            Assert.Contains($"Nenhum contrato encontrado para o cliente {cpfCnpj}", result.Errors);
            Assert.Equal("CLIENT_NOT_FOUND", result.Code);
        }
    }
}
