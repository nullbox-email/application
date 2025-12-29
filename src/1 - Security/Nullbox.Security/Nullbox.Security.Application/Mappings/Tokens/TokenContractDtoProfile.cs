using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Security.Domain.Contracts.Tokens;

[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Nullbox.Security.Application.Tokens;

public class TokenContractDtoProfile : Profile
{
    public TokenContractDtoProfile()
    {
        CreateMap<TokenContract, TokenContractDto>();
    }
}

public static class TokenContractDtoMappingExtensions
{
    public static TokenContractDto MapToTokenContractDto(this TokenContract projectFrom, IMapper mapper) => mapper.Map<TokenContractDto>(projectFrom);

    public static List<TokenContractDto> MapToTokenContractDtoList(
        this IEnumerable<TokenContract> projectFrom,
        IMapper mapper) => projectFrom.Select(x => x.MapToTokenContractDto(mapper)).ToList();
}