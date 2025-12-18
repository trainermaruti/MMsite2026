-- Update admin user credentials in the database
UPDATE AspNetUsers 
SET 
    UserName = 'maruti_makwana@hotmail.com',
    NormalizedUserName = 'MARUTI_MAKWANA@HOTMAIL.COM',
    Email = 'maruti_makwana@hotmail.com',
    NormalizedEmail = 'MARUTI_MAKWANA@HOTMAIL.COM'
WHERE NormalizedUserName = 'ADMIN@MARUTITRAINING.COM';
