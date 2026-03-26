using Microsoft.EntityFrameworkCore;

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
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// builder.Services.AddHttpsRedirection(options =>
// {
//     options.HttpsPort = 7079;
// });

var app = builder.Build();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// app.UseHttpsRedirection();

app.Run();
