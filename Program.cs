using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connection = String.Empty;
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
    connection = builder.Configuration.GetConnectionString("DefaultConnection");
}
else
{
    connection = Environment.GetEnvironmentVariable("DefaultConnection");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connection, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));

builder.Services.AddScoped<backend_purchase_order.Services.OrderService>();
builder.Services.AddScoped<backend_purchase_order.Services.OrderActionHistoryService>();
builder.Services.AddScoped<backend_purchase_order.Services.AuthService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "backend_purchase_order",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "backend_purchase_order",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "a_super_secret_key_that_is_at_least_32_bytes_long_1234567890"))
        };
    });

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAuthorization();

// builder.Services.AddHttpsRedirection(options =>
// {
//     options.HttpsPort = 7079;
// });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.Run();
