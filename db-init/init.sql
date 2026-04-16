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

INSERT INTO volkswagen.Contratos 
(Id, ClienteCpfCnpj, ValorTotal, TaxaMensal, PrazoMeses, DataVencimentoPrimeiraParcela, TipoVeiculoId, CondicaoVeiculoId)
VALUES 
(gen_random_uuid(), '12345678901', 50000.00, 1.5, 48, '2026-05-10', 1, 1), -- Carro Novo
(gen_random_uuid(), '98765432100', 15000.00, 2.1, 24, '2026-05-15', 2, 2), -- Moto Usada
(gen_random_uuid(), '11223344000199', 450000.00, 0.9, 72, '2026-06-01', 3, 1), -- Caminhão Novo (PJ)
(gen_random_uuid(), '55566677788', 35000.00, 1.8, 36, '2026-05-20', 1, 3), -- Carro Seminovos
(gen_random_uuid(), '44433322211', 12000.00, 2.5, 12, '2026-05-05', 2, 2), -- Moto Usada Curto Prazo
(gen_random_uuid(), '99988877766', 85000.00, 1.2, 60, '2026-07-10', 1, 1), -- Carro Premium Novo
(gen_random_uuid(), '22233344455', 250000.00, 1.1, 48, '2026-05-25', 3, 2), -- Caminhão Usado
(gen_random_uuid(), '77788899900', 42000.00, 1.6, 48, '2026-06-15', 1, 2), -- Carro Usado
(gen_random_uuid(), '66655544433', 18000.00, 2.0, 24, '2026-05-12', 2, 1), -- Moto Nova
(gen_random_uuid(), '33322211100', 500000.00, 0.8, 96, '2026-08-01', 3, 3), -- Caminhão Seminovos Longo Prazo
(gen_random_uuid(), '12121212000100', 65000.00, 1.4, 36, '2026-05-30', 1, 1), -- Carro Empresa
(gen_random_uuid(), '85296374100', 22000.00, 1.9, 18, '2026-05-18', 2, 3), -- Moto Seminovos
(gen_random_uuid(), '14725836900', 120000.00, 1.3, 48, '2026-06-20', 1, 2), -- SUV Usado
(gen_random_uuid(), '36925814700', 310000.00, 1.0, 60, '2026-07-05', 3, 1), -- Caminhão Frota
(gen_random_uuid(), '15935745600', 28000.00, 2.2, 36, '2026-05-22', 2, 2); -- Moto Popular Usada

-- 6. Tabela de Pagamentos (Registro de Transações)
CREATE TABLE volkswagen.Pagamentos (
    Id UUID PRIMARY KEY,
    ContratoId UUID NOT NULL,
    NumeroParcela INT NOT NULL,
    ValorPago DECIMAL(18,2) NOT NULL,
    DataPagamento TIMESTAMP NOT NULL,
	ValorOriginalParcela DECIMAL(18,2) NOT NULL,
	ValorDesconto DECIMAL(18,2) DEFAULT 0.00,
    ValorJuros DECIMAL(18,2) DEFAULT 0.00,
    ValorMulta DECIMAL(18,2) DEFAULT 0.00,
    DataVencimentoParcela DATE NOT NULL,
	DiasAntecipacao INT DEFAULT 0,
    DiasAtraso INT DEFAULT 0,
    StatusPagamentoId INT NOT NULL, -- FK para identificar se foi em dia ou atraso [cite: 25]
    CONSTRAINT fk_contrato FOREIGN KEY (ContratoId) REFERENCES volkswagen.Contratos(Id) ON DELETE CASCADE,
    CONSTRAINT fk_status_pagamento FOREIGN KEY (StatusPagamentoId) REFERENCES volkswagen.StatusPagamento(Id)
);