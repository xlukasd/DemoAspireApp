using DemoAspireApp.AppHost.ApiService;

var builder = DistributedApplication.CreateBuilder(args);

var azureStorage = builder.AddAzureStorage("storage");

var blobs = azureStorage.AddBlobs("blobs");

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.DemoAspireApp_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WaitFor(blobs)
    .WithReference(blobs)
    .WithAddContainerCommand();

builder.AddProject<Projects.DemoAspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
