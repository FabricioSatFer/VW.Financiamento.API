## VW Financiamento API 
Uma API para simulação e gestão de financiamentos de veículos, desenvolvida com foco em Clean Architecture, Domain-Driven Design (DDD) e alta testabilidade. 
O projeto simula o ecossistema de financiamentos da Volkswagen, integrando cálculos financeiros complexos (Tabela Price) e regras de negócio corporativas.

## Tecnologias e Ferramentas
* **Runtime:** .NET 8.0
* **Banco de Dados:** PostgreSQL 
* **Containerização:** Docker & Docker Compose
* **Segurança:** JWT (JSON Web Token) & BCrypt para Hashing de senhas
* **Documentação:** Swagger (OpenAPI)
* **Arquitetura:** Clean Architecture (API, Application, Domain, Infrastructure, Shared)
* **ORM:** Entity Framework Core

## Arquitetura do Projeto
O projeto é dividido em camadas para garantir o desacoplamento e a facilidade de manutenção:

* **Financiamento.Api:** Endpoints REST, filtros de exceção e configurações de Middleware.
* **Financiamento.Application:** Serviços de aplicação, orquestração de lógica e DTOs.
* **Financiamento.Domain:** Entidades de negócio, interfaces de repositório e regras de domínio puras.
* **Financiamento.Infrastructure:** Implementação do DbContext, Repositórios, Migrations e integração com o banco.
* **Financiamento.Tests:** Testes unitários de lógica de negócio e serviços.

## Regras de Negócio Implementadas
### Gestão Financeira
- **Tabela Price:** Cálculo automático de parcelas fixas baseado em taxa mensal e prazo.
- **Amortização e Pagamentos:**
  - **Antecipação:** Desconto de 0,5% para pagamentos antes do vencimento.
  - **Atraso:** Multa fixa de 2% + Juros de mora de 0,1% ao dia.
- **Tipos de Veículo:** Regras diferenciadas para Automóveis, Motos e Caminhões.

### Segurança
- **Autenticação:** Proteção de rotas via JWT.
- **Criptografia:** Senhas armazenadas com Hash BCrypt (Salted).
- **Schema Seguro:** Banco de dados organizado sob o schema `volkswagen`.

## Como Executar
### Pré-requisitos
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

### Passo a Passo
1.  **Clonar o repositório:**
    ```bash
    git clone https://github.com/FabricioSatFer/VW.Financiamento.API.git
    cd VW.Financiamento.API
    ```

2.  **Subir o ambiente com Docker:**
    ```bash
    docker-compose up --build -d
    ```

3.  **Acessar a documentação (Swagger):**
    Aguarde alguns segundos para a inicialização do banco e acesse:
    `http://localhost:5000`

### Usuário de Teste (Seed Data)
O sistema inicia com dados pré-populados no `init.sql`:
* **Usuário:** `admin.fabricio`
* **Senha:** `Senha@123`

## Estrutura do Banco de Dados
A base de dados conta com as seguintes tabelas principais:

* `Usuarios`: Gestão de acesso e perfis.
* `Contratos`: Dados mestre do financiamento (Valor, Taxa, Prazo).
* `Pagamentos`: Controle de parcelas, juros, multas e descontos.
* `TiposVeiculo` / `CondicoesVeiculo`: Tabelas de referência.

## Testes
Para rodar os testes automatizados da aplicação:
```bash
dotnet test
```

Desenvolvido por Fabrício como parte de um desafio técnico para o setor automotivo e financeiro.
