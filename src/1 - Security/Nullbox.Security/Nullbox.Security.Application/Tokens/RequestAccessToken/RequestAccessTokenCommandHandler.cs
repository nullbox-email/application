using System.Security.Claims;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Exceptions;
using Nullbox.Security.Application.Common.Interfaces;
using Nullbox.Security.Domain.Services.Tokens;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Security.Application.Tokens.RequestAccessToken;

public class RequestAccessTokenCommandHandler : IRequestHandler<RequestAccessTokenCommand, TokenContractDto>
{
    private readonly ITokenDomainService _tokenDomainService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public RequestAccessTokenCommandHandler(
        ITokenDomainService tokenDomainService,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _tokenDomainService = tokenDomainService;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    [IntentIgnore]
    public async Task<TokenContractDto> Handle(RequestAccessTokenCommand request, CancellationToken cancellationToken)
    {
        // Extract external user ID and provider from the claims
        var currentUser = await _currentUserService.GetAsync();

        if (currentUser == null)
        {
            throw new ForbiddenAccessException("No user is logged in.");
        }

        if (!Guid.TryParse(currentUser.Id, out var userId))
        {
            throw new ForbiddenAccessException("Invalid user ID.");
        }

        // Prepare claims
        List<Claim> additionalClaims = [];

        // Prepare authorized scopes
        List<string> authorizedScopes = [.. request.Scopes];

        // Prepare permissions
        List<string> permissions = [];

        var result = _tokenDomainService.GetAccessToken(
            userId: userId,
            authorizedScopes: authorizedScopes,
            permissions: permissions,
            claims: additionalClaims,
            audience: "nullbox.email"  // TODO: determine audience based on application settings or tenant
        );

        return result.MapToTokenContractDto(_mapper);
    }
}