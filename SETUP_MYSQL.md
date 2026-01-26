# Setting up Local MySQL for Your Project

## Option 1: Using Docker (Recommended)

Since you don't have MySQL installed locally, the easiest approach is to use Docker to run MySQL in a container:

1. Install Docker from https://docs.docker.com/get-docker/
2. Run the following command to start MySQL:

```bash
docker run --name mysql-local -e MYSQL_ROOT_PASSWORD=yourpassword -e MYSQL_DATABASE=ProFormDB -p 3306:3306 -v $(pwd)/mysql-data:/var/lib/mysql -d mysql:8.0
```

3. Update your connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProFormDB;User=root;Password=yourpassword;Port=3306;"
  }
}
```

## Option 2: Install MySQL Locally

### On Windows:
1. Download MySQL Community Server from https://dev.mysql.com/downloads/mysql/
2. Follow the installer steps
3. During installation, make sure to note down the root password

### On macOS:
```bash
brew install mysql
brew services start mysql
```

### On Linux (Ubuntu):
```bash
sudo apt update
sudo apt install mysql-server
sudo service mysql start
```

## Option 3: Using MySQL Portable (For placing in project root)

If you want to have MySQL in your project root as you mentioned:

1. Download MySQL portable version or ZIP archive from the official site
2. Extract to `/workspace/mysql-server` (or wherever you prefer in your project)
3. Configure it to run on a specific port (e.g., 3307 to avoid conflicts)
4. Update your connection string accordingly

## Creating Database Schema

After setting up MySQL, you'll need to create your database schema. You can do this by running:

```sql
CREATE DATABASE ProFormDB;
USE ProFormDB;

-- Add your tables here
```

Or if you're using Entity Framework in your .NET application, you can run migrations:

```bash
dotnet ef database update
```

## Troubleshooting Connection Issues

If you still get "Unable to connect to any of the specified MySQL hosts":

1. Check if MySQL service is running
2. Verify the connection string parameters:
   - Server (localhost vs IP address)
   - Port (default is 3306)
   - Username and password
   - Database name
3. Make sure firewall isn't blocking the connection
4. Verify that MySQL is configured to accept connections on the specified port

## Security Note

The current connection string has an empty password which is insecure. Once everything works, consider setting a strong password and updating the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ProFormDB;User=root;Password=your_secure_password;Port=3306;"
  }
}
```