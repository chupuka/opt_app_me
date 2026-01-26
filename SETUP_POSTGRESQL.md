# Setting up Local PostgreSQL for Your Project

## Option 1: Using Docker (Recommended)

Since you're switching from MySQL to PostgreSQL, the easiest approach is to use Docker to run PostgreSQL in a container:

1. Install Docker from https://docs.docker.com/get-docker/
2. Run the following command to start PostgreSQL:

```bash
docker run --name postgres-local -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ProFormDB -p 5432:5432 -v $(pwd)/postgres-data:/var/lib/postgresql/data -d postgres:13
```

3. Update your connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ProFormDB;Username=postgres;Password=postgres;Port=5432;"
  }
}
```

## Option 2: Install PostgreSQL Locally

### On Windows:
1. Download PostgreSQL from https://www.postgresql.org/download/windows/
2. Follow the installer steps
3. During installation, note down the superuser (postgres) password you set

### On macOS:
```bash
brew install postgresql
brew services start postgresql
```

### On Linux (Ubuntu):
```bash
sudo apt update
sudo apt install postgresql postgresql-contrib
sudo systemctl start postgresql
sudo systemctl enable postgresql
```

Then create your database:
```bash
sudo -u postgres createdb ProFormDB
```

## Option 3: Using Docker Compose (As configured in this project)

If you have the docker-compose.yml file in your project:

1. Navigate to your project directory
2. Run the following command:

```bash
docker-compose up -d
```

This will start PostgreSQL with the configured settings in the docker-compose.yml file.

## Creating Database Schema

After setting up PostgreSQL, you'll need to create your database schema. You can do this by connecting to the database and running:

```sql
-- Connect to the database and create tables as needed
-- Add your tables here
```

Or if you're using Entity Framework in your .NET application with Npgsql provider, you can run migrations:

```bash
dotnet ef database update
```

Make sure you have the Npgsql.EntityFrameworkCore.PostgreSQL package installed in your project.

## Troubleshooting Connection Issues

If you get connection issues:

1. Check if PostgreSQL service is running
2. Verify the connection string parameters:
   - Host (localhost vs IP address)
   - Port (default is 5432)
   - Username and password
   - Database name
3. Make sure firewall isn't blocking the connection
4. Verify that PostgreSQL is configured to accept connections on the specified port
5. Ensure you're using the correct PostgreSQL connection string format:
   - Use `Host=` instead of `Server=`
   - Use `Username=` instead of `User=`
   - Use `Port=` for PostgreSQL port

## Required NuGet Packages

Make sure your .NET project includes the following packages for PostgreSQL support:

```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="6.0.0" />
```

And potentially:
```xml
<PackageReference Include="Npgsql" Version="6.0.0" />
```

## Security Note

The example connection string uses default credentials which are not secure for production. Once everything works, consider setting a stronger password and updating the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=ProFormDB;Username=postgres;Password=your_secure_password;Port=5432;"
  }
}
```