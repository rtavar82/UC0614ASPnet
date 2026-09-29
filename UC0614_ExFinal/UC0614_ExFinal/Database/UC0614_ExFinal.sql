

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
    TokenRecuperacao UNIQUEIDENTIFIER NULL,
    TokenRecuperacaoExpira DATETIME NULL,
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
    @fornecedor NVARCHAR(20),
    @chaveFornecedor NVARCHAR(200),
    @nome NVARCHAR(100),
    @email NVARCHAR(150),
    @passwordHash NVARCHAR(300)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @utilizadorId INT;

    -- 1. Já existe este login externo
    SELECT @utilizadorId = UtilizadorId
    FROM LoginsExternos
    WHERE Fornecedor = @fornecedor
      AND ChaveFornecedor = @chaveFornecedor;

    IF @utilizadorId IS NOT NULL
    BEGIN
        SELECT
            u.Id,
            u.Nome,
            p.Nome AS Perfil
        FROM Utilizadores u
        INNER JOIN Perfis p
            ON p.Id = u.PerfilId
        WHERE u.Id = @utilizadorId
          AND u.Ativo = 1;

        RETURN;
    END

    -- 2. Já existe uma conta local ativa com o mesmo email
    SELECT @utilizadorId = Id
    FROM Utilizadores
    WHERE Email = @email
      AND Ativo = 1;

    IF @utilizadorId IS NOT NULL
    BEGIN
        INSERT INTO LoginsExternos
        (
            UtilizadorId,
            Fornecedor,
            ChaveFornecedor
        )
        VALUES
        (
            @utilizadorId,
            @fornecedor,
            @chaveFornecedor
        );

        SELECT
            u.Id,
            u.Nome,
            p.Nome AS Perfil
        FROM Utilizadores u
        INNER JOIN Perfis p
            ON p.Id = u.PerfilId
        WHERE u.Id = @utilizadorId;

        RETURN;
    END

    -- 3. Se a conta existe mas está pendente, não permite login externo
    IF EXISTS (
        SELECT 1
        FROM Utilizadores
        WHERE Email = @email
          AND Ativo = 0
    )
    BEGIN
        RETURN;
    END

    -- 4. Não existe conta: cria novo utilizador
    INSERT INTO Utilizadores
    (
        Nome,
        Email,
        PasswordHash,
        PerfilId,
        Ativo
    )
    VALUES
    (
        @nome,
        @email,
        @passwordHash,
        2,
        1
    );

    SET @utilizadorId = SCOPE_IDENTITY();

    INSERT INTO LoginsExternos
    (
        UtilizadorId,
        Fornecedor,
        ChaveFornecedor
    )
    VALUES
    (
        @utilizadorId,
        @fornecedor,
        @chaveFornecedor
    );

    SELECT
        u.Id,
        u.Nome,
        p.Nome AS Perfil
    FROM Utilizadores u
    INNER JOIN Perfis p
        ON p.Id = u.PerfilId
    WHERE u.Id = @utilizadorId;
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

CREATE PROCEDURE sp_EliminarUtilizador
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM LoginsExternos
        WHERE UtilizadorId = @id;

        DELETE FROM Utilizadores
        WHERE Id = @id;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
GO

CREATE PROCEDURE sp_CriarTokenRecuperacao
    @email NVARCHAR(150),
    @token UNIQUEIDENTIFIER,
    @expira DATETIME,
    @resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1
        FROM Utilizadores
        WHERE Email = @email
          AND Ativo = 1
    )
    BEGIN
        SET @resultado = 0;
        RETURN;
    END

    UPDATE Utilizadores
    SET TokenRecuperacao = @token,
        TokenRecuperacaoExpira = @expira
    WHERE Email = @email
      AND Ativo = 1;

    SET @resultado = 1;
END;
GO

CREATE PROCEDURE sp_ReporPassword
    @token UNIQUEIDENTIFIER,
    @passwordHash NVARCHAR(300),
    @resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Utilizadores
    SET PasswordHash = @passwordHash,
        TokenRecuperacao = NULL,
        TokenRecuperacaoExpira = NULL
    WHERE TokenRecuperacao = @token
      AND TokenRecuperacaoExpira >= GETDATE()
      AND Ativo = 1;

    IF @@ROWCOUNT = 1
        SET @resultado = 1;
    ELSE
        SET @resultado = 0;
END;
GO

CREATE PROCEDURE sp_ObterUtilizadorPorId
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        Id,
        Nome,
        Email,
        PerfilId,
        Ativo
    FROM Utilizadores
    WHERE Id = @id;
END;
GO
CREATE PROCEDURE sp_AlterarUtilizador
    @id INT,
    @nome NVARCHAR(100),
    @email NVARCHAR(150),
    @perfilId INT,
    @ativo BIT,
    @resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    -- Utilizador não existe
    IF NOT EXISTS (
        SELECT 1
        FROM Utilizadores
        WHERE Id = @id
    )
    BEGIN
        SET @resultado = 0;
        RETURN;
    END

    -- Já existe outra conta com o mesmo email
    IF EXISTS (
        SELECT 1
        FROM Utilizadores
        WHERE Email = @email
          AND Id <> @id
    )
    BEGIN
        SET @resultado = 2;
        RETURN;
    END

    UPDATE Utilizadores
    SET Nome = @nome,
        Email = @email,
        PerfilId = @perfilId,
        Ativo = @ativo
    WHERE Id = @id;

    SET @resultado = 1;
END;
GO
CREATE PROCEDURE sp_InserirUtilizadorAdmin
    @nome NVARCHAR(100),
    @email NVARCHAR(150),
    @passwordHash NVARCHAR(300),
    @perfilId INT,
    @ativo BIT,
    @tokenAtivacao UNIQUEIDENTIFIER = NULL,
    @resultado INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (
        SELECT 1
        FROM Utilizadores
        WHERE Email = @email
    )
    BEGIN
        SET @resultado = 0;
        RETURN;
    END

    INSERT INTO Utilizadores
    (
        Nome,
        Email,
        PasswordHash,
        PerfilId,
        Ativo,
        TokenAtivacao
    )
    VALUES
    (
        @nome,
        @email,
        @passwordHash,
        @perfilId,
        @ativo,
        @tokenAtivacao
    );

    SET @resultado = 1;
END;
GO

