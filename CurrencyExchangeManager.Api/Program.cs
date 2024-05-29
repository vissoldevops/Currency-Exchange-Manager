using CurrencyExchangeManager.Api.Data.DBContext;
using CurrencyExchangeManager.Api.Interfaces;
using CurrencyExchangeManager.Api.Repositories;
using CurrencyExchangeManager.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<CurrencyExchangeDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MySQLConnection")), ServiceLifetime.Transient);
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddTransient<IValidationService, ValidationService>();
builder.Services.AddTransient<IExchangeRateService, ExchangeRateService>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddTransient<ICurrencyRateHistoryRepository, CurrencyRateHistoryRepository>();
                             
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<CurrencyExchangeDBContext>();
    context.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
