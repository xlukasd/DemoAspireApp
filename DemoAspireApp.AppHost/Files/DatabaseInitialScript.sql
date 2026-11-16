DECLARE @DatabaseName NVARCHAR(MAX) = N'MyDemoDatabase';

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = N'MyDemoDatabase')
BEGIN
  CREATE DATABASE MyDemoDatabase;
END;
GO

USE MyDemoDatabase;
GO

-- Create the Tasks table
IF OBJECT_ID(N'Tasks', N'U') IS NULL
BEGIN
    CREATE TABLE Tasks
    (
        Id        INT PRIMARY KEY IDENTITY(1,1) ,
        Description NVARCHAR(MAX) NULL,
        Deadline DATETIME2 NULL
    );

    INSERT 
        INTO Tasks (Description, Deadline)
        VALUES 
            ('Complete workshop survey', GETDATE()),
            ('Visit Cologne Cathedral', CAST(DATEADD(day, 3, GETDATE()) AS DATE));
END;
GO