var builder = DistributedApplication.CreateBuilder(args);

var initialSqlScript = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Files", "DatabaseInitialScript.sql"));

var sqlServer = builder
    .AddSqlServer("sqlServer");

var database = sqlServer.AddDatabase("MyDemoDatabase");

database.WithCreationScript(initialSqlScript);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.DemoAspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(database)
    .WithReference(database);

builder.AddProject<Projects.DemoAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
