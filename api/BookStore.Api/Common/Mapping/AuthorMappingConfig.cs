using BookStore.Contracts.Authors;
using BookStore.Domain.Authors;
using Mapster;

namespace BookStore.Api.Common.Mapping;

public sealed class AuthorMappingConfig : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<Author, AuthorResponse>()
        .Map(dest => dest.Id, src => src.Id.Value)
        .Map(dest => dest.FirstName, src => src.FirstName)
        .Map(dest => dest.LastName, src => src.LastName)
        .Map(dest => dest.Gender, src => src.Gender)
        .Map(dest => dest.DateOfBirth, src => src.DateOfBirth);
  }
}
