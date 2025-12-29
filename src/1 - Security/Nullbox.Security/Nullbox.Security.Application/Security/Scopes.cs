using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Endura.ScopePermissionMap", Version = "1.0")]

namespace Nullbox.Security.Application.Security;

public class Scopes
{
    private List<Scope> _scopes;

    public Scopes()
    {
        _scopes.Add(new Scope(Name: "nullbox.user.on-board", Permissions: [typeof(Nullbox.Security.Application.Users.OnboardUser.OnboardUserCommand)]));
    }
}

public record Scope(string Name, List<Type> Permissions)
{
    public override string ToString()
    {
        return $"{Name}";
    }
}