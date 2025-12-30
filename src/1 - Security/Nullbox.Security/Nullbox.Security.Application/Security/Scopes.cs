using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Aryzac.Security.ScopePermissionMap", Version = "1.0")]

namespace Nullbox.Security.Application.Security;

public class Scopes
{
    private List<Scope> _scopes = new List<Scope>();

    public Scopes()
    {
        _scopes.Add(new Scope(Name: "nullbox.user.on-board", Permissions: [typeof(Nullbox.Security.Application.Users.OnboardUser.OnboardUserCommand)]));
    }

    public IReadOnlyList<Scope> All => _scopes;
}

public record Scope(string Name, List<Type> Permissions)
{
    public override string ToString()
    {
        return $"{Name}";
    }
}