using JAMFProAPIMigration.Controllers;
using JAMFProAPIMigration.Services.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped<TokenManager>();
builder.Services.AddScoped<FileVault2>();
builder.Services.AddScoped<LAPS>();

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();