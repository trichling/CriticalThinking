using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres", port: 5342)
    //.WithPgWeb()
    .WithPgAdmin()
    //.WithDataVolume()
    ;

var database = postgres.AddDatabase("criticalthinking");

var backend = builder.AddProject("backend", "../CriticalThinking.Backend/CriticalThinking.Backend.csproj")
    .WithReference(database)
    .WaitFor(database)
    .WithHttpEndpoint(port: 5000, name: "http");

var frontend = builder.AddNpmApp("frontend", "../CriticalThinking.Frontend", "dev")
    .WithReference(backend)
    .WithHttpEndpoint(targetPort: 5173)
    .WithExternalHttpEndpoints();

builder.Build().Run();