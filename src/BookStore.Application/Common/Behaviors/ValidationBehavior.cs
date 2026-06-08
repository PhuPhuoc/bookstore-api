using System.Reflection;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace BookStore.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(
  IValidator<TRequest>? validator
    )
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
  private readonly IValidator<TRequest>? _validator = validator;

  public async Task<TResponse> Handle(
      TRequest request,
      RequestHandlerDelegate<TResponse> next,
      CancellationToken ct)
  {
    if (_validator is null)
      return await next(ct);

    var result = await _validator.ValidateAsync(request, ct);

    if (result.IsValid)
      return await next(ct);

    var errors = result.Errors
        .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
        .ToList();

    var response = (TResponse?)typeof(TResponse)
        .GetMethod("From", BindingFlags.Static | BindingFlags.Public)
        ?.Invoke(null, [errors]);

    return response;
  }
}
