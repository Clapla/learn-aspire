using LearnAspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight();

var apiService 
    = builder.AddProject<Projects.LearnAspire_ApiService>("apiservice")
        .WithReference(cache)
        .WaitFor(cache)
        .WithDeleteDatabaseCommand();

builder.AddProject<Projects.LearnAspire_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();
