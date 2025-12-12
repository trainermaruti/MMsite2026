-- Seed Sample Contact Messages for Testing
-- Use this script to populate the inbox with test data

-- Sample Message 1: Unread inquiry
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'John Doe',
    'john.doe@example.com',
    '+1-555-123-4567',
    'Inquiry about Corporate Training Programs',
    'Hello, I am the HR Manager at TechCorp Solutions. We are interested in providing technical training for our development team of approximately 50 employees. Could you please share more details about your corporate training programs, pricing, and availability?',
    0, -- Unread
    0, -- Not deleted
    'New',
    datetime('now', '-2 days'),
    NULL,
    NULL
);

-- Sample Message 2: Read inquiry
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Jane Smith',
    'jane.smith@techmail.com',
    '+1-555-987-6543',
    'Question about Certification',
    'Hi, I completed your ASP.NET Core training last month. I wanted to know if you provide certificates upon course completion. If yes, what is the process to obtain it?',
    1, -- Read
    0, -- Not deleted
    'Contacted',
    datetime('now', '-5 days'),
    datetime('now', '-4 days'),
    NULL
);

-- Sample Message 3: Unread event registration interest
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Robert Johnson',
    'robert.j@businessmail.com',
    NULL, -- No phone
    'Interest in Upcoming Workshop',
    'I saw your upcoming workshop on Modern Web Development. I would like to register for this event. Please send me the registration details and payment information.',
    0, -- Unread
    0, -- Not deleted
    'New',
    datetime('now', '-1 day'),
    NULL,
    NULL -- You can link this to an actual EventId if you have events in your database
);

-- Sample Message 4: Another unread message
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Emily Davis',
    'emily.davis@startup.io',
    '+1-555-456-7890',
    'Partnership Opportunity',
    'Dear Maruti Training Team, we are a growing startup looking for a training partner. We would like to discuss potential collaboration opportunities. Please let us know if you would be interested in a meeting.',
    0, -- Unread
    0, -- Not deleted
    'New',
    datetime('now', '-6 hours'),
    NULL,
    NULL
);

-- Sample Message 5: Read message
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Michael Brown',
    'michael.brown@enterprise.com',
    '+1-555-321-9876',
    'Training for Remote Team',
    'We have a distributed team across multiple time zones. Do you offer online training sessions that can accommodate different schedules? We are particularly interested in cloud computing and DevOps training.',
    1, -- Read
    0, -- Not deleted
    'Qualified',
    datetime('now', '-7 days'),
    datetime('now', '-6 days'),
    NULL
);

-- Sample Message 6: Unread urgent inquiry
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Sarah Wilson',
    'sarah.wilson@consulting.com',
    '+1-555-111-2222',
    'Urgent: Training Required Next Week',
    'Hi, we have an urgent requirement for ASP.NET Core training for 10 developers starting next week. Can you accommodate this on short notice? Please respond as soon as possible.',
    0, -- Unread
    0, -- Not deleted
    'New',
    datetime('now', '-3 hours'),
    NULL,
    NULL
);

-- Sample Message 7: Read follow-up
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'David Martinez',
    'david.martinez@education.org',
    '+1-555-777-8888',
    'Follow-up on Previous Inquiry',
    'Following up on my previous email about custom curriculum development. Have you had a chance to review our requirements? Looking forward to your response.',
    1, -- Read
    0, -- Not deleted
    'Contacted',
    datetime('now', '-10 days'),
    datetime('now', '-9 days'),
    NULL
);

-- Sample Message 8: Unread price inquiry
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Lisa Anderson',
    'lisa.anderson@fintech.com',
    '+1-555-444-5555',
    'Pricing Information Request',
    'Could you please send me detailed pricing information for your full-stack web development course? We are considering enrolling 5-8 team members.',
    0, -- Unread
    0, -- Not deleted
    'New',
    datetime('now', '-12 hours'),
    NULL,
    NULL
);

-- Sample Message 9: Read feedback
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'James Taylor',
    'james.taylor@alumni.com',
    NULL,
    'Thank You and Feedback',
    'I wanted to thank you for the excellent training session last week. The instructor was very knowledgeable and the hands-on exercises were extremely helpful. I would definitely recommend your training programs to others.',
    1, -- Read
    0, -- Not deleted
    'Converted',
    datetime('now', '-15 days'),
    datetime('now', '-14 days'),
    NULL
);

-- Sample Message 10: Unread technical question
INSERT INTO ContactMessages (Name, Email, PhoneNumber, Subject, Message, IsRead, IsDeleted, Status, CreatedDate, UpdatedDate, EventId)
VALUES (
    'Amanda White',
    'amanda.white@devshop.com',
    '+1-555-666-3333',
    'Question about Course Prerequisites',
    'I am interested in the Advanced ASP.NET Core course. What are the prerequisites? Do I need prior experience with .NET framework or can I start directly with .NET Core?',
    0, -- Unread
    0, -- Not deleted
    'New',
    datetime('now', '-18 hours'),
    NULL,
    NULL
);

-- Verify insertion
SELECT COUNT(*) as 'Total Messages Inserted' FROM ContactMessages WHERE IsDeleted = 0;
SELECT COUNT(*) as 'Unread Messages' FROM ContactMessages WHERE IsDeleted = 0 AND IsRead = 0;
