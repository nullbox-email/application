using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Entities.Repositories.Api.RepositoryInterface", Version = "1.0")]

namespace Nullbox.Auth.EntraExternalId.Domain.Repositories;

public interface IRepository<in TDomain>
{
    void Add(TDomain entity);
    void Update(TDomain entity);
    void Remove(TDomain entity);
}