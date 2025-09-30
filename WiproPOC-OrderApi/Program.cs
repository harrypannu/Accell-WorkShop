using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using WiproPOC_OrderApi.DAL;


TokenCredential credential;

var builder = WebApplication.CreateBuilder(args);

string keyVaultUrl = builder.Configuration["AzureApi:KeyVaultUri"];
string tenantId = builder.Configuration["AzureAd:TenantId"];
string clientId = builder.Configuration["AzureAd:ClientId"];
string clientSecret = "xyz";

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Api", Version = "v1" });
});

var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

var client = new SecretClient(new Uri(keyVaultUrl), clientSecretCredential);

KeyVaultSecret con = client.GetSecret("SqlConnectionString");
string dbConnectionString = con.Value;

builder.Services.AddDbContext<OrderDbContext>(options =>
          options.UseSqlServer(dbConnectionString));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Order Api v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
