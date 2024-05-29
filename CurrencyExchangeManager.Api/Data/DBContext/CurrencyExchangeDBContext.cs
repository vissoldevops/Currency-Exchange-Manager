using CurrencyExchangeManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeManager.Api.Data.DBContext;

public partial class CurrencyExchangeDBContext : DbContext
{
    public CurrencyExchangeDBContext()
    {
    }

    public CurrencyExchangeDBContext(DbContextOptions<CurrencyExchangeDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CurrencyRate> CurrencyRates { get; set; }

    public virtual DbSet<CurrencyRateHistory> CurrencyRateHistory { get; set; }
}