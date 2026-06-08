using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Application.Common.Behaviors;
using System.Reflection;

namespace BookStore.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddMediatR(cfg =>
      {
        // 1.register Handlers from Assembly
        // cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        // 2. register all open behaviors
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        // unit of work
        cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
      }
    );

    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    return services;
  }
}
