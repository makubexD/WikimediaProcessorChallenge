IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210126191428_InitialSchema')
BEGIN
    CREATE TABLE [Pages] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(500) NULL,
        [Domain] nvarchar(500) NULL,
        [Language] nvarchar(250) NULL,
        [DomainCode] nvarchar(250) NULL,
        [Active] bit NULL,
        [Created] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreatedBy] nvarchar(50) NOT NULL,
        [Modified] datetime2 NULL,
        [ModifiedBy] nvarchar(50) NULL,
        CONSTRAINT [PK_Pages] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210126191428_InitialSchema')
BEGIN
    CREATE TABLE [Activities] (
        [Id] uniqueidentifier NOT NULL,
        [ActivityDate] date NOT NULL,
        [Count] int NULL,
        [PageId] uniqueidentifier NOT NULL,
        [Active] bit NULL,
        [Created] datetime2 NOT NULL DEFAULT (GETDATE()),
        [CreatedBy] nvarchar(50) NOT NULL,
        [Modified] datetime2 NULL,
        [ModifiedBy] nvarchar(50) NULL,
        CONSTRAINT [PK_Activities] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Activities_Pages_PageId] FOREIGN KEY ([PageId]) REFERENCES [Pages] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210126191428_InitialSchema')
BEGIN
    CREATE INDEX [IX_Activities_PageId] ON [Activities] ([PageId]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210126191428_InitialSchema')
BEGIN
    CREATE INDEX [IX_Pages_Name_DomainCode] ON [Pages] ([Name], [DomainCode]);
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210126191428_InitialSchema')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210126191428_InitialSchema', N'5.0.2');
END;
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210127003340_StoredProceduresReports')
BEGIN
    EXEC('
    CREATE PROCEDURE spLanguageDomainReport
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT p.[Language]
            ,p.[Domain]
            ,FORMAT(a.[ActivityDate], ''yyyyMM'') AS [Period]
            ,SUM(a.[Count]) AS [Count]
        FROM [Pages] p
        INNER JOIN [Activities] a ON p.Id = a.PageId
        GROUP BY p.[Language], p.[Domain], FORMAT(a.[ActivityDate], ''yyyyMM'')
        ORDER BY p.[Domain];
    END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210127003340_StoredProceduresReports')
BEGIN
    EXEC('
    CREATE PROCEDURE spLanguagePageReport
    AS
    BEGIN
        SET NOCOUNT ON;

        SELECT p.[Language]
            ,p.[Name] AS [Page]
            ,FORMAT(a.[ActivityDate], ''yyyyMM'') AS [Period]
            ,SUM(a.[Count]) AS [Count]
        FROM [Pages] p
        INNER JOIN [Activities] a ON p.Id = a.PageId
        GROUP BY p.[Language], p.[Name], FORMAT(a.[ActivityDate], ''yyyyMM'')
        ORDER BY p.[Name];
    END')
END;
GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210127003340_StoredProceduresReports')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210127003340_StoredProceduresReports', N'5.0.2');
END;
GO

COMMIT;
GO

