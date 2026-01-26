-- Test connection to PostgreSQL and verify the ProFormDB database exists

-- Connect to PostgreSQL (this would normally be done via command line or client)
-- psql -h localhost -U postgres -d ProFormDB

-- Show information about the current connection
SELECT version();

-- Show current database
SELECT current_database();

-- Show current user
SELECT current_user;

-- List all tables in the current schema (should be empty initially)
-- \dt

-- Test a simple query
SELECT NOW() as current_time, USER as current_user, current_database() as current_database;

-- Verify PostgreSQL settings
SHOW server_version;