using ErrorOr;
using MediatR;

namespace BookStore.Application.Common.Messaging;

public interface ICommand<TResponse> : IRequest<ErrorOr<TResponse>>
{
}
