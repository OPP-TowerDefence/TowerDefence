using TowerDefense;
using TowerDefense.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
        builder.WithOrigins("http://localhost:3000")
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

builder.Services.AddSingleton<GameService>();

builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseRouting();
app.MapHub<GameHub>("/gameHub");

app.Run();
