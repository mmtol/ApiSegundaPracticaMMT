using ApiSegundaPracticaMMT.Data;
using ApiSegundaPracticaMMT.Helpers;
using ApiSegundaPracticaMMT.Models;
using ApiSegundaPracticaMMT.Repositories;
using ApiSegundaPracticaMMT.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHttpContextAccessor();

// Client Vault
var keyVaultUrl = builder.Configuration["KeyVault:VaultUri"];

var client = new SecretClient(
    new Uri(keyVaultUrl),
    new DefaultAzureCredential()
);

builder.Services.AddSingleton(client);

// Model Vault
var crypt = client.GetSecret("cryptexamen");
var secretkey = client.GetSecret("secretkeyexamen");
var audience = client.GetSecret("audienceexamen");
var issuer = client.GetSecret("issuerexamen");
var conn = client.GetSecret("connexamen");

var modelVault = new ModelVault
{
    CryptExamen = crypt.Value.Value,
    SecretKeyExamen = secretkey.Value.Value,
    AudienceExamen = audience.Value.Value,
    IssuerExamen = issuer.Value.Value,
    ConnExamen = conn.Value.Value,
};

builder.Services.AddSingleton(modelVault);

// Key Vault blob
builder.Services.AddSingleton<BlobServiceClient>(provider =>
{
    var client = provider.GetRequiredService<SecretClient>();

    var blobSecret = client.GetSecret("storageexamen");

    return new BlobServiceClient(blobSecret.Value.Value);
});

// Seguridad
HelperActionOAuthService helper = new HelperActionOAuthService(modelVault);
builder.Services.AddSingleton<HelperActionOAuthService>(helper);

builder.Services.AddAuthentication(helper.GetAuthenticationSchema()).AddJwtBearer(helper.GetJWTBearerOptions());

// Services
builder.Services.AddTransient<HelperCrytography>();
builder.Services.AddTransient<HelperUsuarioToken>();
builder.Services.AddTransient<ServiceStorageBlobs>();

// Sql Server
builder.Services.AddDbContext<CubosContext>(options => options.UseSqlServer(modelVault.ConnExamen));
builder.Services.AddTransient<RepositoryCubos>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
