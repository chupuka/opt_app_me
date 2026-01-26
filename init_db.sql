-- Initialize database for ProForm application

-- Database is created via docker-compose environment variable
-- PostgreSQL doesn't need explicit USE statement like MySQL

-- Example table creation (replace with your actual schema)
/*
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
*/

-- Add your actual table definitions here

-- PostgreSQL uses roles instead of GRANT statements in initialization
-- Permissions are handled via the postgres user and database ownership