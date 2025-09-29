using CacheManager.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

var authenticationProviderKey = "Bearer";

// Step 1: Add Azure AD authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"), authenticationProviderKey);

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

// Step 2: Configure JWT events
builder.Services.Configure<JwtBearerOptions>(authenticationProviderKey, options =>
{
    options.Events.OnTokenValidated = context =>
    {
        Console.WriteLine("Token validated successfully!");
        foreach (var claim in context.Principal.Claims)
        {
            Console.WriteLine($"{claim.Type} : {claim.Value}");
        }
        return Task.CompletedTask;
    };

    options.Events.OnAuthenticationFailed = context =>
    {
        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
        return Task.CompletedTask;
    };
});

//builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddOcelot(builder.Configuration)
    .AddCacheManager(x => x.WithDictionaryHandle());

//Need to enable premium subscription
//builder.Services.AddOcelot(builder.Configuration)
//    .AddCacheManager(settings =>
//    {
//        settings.WithRedisConfiguration("redis", config =>
//        {
//            config.WithEndpoint("localhost", 6379)
//                  .WithAllowAdmin()
//                  .WithDatabase(0);
//        })
//        .WithMaxRetries(50)
//        .WithRetryTimeout(100);
//    });

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();


await app.UseOcelot();
app.MapGet("/", () => "Hello World!");

app.Run();
