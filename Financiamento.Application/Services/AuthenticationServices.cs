using Financiamento.Application.DTOs;
using Financiamento.Application.Interfaces;
using Financiamento.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Financiamento.Application.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly IConfiguration _config;
        private readonly IAuthenticationRepository _authenticationRepository;

        public AuthenticationServices(IConfiguration config, IAuthenticationRepository authenticationRepository)
        {
            _config = config;
            _authenticationRepository = authenticationRepository;
        }

        public async Task<LoginResponseDto> Autenticar(LoginRequestDto req)
        {
            if (req is null || string.IsNullOrWhiteSpace(req.Username) || string.IsNullOrWhiteSpace(req.Senha))
                return null;

            var usuario = await _authenticationRepository.ObterPorUsername(req.Username);
            if (usuario == null) return null;

            bool senhaCorreta = BCrypt.Net.BCrypt.Verify(req.Senha, usuario.Senha);
            if (!senhaCorreta) return null;

            var jwtSection = _config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSection["Key"] ?? string.Empty);
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var expireMinutes = int.TryParse(jwtSection["ExpireMinutes"], out var m) ? m : 60;

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, req.Username),
                new Claim(ClaimTypes.Name, req.Username)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            var retorno = new LoginResponseDto
            {
                Token = jwt,
                TokenType = "Bearer",
                ExpiresIn = (int)TimeSpan.FromMinutes(expireMinutes).TotalSeconds
            };

            return retorno;
        }
    }
}
