
using BookStore.Application.Authors.Commands.CreateAuthor;
using BookStore.Contracts.Authors;
using MediatR;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Api.Controllers.Authors;

public class AuthorsController(ISender mediator, IMapper mapper) : ApiController(mediator)
{
  private readonly IMapper _mapper = mapper;

  [HttpPost]
  public async Task<IActionResult> Create(CreateAuthorRequest request)
  {
    var command = _mapper.Map<CreateAuthorCommand>(request);
    var result = await _mediator.Send(command);

    return result.Match(
        author => CreatedAtAction(
            nameof(Get),
            new { id = author.Id.Value },
            _mapper.Map<AuthorResponse>(author)),
        errors => Problem(errors)
    );
  }

  [HttpGet()]
  public async Task<IActionResult> GetAll()
  {
    throw new NotImplementedException();
  }

  [HttpGet("{id:guid}")]
  public async Task<IActionResult> Get(Guid id)
  {
    throw new NotImplementedException();
  }

  [HttpPatch("{id:guid}")]
  public async Task<IActionResult> Update(Guid id)
  {
    throw new NotImplementedException();
  }

  [HttpDelete("{id:guid}")]
  public async Task<IActionResult> Delete(Guid id)
  {
    throw new NotImplementedException();
  }
}
