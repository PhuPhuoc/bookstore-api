using BookStore.Application.Common.Interfaces.Persistence;
using BookStore.Domain.Repositories;
using BookStore.Infrastructure.Persistence.Common;
using BookStore.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure.Persistence;

public static class DependencyInjection
{
  public static IServiceCollection AddPersistence(
      this IServiceCollection services,
      IConfiguration configuration)
  {
    services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped<IAuthorRepository, AuthorRepository>();

    return services;
  }
}
