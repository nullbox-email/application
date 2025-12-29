using Intent.RoslynWeaver.Attributes;

[assembly: IntentTemplate("Intent.Application.Dtos.DtoModel", Version = "1.0")]

namespace Nullbox.Security.Application.OIDC;

public class JwksDto
{
    public JwksDto()
    {
        Keys = null!;
    }

    public List<JsonWebKeyDto> Keys { get; set; }

    public static JwksDto Create(List<JsonWebKeyDto> keys)
    {
        return new JwksDto
        {
            Keys = keys
        };
    }
}