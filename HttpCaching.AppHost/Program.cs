var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder
    .AddProject<Projects.HttpCaching_ApiService>("apiservice");

builder.AddProject<Projects.HttpCaching_Web>("webfrontend")
    .WithReference(apiService);

builder.Build().Run();
