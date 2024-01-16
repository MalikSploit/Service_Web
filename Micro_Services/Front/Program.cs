using Front.Components;
using Front.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using Front.Components.Pages;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services
    .AddAuthentication(options => {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options => {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(6000);
    });

builder.Services.AddScoped<BookService>();
builder.Services.AddHttpClient<BookService>();
//builder.Services.AddAuthenticationCore();
builder.Services.AddHttpClient();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<CartStateService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<TermsOfServiceModal>();

// Register Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
