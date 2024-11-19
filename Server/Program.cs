using Serilog;
using TowerDefense;
using TowerDefense.Services;
using TowerDefense.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

if (builder.Environment.IsProduction())
{
    builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration)
);

    builder.Services.AddSingleton<TowerDefense.Interfaces.ILogger, SerilogAdapter>();

}
else
{
    builder.Services.AddSingleton<TowerDefense.Interfaces.ILogger>(Logger.Instance);
}


builder.Services.AddSingleton<GameService>();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseRouting();
app.MapHub<GameHub>("/gameHub");

app.Run();
