--CREATE DATABASE UC0614_ExFinal;
--GO
USE UC0614_Teste;
GO

CREATE TABLE Perfis (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(30) NOT NULL UNIQUE
);
INSERT INTO Perfis (Nome) VALUES ('Administrador'), ('Utilizador');

CREATE TABLE Utilizadores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(300) NOT NULL,
    PerfilId INT NOT NULL CONSTRAINT FK_Utilizadores_Perfis FOREIGN KEY REFERENCES Perfis(Id),
    Ativo BIT NOT NULL CONSTRAINT DF_Utilizadores_Ativo DEFAULT 0,
    TokenAtivacao UNIQUEIDENTIFIER NULL,
    CriadoEm DATETIME NOT NULL CONSTRAINT DF_Utilizadores_CriadoEm DEFAULT GETDATE()
);
GO

CREATE TABLE LoginsExternos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UtilizadorId INT NOT NULL CONSTRAINT FK_LoginsExternos_Utilizadores FOREIGN KEY REFERENCES Utilizadores(Id),
    Fornecedor NVARCHAR(20) NOT NULL,
    ChaveFornecedor NVARCHAR(200) NOT NULL,
    CONSTRAINT UQ_LoginsExternos_Fornecedor_Chave UNIQUE (Fornecedor, ChaveFornecedor)
);
GO


CREATE PROCEDURE sp_RegistarUtilizador
    @nome NVARCHAR(100), @email NVARCHAR(150), @passwordHash NVARCHAR(300), @perfilId INT = 2,
    @resultado INT OUTPUT, @token UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM Utilizadores WHERE Email = @email)
    BEGIN SET @resultado = 0; RETURN; END
    SET @token = NEWID();
    INSERT INTO Utilizadores (Nome, Email, PasswordHash, PerfilId, TokenAtivacao)
    VALUES (@nome, @email, @passwordHash, @perfilId, @token);
    SET @resultado = 1;
END;
GO

CREATE PROCEDURE sp_AtivarConta @token UNIQUEIDENTIFIER, @resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE Utilizadores SET Ativo = 1, TokenAtivacao = NULL WHERE TokenAtivacao = @token AND Ativo = 0;
    SET @resultado = @@ROWCOUNT;
END;
GO

CREATE PROCEDURE sp_ObterUtilizadorLogin @email NVARCHAR(150)
AS
BEGIN
    SELECT u.Id, u.Nome, u.Email, u.PasswordHash, u.Ativo, p.Nome AS Perfil
    FROM Utilizadores u INNER JOIN Perfis p ON p.Id = u.PerfilId WHERE u.Email = @email;
END;
GO

CREATE PROCEDURE sp_ObterOuCriarLoginExterno
    @fornecedor NVARCHAR(20), @chaveFornecedor NVARCHAR(200),
    @nome NVARCHAR(100), @email NVARCHAR(150), @passwordHash NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.Id, u.Nome, p.Nome AS Perfil
    FROM LoginsExternos l INNER JOIN Utilizadores u ON u.Id = l.UtilizadorId
    INNER JOIN Perfis p ON p.Id = u.PerfilId
    WHERE l.Fornecedor = @fornecedor AND l.ChaveFornecedor = @chaveFornecedor;
    IF @@ROWCOUNT > 0 RETURN;
    IF EXISTS (SELECT 1 FROM Utilizadores WHERE Email = @email) RETURN;
    INSERT INTO Utilizadores (Nome, Email, PasswordHash, PerfilId, Ativo)
    VALUES (@nome, @email, @passwordHash, 2, 1);
    DECLARE @utilizadorId INT = SCOPE_IDENTITY();
    INSERT INTO LoginsExternos (UtilizadorId, Fornecedor, ChaveFornecedor)
    VALUES (@utilizadorId, @fornecedor, @chaveFornecedor);
    SELECT u.Id, u.Nome, p.Nome AS Perfil
    FROM Utilizadores u INNER JOIN Perfis p ON p.Id = u.PerfilId WHERE u.Id = @utilizadorId;
END;
GO

CREATE PROCEDURE sp_AlterarPassword @id INT, @passwordHash NVARCHAR(300)
AS
BEGIN UPDATE Utilizadores SET PasswordHash = @passwordHash WHERE Id = @id; END;
GO

CREATE PROCEDURE sp_ListarUtilizadores
AS
BEGIN
    SELECT u.Id, u.Nome, u.Email, p.Nome AS Perfil, CASE WHEN u.Ativo = 1 THEN 'Ativo' ELSE 'Pendente' END AS Estado
    FROM Utilizadores u INNER JOIN Perfis p ON p.Id = u.PerfilId ORDER BY u.Nome;
END;
GO

CREATE PROCEDURE sp_EliminarUtilizador @id INT
AS
BEGIN DELETE FROM Utilizadores WHERE Id = @id; END;
GO
