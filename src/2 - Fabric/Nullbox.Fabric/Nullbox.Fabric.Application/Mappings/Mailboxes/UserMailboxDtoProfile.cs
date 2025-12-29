using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using Nullbox.Fabric.Domain.Entities.Mailboxes;

[assembly: IntentTemplate("Intent.Application.Dtos.AutoMapper.DtoMappingProfile", Version = "1.0")]

namespace Nullbox.Fabric.Application.Mailboxes;

public class UserMailboxDtoProfile : Profile
{
    public UserMailboxDtoProfile()
    {
        CreateMap<Mailbox, UserMailboxDto>()
                .ForMember(d => d.UserId, opt => opt.MapFrom(src => src.AccountId));
    }
}

public static class UserMailboxDtoMappingExtensions
{
    public static UserMailboxDto MapToUserMailboxDto(this Mailbox projectFrom, IMapper mapper) => mapper.Map<UserMailboxDto>(projectFrom);

    public static List<UserMailboxDto> MapToUserMailboxDtoList(this IEnumerable<Mailbox> projectFrom, IMapper mapper) => projectFrom.Select(x => x.MapToUserMailboxDto(mapper)).ToList();
}