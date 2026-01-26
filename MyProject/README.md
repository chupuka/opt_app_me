# MyProject - PostgreSQL Setup

This project is configured to use PostgreSQL as the database backend. Follow these instructions to set up your development environment.

## Prerequisites

1. .NET 6 SDK
2. Docker and Docker Compose (for running PostgreSQL locally)

## Setting up PostgreSQL

To run PostgreSQL locally using Docker:

```bash
# Navigate to the project root
cd /workspace

# Start PostgreSQL using Docker Compose
./start_postgres.sh  # On Linux/macOS
# OR
start_postgres.bat   # On Windows
```

## Connection String

The application is configured to connect to PostgreSQL using the following connection string:

```
Host=localhost;Database=ProFormDB;Username=postgres;Password=postgres;Port=5432;
```

This is defined in `appsettings.json` in the workspace root.

## Installing Dependencies

Make sure to install the required NuGet packages:

```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.EntityFrameworkCore.Design
```

## Running Migrations

Once PostgreSQL is running, apply the database migrations:

```bash
# From the project directory
dotnet ef migrations add InitialCreate
dotnet ef database update
```

## Project Structure

- `Program.cs` - Contains the main application setup and DbContext configuration
- `MyProject.csproj` - Project file with required dependencies
- `appsettings.json` - Application configuration with PostgreSQL connection string