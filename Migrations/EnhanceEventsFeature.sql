-- Migration: Enhance Events Feature
-- Date: 2025-12-02
-- Description: Add new fields to TrainingEvents and create TrainingEventRegistrations table

-- Step 1: Add new columns to TrainingEvents table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TrainingEvents' AND COLUMN_NAME = 'Summary')
    ALTER TABLE TrainingEvents ADD Summary NVARCHAR(500) NULL;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TrainingEvents' AND COLUMN_NAME = 'IsOnline')
    ALTER TABLE TrainingEvents ADD IsOnline BIT NOT NULL DEFAULT 0;

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TrainingEvents' AND COLUMN_NAME = 'TimeZone')
    ALTER TABLE TrainingEvents ADD TimeZone NVARCHAR(100) NOT NULL DEFAULT 'UTC';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TrainingEvents' AND COLUMN_NAME = 'Status')
    ALTER TABLE TrainingEvents ADD Status NVARCHAR(50) NOT NULL DEFAULT 'Draft';

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TrainingEvents' AND COLUMN_NAME = 'BannerUrl')
    ALTER TABLE TrainingEvents ADD BannerUrl NVARCHAR(1000) NULL;

-- Step 2: Create TrainingEventRegistrations table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TrainingEventRegistrations')
BEGIN
    CREATE TABLE TrainingEventRegistrations (
        Id INT PRIMARY KEY IDENTITY(1,1),
        TrainingEventId INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Email NVARCHAR(200) NOT NULL,
        Phone NVARCHAR(20) NULL,
        RegisteredAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        Notes NVARCHAR(1000) NULL,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_TrainingEventRegistrations_TrainingEvents FOREIGN KEY (TrainingEventId) 
            REFERENCES TrainingEvents(Id) ON DELETE CASCADE
    );

    -- Add indexes for performance
    CREATE INDEX IX_TrainingEventRegistrations_TrainingEventId ON TrainingEventRegistrations(TrainingEventId);
    CREATE INDEX IX_TrainingEventRegistrations_Email ON TrainingEventRegistrations(Email);
    CREATE INDEX IX_TrainingEventRegistrations_RegisteredAt ON TrainingEventRegistrations(RegisteredAt DESC);
END

-- Step 3: Add index on TrainingEvents Status field
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TrainingEvents_Status' AND object_id = OBJECT_ID('TrainingEvents'))
    CREATE INDEX IX_TrainingEvents_Status ON TrainingEvents(Status);

-- Step 4: Add index on TrainingEvents TimeZone field  
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TrainingEvents_IsDeleted' AND object_id = OBJECT_ID('TrainingEvents'))
    CREATE INDEX IX_TrainingEvents_IsDeleted ON TrainingEvents(IsDeleted);

PRINT 'Migration completed successfully!';
