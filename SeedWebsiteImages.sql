-- Seed initial website images
INSERT INTO WebsiteImages (ImageKey, DisplayName, Description, ImageUrl, AltText, Category, CreatedDate, UpdatedDate, IsDeleted)
VALUES 
('profile_main', 'Main Profile Picture', 'Profile picture displayed in header navigation', '/images/44.png', 'Profile Picture', 'Profile', datetime('now'), datetime('now'), 0),
('profile_hero', 'About Section Photo', 'Large profile photo in about section', '/images/22.png', 'Maruti Makwana', 'About', datetime('now'), datetime('now'), 0),
('experience_badge', 'Experience Badge', 'Badge showing years of experience', '/images/experience-badge.png', 'Experience Badge', 'Badge', datetime('now'), datetime('now'), 0);
