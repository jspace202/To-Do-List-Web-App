using CCFinal.CanvasIntegration;
using CCFinal.CanvasIntegration.Database;
using CCFinal.CanvasIntegration.Services;
using CCFinal.CanvasIntegration.Subscriptions;
using Microsoft.EntityFrameworkCore;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostBuilderContext, config) => {
        //if (!hostBuilderContext.HostingEnvironment.IsDevelopment())
        //    config.AddEnvironmentVariables("KEY__");
    })
    .ConfigureServices((hostContext, services) => {
        services.AddLogging();

        services.AddDbContext<CanvasIntegrationDbContext>(options =>
            options.UseSqlServer(hostContext.Configuration.GetConnectionString(nameof(CanvasIntegrationDbContext))
                                 ?? throw new InvalidOperationException(
                                     $"Connection string '{nameof(CanvasIntegrationDbContext)}' is not found"),
                serverDbContextOptionsBuilder => {
                    //var minutes = (int)TimeSpan.FromMinutes(3).TotalSeconds;
                    //serverDbContextOptionsBuilder.CommandTimeout(minutes);
                    serverDbContextOptionsBuilder.EnableRetryOnFailure();
                }));
        services.AddScoped<CanvasIntegrationDbContext>();

        services.AddTransient<IKeyUpdate, KeyUpdate>();
        services.AddCap(options => {
            options.UseEntityFramework<CanvasIntegrationDbContext>();
            options.UseKafka(hostContext.Configuration.GetSection("Kafka")["Servers"] ?? string.Empty);
            options.DefaultGroupName = "CanvasIntegrationConsumer";
            options.SucceedMessageExpiredAfter = 10 * 60;
        });

        services.AddSingleton<HttpClient>(_ => {
            var client = new HttpClient();
            var uri = hostContext.Configuration.GetSection("BaseUris")["Canvas"] ?? string.Empty;
            client.BaseAddress = new Uri(uri);

            return client;
        });
        services.AddTransient<ICanvasService, CanvasService>();
        services.AddHostedService<Worker>();
        services.AddHostedService<Cleanup>();
    })
    .Build();

using (var scope = host.Services.CreateAsyncScope()) {
    // Force DB Migrations
    var dbContext = scope.ServiceProvider.GetRequiredService<CanvasIntegrationDbContext>();
    try {
        await dbContext.Database.EnsureCreatedAsync();
    }
    catch (Exception e) {
        Console.WriteLine(e);
    }
}

host.Run();