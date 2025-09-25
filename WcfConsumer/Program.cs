using System.Reflection;
using WcfConsumer.Application.Interfaces;
using WcfConsumer.Application.UseCases.GetCapital;
using WcfConsumer.Domain.Interfaces;
using WcfConsumer.Infrastructure;
using WcfConsumer.Infrastructure.Caching;
using WcfConsumer.Infrastructure.Decorators;
using WcfConsumer.Infrastructure.Resiliency;
using WcfConsumer.Infrastructure.Soap;
using WcfConsumer.Infrastructure.Soap.ExternalServices;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Force detailed startup errors
    builder.WebHost
        .CaptureStartupErrors(true)
        .UseSetting("detailedErrors", "true");

    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();

    // Add services to the container
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Redis cache (read config directly from appsettings.json)
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = builder.Configuration["Redis:Configuration"];
        options.InstanceName = builder.Configuration["Redis:InstanceName"];
    });


    builder.Services.AddScoped<CountryService>();
    builder.Services.AddScoped<ICacheService, DistributedCacheService>();
    builder.Services.AddSingleton<IResiliencyPolicy, PollyResiliencyPolicy>();
    builder.Services.AddScoped<IResilientApiExecutor, ResilientApiExecutor>();
    builder.Services.AddSingleton<ICountryClientFactory, CountryClientFactory>();

    builder.Services.AddScoped<ICountryService>(sp =>
    {
        var service = sp.GetRequiredService<CountryService>();
        var cache = sp.GetRequiredService<ICacheService>();
        var logger = sp.GetRequiredService<ILogger<CacheProxy<ICountryService>>>();

        var proxy = DispatchProxy.Create<ICountryService, CacheProxy<ICountryService>>();
        (proxy as CacheProxy<ICountryService>)!.Decorated = service;
        (proxy as CacheProxy<ICountryService>)!.CacheService = cache;
        (proxy as CacheProxy<ICountryService>)!.Logger = logger;

        return proxy;
    });



    builder.Services.AddScoped<GetCapitalHandler>();

    var app = builder.Build();

    app.MapControllers();

    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI();
    //}
    app.UseSwagger();
    app.UseSwaggerUI();

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("🔥 Fatal startup error: " + ex);
    throw;
}
