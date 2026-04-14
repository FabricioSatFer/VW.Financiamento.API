using Microsoft.Extensions.Configuration;
using Financiamento.Application.Services;
using Financiamento.Domain.Entities;
using Financiamento.Infrastructure.Interfaces;
using Moq;
using Financiamento.Application.DTOs;

namespace Financiamento.Tests
{
    public class AuthenticationServicesTests
    {
        [Fact]
        public async Task Authenticate_RetornaTokenValido()
        {
            var mockRepo = new Mock<IAuthenticationRepository>();
            mockRepo.Setup(repo => repo.ObterPorUsername(It.IsAny<string>())).ReturnsAsync(new Usuario
            {
                Id = Guid.NewGuid(),
                Username = "usuarioTeste",
                Senha = "$2a$11$HbVTXln4I5f998zSbDX4guAZYja7hUZp52LfXFt62SAOzFOQkkzRe"
            });

            var request = new LoginRequestDto
            {
                Username = "usuarioTeste",
                Senha = "Senha@123"
            };

            var configMock = new Mock<IConfiguration>();
            var jwtSectionMock = new Mock<IConfigurationSection>();
            jwtSectionMock.SetupGet(s => s["Key"]).Returns("52e185ae3b8487f241d8deaad0638b80f754c764bb2d489f7676a3350ed44163");
            jwtSectionMock.SetupGet(s => s["Issuer"]).Returns("test-issuer");
            jwtSectionMock.SetupGet(s => s["Audience"]).Returns("test-audience");
            jwtSectionMock.SetupGet(s => s["ExpireMinutes"]).Returns("60");
            configMock.Setup(c => c.GetSection("Jwt")).Returns(jwtSectionMock.Object);

            var service = new AuthenticationServices(configMock.Object, mockRepo.Object);
            var result = await service.Autenticar(request);

            Assert.NotNull(result);
            Assert.False(string.IsNullOrWhiteSpace(result.Token));
        }

        [Fact]
        public async Task Authenticate_RetornaNullParaCredenciaisInvalidas()
        {
            var mockRepo = new Mock<IAuthenticationRepository>();
            mockRepo.Setup(repo => repo.ObterPorUsername(It.IsAny<string>())).ReturnsAsync((Usuario)null);
            var request = new LoginRequestDto
            {
                Username = "usuarioInexistente",
                Senha = "Senha@123"
            };
            var configMock = new Mock<IConfiguration>();
            var service = new AuthenticationServices(configMock.Object, mockRepo.Object);
            var result = await service.Autenticar(request);
            Assert.Null(result);
        }
    }
}
