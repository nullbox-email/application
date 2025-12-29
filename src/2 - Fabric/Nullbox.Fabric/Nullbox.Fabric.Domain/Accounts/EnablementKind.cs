using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.DomainEnum", Version = "1.0")]

namespace Nullbox.Fabric.Domain.Accounts;

public enum EnablementKind
{
    Plan,
    AddOn,
    Promo,
    Admin
}