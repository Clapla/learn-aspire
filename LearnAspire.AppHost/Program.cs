using LearnAspire.AppHost.Extensions;

var builder = DistributedApplication.CreateBuilder(args);

var passwordParameter = builder.AddParameter("password", "p5aJ0d7WgxzWu2Cc7yDR)4");

var sql = builder.AddSqlServer("sql", port: 58349, password: passwordParameter)
    .WithLifetime(ContainerLifetime.Persistent);

var serviceBus = builder
    .AddContainer("servicebus", "mcr.microsoft.com/azure-messaging/servicebus-emulator")
    .WithReference(sql.Resource.PrimaryEndpoint)
    .WithEndpoint(port: 5672, targetPort: 5672, name: "servicebus")
    .WithEnvironment("ACCEPT_EULA", "Y")
    .WithEnvironment("SQL_SERVER", "sql")
    .WithEnvironment("MSSQL_SA_PASSWORD", "p5aJ0d7WgxzWu2Cc7yDR)4")
    .WithBindMount("Config.json", "/ServiceBus_Emulator/ConfigFiles/Config.json")
    .WaitFor(sql);

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
    .WaitFor(cache)
    .WaitFor(sql)
    .WaitFor(serviceBus);

builder.Build().Run();
