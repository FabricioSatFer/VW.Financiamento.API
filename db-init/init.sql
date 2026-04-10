-- Criando o Schema para organizar o projeto
CREATE SCHEMA IF NOT EXISTS volkswagen;

-- 1. Tabela de Status de Pagamento (Referência para as regras de negócio)
CREATE TABLE volkswagen.StatusPagamento (
    Id SERIAL PRIMARY KEY,
    Descricao VARCHAR(20) NOT NULL UNIQUE -- Exemplos: EM_DIA, ATRASO, ANTECIPADO
);

INSERT INTO volkswagen.StatusPagamento (id, Descricao) VALUES (0, 'EM_DIA'), (1, 'ATRASO'), (2, 'ANTECIPADO');

-- 2. Tabela de Tipos de Veículo
CREATE TABLE volkswagen.TiposVeiculo (
    Id SERIAL PRIMARY KEY,
    Descricao VARCHAR(50) NOT NULL UNIQUE -- AUTOMOVEL, MOTO, CAMINHAO [cite: 18]
);

INSERT INTO volkswagen.TiposVeiculo (Descricao) VALUES ('AUTOMOVEL'), ('MOTO'), ('CAMINHAO');

-- 3. Tabela de Condições do Veículo
CREATE TABLE volkswagen.CondicoesVeiculo (
    Id SERIAL PRIMARY KEY,
    Descricao VARCHAR(50) NOT NULL UNIQUE -- NOVO, USADO, SEMINOVO [cite: 19]
);

INSERT INTO volkswagen.CondicoesVeiculo (Descricao) VALUES ('NOVO'), ('USADO'), ('SEMINOVO');

-- 4. Tabela de Usuarios
CREATE TABLE volkswagen.Usuarios (
    Id UUID PRIMARY KEY DEFAULT gen_random_uuid(), 
    Username VARCHAR(50) NOT NULL UNIQUE,
    SenhaHash TEXT NOT NULL,                        
    Email VARCHAR(100) NOT NULL UNIQUE,
    Ativo BOOLEAN DEFAULT TRUE,
    DataCriacao TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UltimoLogin TIMESTAMP WITH TIME ZONE
);

CREATE TABLE volkswagen.Roles (
    Id SERIAL PRIMARY KEY,
    Nome VARCHAR(20) NOT NULL UNIQUE 
);

INSERT INTO volkswagen.Roles (Nome) VALUES 
('ADMIN'), 
('CADASTRO');

ALTER TABLE volkswagen.Usuarios 
ADD COLUMN RoleId INT REFERENCES volkswagen.Roles(Id);

INSERT INTO volkswagen.Usuarios (Username, Email, SenhaHash, RoleId, Ativo) 
VALUES 
(
    'admin.fabricio', 
    'fabricio@vw.com.br', 
    '$2a$11$HbVTXln4I5f998zSbDX4guAZYja7hUZp52LfXFt62SAOzFOQkkzRe', -- Hash BCrypt
    (SELECT Id FROM volkswagen.Roles WHERE Nome = 'ADMIN'), 
    TRUE
),
(
    'cadastro.teste', 
    'teste@gmail.com', 
    '$2a$11$HbVTXln4I5f998zSbDX4guAZYja7hUZp52LfXFt62SAOzFOQkkzRe', -- Hash BCrypt
    (SELECT Id FROM volkswagen.Roles WHERE Nome = 'CADASTRO'), 
    TRUE
);

-- 5. Tabela de Contratos (Entidade Principal)
CREATE TABLE volkswagen.Contratos (
    Id UUID PRIMARY KEY, -- [cite: 12]
    ClienteCpfCnpj VARCHAR(14) NOT NULL, -- [cite: 13]
    ValorTotal DECIMAL(18,2) NOT NULL, -- [cite: 14]
    TaxaMensal DECIMAL(5,2) NOT NULL, -- [cite: 15]
    PrazoMeses INT NOT NULL, -- [cite: 16]
    DataVencimentoPrimeiraParcela DATE NOT NULL, -- [cite: 17]
    TipoVeiculoId INT NOT NULL,
    CondicaoVeiculoId INT NOT NULL,
    DataCriacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_tipo_veiculo FOREIGN KEY (TipoVeiculoId) REFERENCES volkswagen.TiposVeiculo(Id),
    CONSTRAINT fk_condicao_veiculo FOREIGN KEY (CondicaoVeiculoId) REFERENCES volkswagen.CondicoesVeiculo(Id)
);

-- 6. Tabela de Pagamentos (Registro de Transações)
CREATE TABLE volkswagen.Pagamentos (
    Id UUID PRIMARY KEY,
    ContratoId UUID NOT NULL,
    NumeroParcela INT NOT NULL,
    ValorPago DECIMAL(18,2) NOT NULL,
    DataPagamento TIMESTAMP NOT NULL,
    DataVencimentoParcela DATE NOT NULL,
    StatusPagamentoId INT NOT NULL, -- FK para identificar se foi em dia ou atraso [cite: 25]
    CONSTRAINT fk_contrato FOREIGN KEY (ContratoId) REFERENCES volkswagen.Contratos(Id) ON DELETE CASCADE,
    CONSTRAINT fk_status_pagamento FOREIGN KEY (StatusPagamentoId) REFERENCES volkswagen.StatusPagamento(Id)
);