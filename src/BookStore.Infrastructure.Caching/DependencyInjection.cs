using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Infrastructure.Caching;


public static class DependencyInjection
{
  public static IServiceCollection AddCaching(
      this IServiceCollection services,
      IConfiguration configuration)
  {
    return services;
  }
}
