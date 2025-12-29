using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Security.Application.OIDC;

public class JsonWebKeyDto
{
    public JsonWebKeyDto()
    {
        Kty = null!;
        Alg = null!;
        Use = null!;
        Kid = null!;
        KeyOps = null!;
    }

    public string Kty { get; set; }
    public string Alg { get; set; }
    public string Use { get; set; }
    public string Kid { get; set; }
    public string? K { get; set; }
    public List<string> KeyOps { get; set; }

    public static JsonWebKeyDto Create(string kty, string alg, string use, string kid, string? k, List<string> keyOps)
    {
        return new JsonWebKeyDto
        {
            Kty = kty,
            Alg = alg,
            Use = use,
            Kid = kid,
            K = k,
            KeyOps = keyOps
        };
    }
}