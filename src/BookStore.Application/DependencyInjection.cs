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
        // register Handlers from Assembly
        cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        // register all open behaviors
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        // register unit of work
        cfg.AddOpenBehavior(typeof(UnitOfWorkBehavior<,>));
      }
    );

    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
    return services;
  }
}
