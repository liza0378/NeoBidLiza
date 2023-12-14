using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoBid.Server;
using NeoBid.Server.Auth;
using NeoBid.Server.Components;
using NeoBid.Server.Data;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

// Add services to the container.
services.AddRazorComponents()
    .AddInteractiveServerComponents();

services.AddDbContextFactory<ApplicationDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<AuthenticationStateProvider,
    CustomAuthenticationStateProvider>();

builder.Services.AddCascadingAuthenticationState();

builder.Services.AddScoped<AuthenticationService>();

services.AddHostedService<TimedHostedService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

    try
    {
        using var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migraton");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
