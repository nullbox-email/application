using Intent.RoslynWeaver.Attributes;
using MediatR;
using Nullbox.Security.Application.Common.Exceptions;
using Nullbox.Security.Application.Common.Interfaces;
using Nullbox.Security.Domain.Common.Exceptions;
using Nullbox.Security.Domain.Entities.Users;
using Nullbox.Security.Domain.Repositories.Users;

[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Nullbox.Security.Application.Users.OnboardUser;

public class OnboardUserCommandHandler : IRequestHandler<OnboardUserCommand>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ICurrentUserService _currentUserService;

    public OnboardUserCommandHandler(
        IUserProfileRepository userProfileRepository,
        ICurrentUserService currentUserService)
    {
        _userProfileRepository = userProfileRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(OnboardUserCommand request, CancellationToken cancellationToken)
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

        var existingUser = await _userProfileRepository.FindByIdAsync(id: userId, cancellationToken: cancellationToken);

        if (existingUser == null)
        {
            throw new NotFoundException("User not found.");
        }

        existingUser.SetName(request.Name);
        existingUser.Activate();
    }
}