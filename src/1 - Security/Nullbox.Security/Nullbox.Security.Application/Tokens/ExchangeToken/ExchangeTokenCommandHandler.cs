using System.Security.Claims;
using AutoMapper;
using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Exceptions;
using Nullbox.Security.Application.Common.Interfaces;
using Nullbox.Security.Domain.Entities.Users;
using Nullbox.Security.Domain.Repositories.Users;
using Nullbox.Security.Domain.Services.Tokens;
using Nullbox.Security.Domain.Users;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Security.Application.Tokens.ExchangeToken;

public class ExchangeTokenCommandHandler : IRequestHandler<ExchangeTokenCommand, TokenContractDto>
{
    private readonly ITokenDomainService _tokenDomainService;
    private readonly IExternalUserRepository _externalUserRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ExchangeTokenCommandHandler(
        ITokenDomainService tokenDomainService,
        IExternalUserRepository externalUserRepository,
        IUserProfileRepository userProfileRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _tokenDomainService = tokenDomainService;
        _externalUserRepository = externalUserRepository;
        _userProfileRepository = userProfileRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    [IntentIgnore]
    public async Task<TokenContractDto> Handle(ExchangeTokenCommand request, CancellationToken cancellationToken)
    {
        // Extract external user ID and provider from the claims
        var currentUser = await _currentUserService.GetAsync();

        if (currentUser == null)
        {
            throw new ForbiddenAccessException("No user is logged in.");
        }

        var externalUserId = currentUser.Id;
        var issuer = currentUser.Issuer;
        var audience = currentUser.Audience;

        if (string.IsNullOrEmpty(externalUserId) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new ForbiddenAccessException("Invalid external token.");
        }

        // Construct a unique key for the external provider
        var authenticationContext = $"{issuer}|{audience}";

        // Look up the user by the external user id and authentication context.
        var externalUser = await _externalUserRepository.FindAsync(
            eu => eu.Id == externalUserId && eu.Context == authenticationContext,
            cancellationToken: cancellationToken);
        var userExists = externalUser != null;

        List<Claim> additionalClaims = [];

        UserProfile userProfile = null!;

        if (!userExists)
        {
            userProfile = new UserProfile(currentUser.Email.ToLower());
            externalUser = new ExternalUser(id: externalUserId, context: authenticationContext, userId: userProfile.Id);

            _userProfileRepository.Add(userProfile);
            _externalUserRepository.Add(externalUser);
        }
        else
        {
            userProfile = await _userProfileRepository.FindByIdAsync(id: externalUser.UserId, cancellationToken);
        }

        // Prepare claims
        additionalClaims.AddRange(
            new(Domain.Common.Constants.Claims.GetClaimType(Domain.Common.Constants.Claims.User.Status), userProfile.Status.ToString().ToLower()),
            new(ClaimTypes.Name, !string.IsNullOrWhiteSpace(userProfile.Name) ? userProfile.Name : "unknown"),
            new(ClaimTypes.Email, !string.IsNullOrWhiteSpace(userProfile.EmailAddress.NormalizedValue) ? userProfile.EmailAddress.NormalizedValue : "unknown")
        );

        if (userProfile.Status != UserStatus.Active)
        {
            // Add restricted access claim for new users
            additionalClaims.Add(
                new(Domain.Common.Constants.Claims.GetClaimType(Domain.Common.Constants.Claims.Permissions.RestrictedAccess), "true")
            );
        }

        var result = _tokenDomainService.GetIdToken(
            userId: externalUser?.UserId ?? Guid.Empty,
            claims: additionalClaims,
            audience: "nullbox.email" // TODO: determine audience based on application settings or tenant
        );

        return result.MapToTokenContractDto(_mapper);
    }
}