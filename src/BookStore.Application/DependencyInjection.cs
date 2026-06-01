using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using BookStore.Application.Common.Behaviors;

namespace BookStore.Application;

public static class DependencyInjection
{
  public static IServiceCollection AddApplication(this IServiceCollection services)
  {
    services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

    services.AddScoped(
        typeof(IPipelineBehavior<,>),
        typeof(ValidationBehavior<,>));

    services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

    var config = TypeAdapterConfig.GlobalSettings;
    config.Scan(typeof(DependencyInjection).Assembly);
    services.AddSingleton(config);
    services.AddScoped<IMapper, ServiceMapper>();

    return services;
  }
}
