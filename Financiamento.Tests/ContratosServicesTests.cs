using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Financiamento.Application.DTOs;
using Financiamento.Application.Services;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace Financiamento.Tests
{
    public class ContratosServicesTests
    {
        [Fact]
        public async Task GetAll_RetornaContratoDtosMapeados()
        {
            var mockRepo = new Mock<IContratosRepository>();
            var contratos = new List<Contrato>
            {
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "836.434.820-51" , ValorTotal = 1000 },
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "997.062.870-43" , ValorTotal = 2000 },
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "LM.G5P.5CS/0001-49" , ValorTotal = 200000 },
            };

            mockRepo.Setup(r => r.GetAll()).ReturnsAsync(contratos);

            var service = new ContratosServices(mockRepo.Object);

            var result = await service.GetAll();

            Assert.Equal(3, System.Linq.Enumerable.Count(result));
        }

        [Fact]
        public async Task Get_RetornaContratoDtoMapeado()
        {
            var id = Guid.NewGuid();
            var mockRepo = new Mock<IContratosRepository>();

            mockRepo.Setup(r => r.Get(id)).ReturnsAsync(new Contrato { Id = id, ClienteCpfCnpj = "836.434.820-51", ValorTotal = 1000 });

            var service = new ContratosServices(mockRepo.Object);
            var result = await service.Get(id);

            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
            Assert.Equal("836.434.820-51", result.ClienteCpfCnpj);
            Assert.Equal(1000, result.ValorTotal);
        }

        [Fact]
        public async Task Get_RetornaNullQuandoNaoEncontrado()
        {
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Contrato?)null);

            var service = new ContratosServices(mockRepo.Object);

            var result = await service.Get(Guid.NewGuid());

            Assert.Null(result);
        }

        [Fact]
        public async Task Delete_RetornaTrueQuandoEncontrado()
        {
            var id = Guid.NewGuid();
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.Get(id)).ReturnsAsync(new Contrato { Id = id });

            var service = new ContratosServices(mockRepo.Object);

            var result = await service.Delete(id);

            Assert.True(result);
        }

        [Fact]
        public async Task Delete_RetornaFalseQuandoNaoEncontrado()
        {
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.Get(It.IsAny<Guid>())).ReturnsAsync((Contrato?)null);

            var service = new ContratosServices(mockRepo.Object);

            var result = await service.Delete(Guid.NewGuid());

            Assert.False(result);
        }

        [Fact]
        public async Task Create_RetornaTrueQuandoAdicionado()
        {
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.Add(It.IsAny<Contrato>())).ReturnsAsync((Contrato c) => c);

            var service = new ContratosServices(mockRepo.Object);

            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                ValorTotal = 1000,
                TaxaMensal = 1,
                PrazoMeses = 12,
                DataVencimentoPrimeiraParcela = DateTime.UtcNow,
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = await service.Create(dto);

            Assert.True(result.Success);
        }

        [Fact]
        public async Task Create_RetornaFalseQuandoFalha()
        {
            var mockRepo = new Mock<IContratosRepository>();
            mockRepo.Setup(r => r.Add(It.IsAny<Contrato>())).ReturnsAsync((Contrato?)null);
            var service = new ContratosServices(mockRepo.Object);
            var dto = new ContratoCreateDto
            {
                ClienteCpfCnpj = "529.377.080-21",
                TaxaMensal = 1,
                PrazoMeses = 12,
                DataVencimentoPrimeiraParcela = DateTime.UtcNow,
                TipoVeiculo = TipoVeiculo.Automovel,
                CondicaoVeiculo = CondicaoVeiculo.Novo
            };

            var result = await service.Create(dto);

            Assert.False(result.Success);
        }

        [Fact]
        public async Task GetAllPaginado_RetornaPaginaCorreta()
        {
            var mockRepo = new Mock<IContratosRepository>();
            var contratos = new List<Contrato>
            {
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "52937708021", ValorTotal = 1000 },
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "83643482051", ValorTotal = 2000 },
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "99706287043", ValorTotal = 3000 }
            };

            mockRepo.Setup(r => r.GetAllPaginado(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int offset, int pageSize) => 
                {
                    var items = contratos.Skip(offset).Take(pageSize);
                    return (items, contratos.Count);
                });

            var service = new ContratosServices(mockRepo.Object);
            var parameters = new PaginationParameters { Pagina = 1, TamanhoPagina = 2 };

            var result = await service.GetAllPaginado(parameters);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(3, result.TotalRegistros);
            Assert.Equal(2, result.TotalPaginas);
            Assert.False(result.TemAnterior);
            Assert.True(result.TemProximo);
        }

        [Fact]
        public async Task GetAllPaginado_RetornaSegundaPagina()
        {
            var mockRepo = new Mock<IContratosRepository>();
            var contratos = new List<Contrato>
            {
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "52937708021", ValorTotal = 1000 },
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "83643482051", ValorTotal = 2000 },
                new Contrato { Id = Guid.NewGuid(), ClienteCpfCnpj = "99706287043", ValorTotal = 3000 }
            };

            mockRepo.Setup(r => r.GetAllPaginado(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((int offset, int pageSize) =>
                {
                    var items = contratos.Skip(offset).Take(pageSize);
                    return (items, contratos.Count);
                });

            var service = new ContratosServices(mockRepo.Object);
            var parameters = new PaginationParameters { Pagina = 2, TamanhoPagina = 2 };

            var result = await service.GetAllPaginado(parameters);

            Assert.Single(result.Items);
            Assert.Equal(3, result.TotalRegistros);
            Assert.True(result.TemAnterior);
            Assert.False(result.TemProximo);
        }

    }
}
