@echo off
echo Starting PostgreSQL service for ProForm project...

REM Check if Docker is installed
docker --version >nul 2>&1
if errorlevel 1 (
    echo Error: Docker is not installed. Please install Docker first.
    echo Visit https://docs.docker.com/get-docker/ for installation instructions.
    pause
    exit /b 1
)

REM Check if docker daemon is running
docker info >nul 2>&1
if errorlevel 1 (
    echo Error: Docker daemon is not running. Please start Docker Desktop and try again.
    pause
    exit /b 1
)

REM Start PostgreSQL container using docker-compose
if exist "docker-compose.yml" (
    echo Using docker-compose to start PostgreSQL...
    docker-compose up -d postgres
) else (
    echo docker-compose.yml not found, starting PostgreSQL directly...
    docker run --name proform-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ProFormDB -p 5432:5432 -v %cd%\postgres-data:/var/lib/postgresql/data -d postgres:13
)

REM Wait a few seconds for PostgreSQL to start
echo Waiting for PostgreSQL to start...
timeout /t 10 /nobreak >nul

REM Check if the container is running
docker ps --format "table {{.Names}}" | findstr "proform-postgres" >nul
if %errorlevel% equ 0 (
    echo PostgreSQL service started successfully!
    echo Container is running. You can now connect using:
    echo Server: localhost
    echo Port: 5432
    echo Database: ProFormDB
    echo User: postgres
    echo Password: postgres
) else (
    echo Failed to start PostgreSQL service. Check the logs with: docker logs proform-postgres
    pause
    exit /b 1
)

pause