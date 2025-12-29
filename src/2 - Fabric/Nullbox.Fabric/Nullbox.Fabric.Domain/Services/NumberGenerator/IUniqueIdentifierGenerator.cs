using System.Threading.Tasks;

namespace Nullbox.Fabric.Domain.Services.NumberGenerator;

public interface IUniqueIdentifierGenerator
{
    Task<string> GenerateAsync(UniqueIdentifierGeneratorSettings uniqueIdentifierGeneratorSettings
        , Func<string, Task<bool>> isUniqueAsync);
}
