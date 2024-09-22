using Carter;
using Microsoft.AspNetCore.Authentication.Cookies;
using RealworldBlazorHtmx.App.ServiceClient;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents();
builder.Services.AddCarter();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddAuthorization();


builder.Services.Configure<ConduitClientSettings>(builder.Configuration.GetSection(nameof(ConduitClientSettings)));
builder.Services.AddHttpClient<IConduitApiClient, ConduitApiClient>();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapCarter();
app.UseStaticFiles();
app.Run();