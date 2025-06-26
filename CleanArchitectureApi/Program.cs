using Domain.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("Connection");

// Configurar el servicio DbContext para usar SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Registrar IPaymentService con su implementación PaymentService
builder.Services.AddScoped<IPaymentService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var merchantId = config["OpenPay:MerchantId"];
    var apiKey = config["OpenPay:ApiKey"];

    if (string.IsNullOrWhiteSpace(merchantId))
        throw new InvalidOperationException("OpenPay MerchantId is not configured.");

    if (string.IsNullOrWhiteSpace(apiKey))
        throw new InvalidOperationException("OpenPay ApiKey is not configured.");

    return new PaymentService(merchantId, apiKey);
});

// Añadir política CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS antes de la autorización
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
