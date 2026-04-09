FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copia a Solution
COPY ["Financiamento.slnx", "./"]

# 2. Copia os arquivos de projeto (CSPROJ)
# IMPORTANTE: Verifique se o nome das pastas no Windows é exatamente este.
# Se no Windows for "Financiamento.Api", mantenha. Se for "VW.Financiamento.Api", altere abaixo.
COPY ["Financiamento.Api/Financiamento.Api.csproj", "Financiamento.Api/"]
COPY ["Financiamento.Application/Financiamento.Application.csproj", "Financiamento.Application/"]
COPY ["Financiamento.Domain/Financiamento.Domain.csproj", "Financiamento.Domain/"]
COPY ["Financiamento.Infrastructure/Financiamento.Infrastructure.csproj", "Financiamento.Infrastructure/"]
COPY ["Financiamento.Shared/Financiamento.Shared.csproj", "Financiamento.Shared/"]
COPY ["Financiamento.Tests/Financiamento.Tests.csproj", "Financiamento.Tests/"]

# 3. Restore
RUN dotnet restore "Financiamento.Api/Financiamento.Api.csproj"

# 4. COPIA TODO O CÓDIGO FONTE
# Isso é essencial para que o Publish encontre os arquivos .cs
COPY . .

# 5. PUBLISH
# Rodamos a partir da raiz para garantir que as referências de projeto (../) funcionem
RUN dotnet publish "Financiamento.Api/Financiamento.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 6. RUNTIME
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Configuração de porta para .NET 8
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Financiamento.Api.dll"]