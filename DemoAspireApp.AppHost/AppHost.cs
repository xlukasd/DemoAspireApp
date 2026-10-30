var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiServiceVariableNameDoesNotMatter = builder.AddProject<Projects.DemoAspireApp_ApiService>("mybackend")
    .WithHttpHealthCheck("/health");

builder.AddProject<Projects.DemoAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiServiceVariableNameDoesNotMatter)
    .WaitFor(apiServiceVariableNameDoesNotMatter);

builder.Build().Run();
