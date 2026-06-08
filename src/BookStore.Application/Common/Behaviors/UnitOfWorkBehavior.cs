using BookStore.Application.Common.Messaging;
using BookStore.Application.Common.Interfaces.Persistence;
using ErrorOr;
using MediatR;

namespace BookStore.Application.Common.Behaviors;

public sealed class UnitOfWorkBehavior<TRequest, TResponse>(
    IUnitOfWork unitOfWork)
    : IPipelineBehavior<TRequest, ErrorOr<TResponse>>
    where TRequest : ICommand<TResponse>
{
  public async Task<ErrorOr<TResponse>> Handle(
      TRequest request,
      RequestHandlerDelegate<ErrorOr<TResponse>> next,
      CancellationToken cancellationToken)
  {
    var result = await next(cancellationToken);

    if (!result.IsError)
    {
      await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    return result;
  }
}
