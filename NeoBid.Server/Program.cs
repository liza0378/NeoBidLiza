// ��������� �������� �������� ���� ��� ������ � ������������, ���������� ���� �����, �� ������ ������������ �������
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using NeoBid.Server;
using NeoBid.Server.Auth;
using NeoBid.Server.Components;
using NeoBid.Server.Data;

// ��������� ����������� ��� ���-�������
var builder = WebApplication.CreateBuilder(args);

// ���������� ������
var services = builder.Services;

// ������ ������ Razor Components �� Interactive Server Components �� ����������
services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ����������� �������� ���� �����, �������������� SQLite
services.AddDbContextFactory<ApplicationDbContext>(x =>
    x.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// �������� CustomAuthenticationStateProvider �� ����� ��� ��������� ������ ��������������
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// ������ �����, �� �������� �������� ���������� ���� ��������������
builder.Services.AddCascadingAuthenticationState();

// �������� ����� ��� ��������������
builder.Services.AddScoped<AuthenticationService>();

// ������ �����, �� ���������� �� ������ ��������
services.AddHostedService<TimedHostedService>();

// ��������� ��'��� ���-�������
var app = builder.Build();

// ��������� ����� ��� ����������� ���� �����
using (var scope = app.Services.CreateScope())
{
    var provider = scope.ServiceProvider;
    var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

    try
    {
        // ��������� �� ���������� �������� ���� �����
        using var context = provider.GetRequiredService<ApplicationDbContext>();
        await context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
        // ������ �������, �� ������� �� ��� ������� ���� �����
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occured during migration");
        throw;
    }
}

// ����������� ������ ������� HTTP ������
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // ������������� HSTS ��� ��������� ������� (�� ������������� 30 ���)
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

// ����������� ������������� ��� Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ��������� ���-�������
app.Run();
