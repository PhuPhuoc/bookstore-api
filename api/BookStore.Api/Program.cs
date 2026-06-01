using Serilog;
using BookStore.Infrastructure.Persistence;
using BookStore.Application;
using Mapster;
using System.Reflection;
using MapsterMapper;

// ── 1. Bootstrap logger (dùng trước khi builder khởi động xong)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
  Log.Information("Starting BookStore API...");

  var builder = WebApplication.CreateBuilder(args);
  {
    // ── 2. Đọc config từ appsettings.json rồi replace bootstrap logger
    builder.Host.UseSerilog((context, services, config) =>
        config.ReadFrom.Configuration(context.Configuration)
              .ReadFrom.Services(services));

    // ── Mapster config
    var mappingConfig = TypeAdapterConfig.GlobalSettings;
    mappingConfig.Scan(Assembly.GetExecutingAssembly());

    builder.Services.AddSingleton(mappingConfig);
    builder.Services.AddScoped<IMapper, ServiceMapper>();

    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    builder.Services.AddApplication();
    builder.Services.AddPersistence(builder.Configuration);
  }

  var app = builder.Build();
  {
    if (app.Environment.IsDevelopment())
    {
      app.UseSwagger().UseSwaggerUI();
    }

    // ── 3. Log mọi HTTP request (method, path, status, duration)
    app.UseSerilogRequestLogging();

    app.UseHttpsRedirection();
    app.MapControllers();
    app.MapGet("/", () => Results.Redirect("/swagger"));
    app.Run();
  }
}
catch (HostAbortedException)
{
  // EF Core design-time uses this internally.
  // Ignore to avoid noisy fatal logs during migrations.
}
catch (Exception ex)
{
  Log.Fatal(ex, "Application terminated unexpectedly.");
}
finally
{
  Log.CloseAndFlush();
}
