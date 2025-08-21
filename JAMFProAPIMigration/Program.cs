using System.Net.Http.Headers;
using JAMFProAPIMigration.Interfaces;
using JAMFProAPIMigration.Services.Core;
using JAMFProAPIMigration.Services.Util;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Services.AddScoped<TokenManager>();
builder.Services.AddScoped<IFileVault2, FileVault2>();
builder.Services.AddScoped<ILAPS, LAPS>();
builder.Services.AddScoped<IRecoveryKeys, RecoveryKeys>();
builder.Services.AddScoped<IComputerService, ComputerService>();
builder.Services.AddScoped<IJamfHttpClient, JamfHttpClient>();

// JamfHttpClient registeration 
builder.Services
    .AddHttpClient<IJamfHttpClient, JamfHttpClient>(client => 
    {
        // Base URL
        client.BaseAddress = new Uri(ConfigProvider.GetJAMFURL());

        // default Accpet header for all reqs
        client.DefaultRequestHeaders
        .Accept
        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    });

builder.Services.AddControllers();

var app = builder.Build();

app.UseRouting();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();