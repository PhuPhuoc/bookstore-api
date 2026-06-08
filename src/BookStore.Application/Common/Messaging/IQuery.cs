using ErrorOr;
using MediatR;

namespace BookStore.Application.Common.Messaging;

public interface IQuery<TResponse> : IRequest<ErrorOr<TResponse>>
{
}
