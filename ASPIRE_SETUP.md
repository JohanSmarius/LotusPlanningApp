# Lotus Planning App - Aspire Setup

This application uses .NET Aspire for orchestration and service defaults.

## Prerequisites

- .NET 10 SDK
- Docker Desktop (for running SQL Server)

## Running the Application

### Option 1: Using Aspire AppHost (Recommended)

The AppHost orchestrates all services including SQL Server:

```bash
cd LotusPlanningApp.AppHost
dotnet run
```

This will:
- Start a SQL Server container with persistent storage
- Apply database migrations automatically
- Start the web application
- Open the Aspire dashboard at http://localhost:15XXX (port shown in console)

The web application will be available at the URL shown in the Aspire dashboard.

### Option 2: Running the Web App Standalone

If you need to run just the web app (e.g., for debugging):

1. Start SQL Server manually:
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
  -p 1433:1433 --name lotussql -d \
  mcr.microsoft.com/mssql/server:2022-latest
```

2. Set the connection string:
```bash
export ConnectionStrings__lotusdb="Server=localhost;Database=lotusdb;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True"
```

3. Run the web app:
```bash
cd LotusPlanningApp/LotusPlanningApp
dotnet run
```

## SQL Server Configuration

The SQL Server container is configured with:
- **Port**: 1433 (default SQL Server port)
- **Persistent Storage**: Data is stored in a Docker volume
- **Lifetime**: Container persists across application restarts
- **Database Name**: `lotusdb`

## Service Defaults

The ServiceDefaults project provides:
- **OpenTelemetry**: Distributed tracing and metrics
- **Health Checks**: Available at `/health` and `/alive` (in development)
- **Service Discovery**: Automatic service-to-service communication
- **Resilience**: Retry policies and circuit breakers for HTTP clients

## Migrations

Database migrations are applied automatically on application startup. To create new migrations:

```bash
cd Infrastructure
dotnet ef migrations add YourMigrationName --startup-project ../LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj
```

## Troubleshooting

### Docker Issues
- Ensure Docker Desktop is running
- Check Docker logs: `docker logs lotussql`
- Remove existing container: `docker rm -f lotussql`

### Database Issues
- Check if SQL Server is accessible: `docker exec lotussql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong!Passw0rd" -Q "SELECT @@VERSION"`
- View application logs for migration errors

### Aspire Dashboard
- If the dashboard doesn't open, check the console output for the URL
- The dashboard provides real-time telemetry, logs, and health status
