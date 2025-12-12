-- Migration: Add SkillTechUrl to Courses, Trainings, and TrainingEvents tables
-- Purpose: Enable linking portfolio items to SkillTech.club website
-- Date: December 1, 2025

-- Add SkillTechUrl column to Courses table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Courses' AND COLUMN_NAME = 'SkillTechUrl')
BEGIN
    ALTER TABLE Courses
    ADD SkillTechUrl NVARCHAR(500) NULL;
    
    PRINT 'Added SkillTechUrl column to Courses table';
END
ELSE
BEGIN
    PRINT 'SkillTechUrl column already exists in Courses table';
END
GO

-- Add SkillTechUrl column to Trainings table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Trainings' AND COLUMN_NAME = 'SkillTechUrl')
BEGIN
    ALTER TABLE Trainings
    ADD SkillTechUrl NVARCHAR(500) NULL;
    
    PRINT 'Added SkillTechUrl column to Trainings table';
END
ELSE
BEGIN
    PRINT 'SkillTechUrl column already exists in Trainings table';
END
GO

-- Add SkillTechUrl column to TrainingEvents table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'TrainingEvents' AND COLUMN_NAME = 'SkillTechUrl')
BEGIN
    ALTER TABLE TrainingEvents
    ADD SkillTechUrl NVARCHAR(500) NULL;
    
    PRINT 'Added SkillTechUrl column to TrainingEvents table';
END
ELSE
BEGIN
    PRINT 'SkillTechUrl column already exists in TrainingEvents table';
END
GO

-- Sample data: Update existing courses with SkillTech URLs (based on skilltech.club website)
UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/azure-fundamentals-certification/az-900/1'
WHERE Title LIKE '%Azure Fundamentals%' OR Title LIKE '%AZ-900%';

UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/azure-ai-fundamentals-certification/ai-900/2'
WHERE Title LIKE '%AI Fundamentals%' OR Title LIKE '%AI-900%';

UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/azure-architect-course/az305/6'
WHERE Title LIKE '%Azure Architect%' OR Title LIKE '%AZ-305%';

UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/azure-developer-certification/az-204-certification/11'
WHERE Title LIKE '%Azure Developer%' OR Title LIKE '%AZ-204%';

UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/azure-ai-fundamentals-certification/ai-102-certification/13'
WHERE Title LIKE '%AI-102%';

UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/azure-administrator-training'
WHERE Title LIKE '%Azure Administrator%' OR Title LIKE '%AZ-104%';

UPDATE Courses
SET SkillTechUrl = 'https://skilltech.club/courses/microsoft-copilot-studio'
WHERE Title LIKE '%Copilot%';

PRINT 'Sample course URLs updated';
GO

PRINT 'Migration completed successfully!';
PRINT '';
PRINT 'Next Steps:';
PRINT '1. Update existing records with SkillTech URLs in the admin panel';
PRINT '2. When adding new courses/trainings/events, optionally add SkillTech URL';
PRINT '3. Cards with SkillTech URLs will redirect visitors to SkillTech.club when clicked';
