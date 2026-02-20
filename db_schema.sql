-- Create Database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'NasaApodDb')
BEGIN
    CREATE DATABASE NasaApodDb;
END
GO
USE NasaApodDb;
GO

-- Create Table Items
IF OBJECT_ID('dbo.ApodItems', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.ApodItems (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        ApodDate DATE NOT NULL,
        Title NVARCHAR(255) NOT NULL,
        Explanation NVARCHAR(MAX),
        MediaType VARCHAR(50) NOT NULL, -- 'image' or 'video'
        Url NVARCHAR(500) NOT NULL,
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- Create Unique Index
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ApodItems_Date' AND object_id = OBJECT_ID('dbo.ApodItems'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_ApodItems_Date ON dbo.ApodItems(ApodDate);
END
GO
