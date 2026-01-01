var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server with persistent storage and default port (1433)
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume() // Persistent storage
    .WithLifetime(ContainerLifetime.Persistent)
    .WithEndpoint(port: 1433, targetPort: 1433, name: "sql"); // Default SQL Server port

// Create the database
var database = sqlServer.AddDatabase("lotusdb");

// Add the web project and reference the database
var lotusApp = builder.AddProject("lotusplanningapp", "../LotusPlanningApp/LotusPlanningApp/LotusPlanningApp.csproj")
    .WithExternalHttpEndpoints()
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
