using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class ApiController(
   ISender mediator
    ) : ControllerBase
{
  protected readonly ISender _mediator = mediator;

  protected IActionResult Problem(List<Error> errors)
  {
    var first = errors.First();

    var statusCode = first.Type switch
    {
      ErrorType.NotFound => StatusCodes.Status404NotFound,
      ErrorType.Conflict => StatusCodes.Status409Conflict,
      ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
      _ => StatusCodes.Status500InternalServerError,
    };

    return Problem(statusCode: statusCode, detail: first.Description);
  }
}
