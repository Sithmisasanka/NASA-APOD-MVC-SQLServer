-- ============================================
-- NASA APOD Database Setup Script
-- Target: SQL Server Express / LocalDB
-- ============================================

-- 1. Create Database (if not exists)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'NasaApodDb')
BEGIN
    CREATE DATABASE NasaApodDb;
END
GO

USE NasaApodDb;
GO

-- 2. Create Table (if not exists)
IF OBJECT_ID(N'dbo.ApodEntries', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApodEntries
    (
        Id              INT             IDENTITY(1,1)   NOT NULL,
        ApodDate        DATE                            NOT NULL,
        Title           NVARCHAR(300)                   NOT NULL,
        Explanation     NVARCHAR(MAX)                   NULL,
        Url             NVARCHAR(1000)                  NOT NULL,
        MediaType       NVARCHAR(50)                    NOT NULL,
        ServiceVersion  NVARCHAR(50)                    NULL,
        SavedAt         DATETIME2                       NOT NULL    DEFAULT SYSUTCDATETIME(),

        CONSTRAINT PK_ApodEntries PRIMARY KEY CLUSTERED (Id)
    );
END
GO

-- 3. Create Unique Index on ApodDate (prevents duplicate dates)
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
    WHERE name = N'UX_ApodEntries_ApodDate'
      AND object_id = OBJECT_ID(N'dbo.ApodEntries')
)
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX UX_ApodEntries_ApodDate
        ON dbo.ApodEntries (ApodDate);
END
GO

PRINT 'Database [NasaApodDb] and table [dbo.ApodEntries] are ready.';
GO
