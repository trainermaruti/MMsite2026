-- Migration: Add Enterprise Features
-- Description: Adds soft deletes, audit fields, Certificate table, LeadAuditLog table, and enhanced ContactMessage fields
-- Generated: December 1, 2025

-- Add new columns to Training table
ALTER TABLE Trainings ADD UpdatedDate datetime2(7) NULL;
ALTER TABLE Trainings ADD IsDeleted bit NOT NULL DEFAULT 0;

-- Add new columns to Courses table  
ALTER TABLE Courses ADD UpdatedDate datetime2(7) NULL;
ALTER TABLE Courses ADD IsDeleted bit NOT NULL DEFAULT 0;

-- Add new columns to TrainingEvents table
ALTER TABLE TrainingEvents ADD UpdatedDate datetime2(7) NULL;
ALTER TABLE TrainingEvents ADD IsDeleted bit NOT NULL DEFAULT 0;

-- Add new columns to ContactMessages table
ALTER TABLE ContactMessages ADD UpdatedDate datetime2(7) NULL;
ALTER TABLE ContactMessages ADD IsDeleted bit NOT NULL DEFAULT 0;
ALTER TABLE ContactMessages ADD EventId int NULL;
ALTER TABLE ContactMessages ADD Status nvarchar(50) NOT NULL DEFAULT 'New';

-- Create Certificates table
CREATE TABLE Certificates (
    Id int IDENTITY(1,1) NOT NULL,
    CertificateId nvarchar(50) NOT NULL,
    StudentName nvarchar(200) NOT NULL,
    StudentEmail nvarchar(200) NOT NULL,
    CourseTitle nvarchar(200) NOT NULL,
    CourseCategory nvarchar(100) NULL,
    CompletionDate datetime2(7) NOT NULL,
    IssueDate datetime2(7) NOT NULL,
    Instructor nvarchar(200) NULL,
    DurationHours int NOT NULL DEFAULT 0,
    Score decimal(18,2) NULL,
    Grade nvarchar(50) NULL,
    CertificateUrl nvarchar(500) NULL,
    Notes nvarchar(1000) NULL,
    CreatedDate datetime2(7) NOT NULL DEFAULT GETUTCDATE(),
    UpdatedDate datetime2(7) NULL,
    IsDeleted bit NOT NULL DEFAULT 0,
    IsRevoked bit NOT NULL DEFAULT 0,
    RevokedDate datetime2(7) NULL,
    RevocationReason nvarchar(500) NULL,
    CONSTRAINT PK_Certificates PRIMARY KEY (Id)
);

-- Create LeadAuditLogs table
CREATE TABLE LeadAuditLogs (
    Id int IDENTITY(1,1) NOT NULL,
    ContactMessageId int NOT NULL,
    Action nvarchar(100) NOT NULL,
    OldValue nvarchar(50) NULL,
    NewValue nvarchar(50) NULL,
    ChangedBy nvarchar(200) NOT NULL,
    Notes nvarchar(500) NULL,
    CreatedDate datetime2(7) NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT PK_LeadAuditLogs PRIMARY KEY (Id)
);

-- Create indexes for performance
CREATE INDEX IX_ContactMessages_Status ON ContactMessages(Status);
CREATE INDEX IX_ContactMessages_EventId ON ContactMessages(EventId);
CREATE UNIQUE INDEX IX_Certificates_CertificateId ON Certificates(CertificateId);
CREATE INDEX IX_Certificates_StudentEmail ON Certificates(StudentEmail);
CREATE INDEX IX_Certificates_IsDeleted ON Certificates(IsDeleted);
CREATE INDEX IX_LeadAuditLogs_ContactMessageId ON LeadAuditLogs(ContactMessageId);

-- Add foreign key constraints
ALTER TABLE ContactMessages 
ADD CONSTRAINT FK_ContactMessages_TrainingEvents_EventId 
FOREIGN KEY (EventId) REFERENCES TrainingEvents(Id) ON DELETE SET NULL;

ALTER TABLE LeadAuditLogs 
ADD CONSTRAINT FK_LeadAuditLogs_ContactMessages_ContactMessageId 
FOREIGN KEY (ContactMessageId) REFERENCES ContactMessages(Id) ON DELETE CASCADE;

-- Insert sample certificates for testing
INSERT INTO Certificates (CertificateId, StudentName, StudentEmail, CourseTitle, CourseCategory, CompletionDate, IssueDate, Instructor, DurationHours, Score, Grade)
VALUES 
('CERT-2024-001234', 'John Doe', 'john.doe@example.com', 'Azure Fundamentals', 'Cloud', '2024-11-15', '2024-11-16', 'Maruti Makwana', 40, 95.5, 'A+'),
('CERT-2024-001235', 'Jane Smith', 'jane.smith@example.com', 'AI and Machine Learning', 'AI', '2024-11-20', '2024-11-21', 'Maruti Makwana', 60, 92.0, 'A'),
('CERT-2024-001236', 'Bob Johnson', 'bob.j@example.com', 'DevOps Essentials', 'DevOps', '2024-11-25', '2024-11-26', 'Maruti Makwana', 45, 88.5, 'B+');

GO
