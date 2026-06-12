using BookStore.Application.Authors.Commands.CreateAuthor;
using BookStore.Contracts.Authors;
using BookStore.Domain.Authors;
using Mapster;

namespace BookStore.Api.Common.Mapping;

public sealed class AuthorMappingConfig : IRegister
{
  public void Register(TypeAdapterConfig config)
  {
    config.NewConfig<CreateAuthorRequest, CreateAuthorCommand>();

    config.NewConfig<AuthorAlias, AuthorAliasResponse>()
        .Map(dest => dest.Id, src => src.Id.Value)
        .Map(dest => dest.Name, src => src.Name);

    config.NewConfig<Author, AuthorResponse>()
        .Map(dest => dest.Id, src => src.Id.Value)
        .Map(dest => dest.FirstName, src => src.FirstName)
        .Map(dest => dest.LastName, src => src.LastName)
        .Map(dest => dest.Gender, src => src.Details.Gender.ToString())
        .Map(dest => dest.DateOfBirth, src => src.Details.DateOfBirth)
        .Map(dest => dest.Biography, src => src.Details.Biography)
        .Map(dest => dest.Nationality, src => src.Details.Nationality)
        .Map(dest => dest.BirthPlace, src => src.Details.BirthPlace)
        .Map(dest => dest.DateOfDeath, src => src.Details.DateOfDeath)
        .Map(dest => dest.PortraitImageUrl, src => src.Details.PortraitImageUrl)
        .Map(dest => dest.OfficialWebsite, src => src.Details.OfficialWebsite)
        .Map(dest => dest.IsActive, src => src.IsActive)
        .Map(dest => dest.Aliases, src => src.Aliases);
  }
}
