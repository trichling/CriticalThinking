var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .AddDatabase("criticalthinkinggame");

// Add API service
var apiService = builder.AddProject<Projects.CriticalThinkingGame_ApiService>("apiservice")
    .WithReference(postgres)
    .WaitFor(postgres);

// Add Vue.js frontend as an npm project
builder.AddNpmApp("frontend", "../CriticalThinkingGame.Frontend", "dev")
    .WithReference(apiService)
    .WithHttpEndpoint(targetPort: 5173)
    .WithExternalHttpEndpoints()
    .WithEnvironment("VITE_API_URL", apiService.GetEndpoint("http"));

await builder.Build().RunAsync();
