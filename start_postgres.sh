#!/bin/bash

echo "Starting PostgreSQL service for ProForm project..."

# Check if Docker is installed
if ! command -v docker &> /dev/null; then
    echo "Error: Docker is not installed. Please install Docker first."
    echo "Visit https://docs.docker.com/get-docker/ for installation instructions."
    exit 1
fi

# Check if docker daemon is running
if ! docker info &> /dev/null; then
    echo "Error: Docker daemon is not running. Please start Docker and try again."
    exit 1
fi

# Start PostgreSQL container using docker-compose
if [ -f "docker-compose.yml" ]; then
    echo "Using docker-compose to start PostgreSQL..."
    docker-compose up -d postgres
else
    echo "docker-compose.yml not found, starting PostgreSQL directly..."
    docker run --name proform-postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -e POSTGRES_DB=ProFormDB -p 5432:5432 -v $(pwd)/postgres-data:/var/lib/postgresql/data -d postgres:13
fi

# Wait a few seconds for PostgreSQL to start
echo "Waiting for PostgreSQL to start..."
sleep 10

# Check if the container is running
if docker ps | grep -q proform-postgres; then
    echo "PostgreSQL service started successfully!"
    echo "Container is running. You can now connect using:"
    echo "Server: localhost"
    echo "Port: 5432"
    echo "Database: ProFormDB"
    echo "User: postgres"
    echo "Password: postgres"
else
    echo "Failed to start PostgreSQL service. Check the logs with: docker logs proform-postgres"
    exit 1
fi