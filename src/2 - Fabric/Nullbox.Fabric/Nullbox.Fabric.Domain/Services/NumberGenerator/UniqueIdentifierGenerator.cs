using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Nullbox.Fabric.Domain.Services.NumberGenerator;

public class UniqueIdentifierGenerator : IUniqueIdentifierGenerator
{
    private readonly UniqueIdentifierGeneratorSettings _settings;
    private readonly SemaphoreSlim _semaphore;
    private readonly StringBuilder _sb;

    public UniqueIdentifierGenerator()
    {
        _settings = new UniqueIdentifierGeneratorSettings();
        _semaphore = new SemaphoreSlim(1, 1);
        _sb = new StringBuilder(_settings.Length + (_settings.Prefix?.Length ?? 0) + (_settings.Postfix?.Length ?? 0));
    }

    public async Task<string> GenerateAsync(UniqueIdentifierGeneratorSettings uniqueIdentifierGeneratorSettings, Func<string, Task<bool>> isUniqueAsync)
    {
        var settings = uniqueIdentifierGeneratorSettings ?? _settings;
        var chars = new string(settings.Chars?.ToArray());

        await _semaphore.WaitAsync();
        try
        {
            string number;
            do
            {
                _sb.Clear();
                _sb.Append(settings.Prefix);
                for (int i = 0; i < settings.Length; i++)
                {
                    var random = GetRandomInt(0, chars.Length);
                    _sb.Append(chars[random]);
                }
                _sb.Append(settings.Postfix);
                number = _sb.ToString();
            } while (isUniqueAsync is not null && !await isUniqueAsync(number));
            return number;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private static int GetRandomInt(int min, int max)
    {
        if (min > max)
        {
            throw new ArgumentOutOfRangeException(nameof(min), "The minimum value must be less than or equal to the maximum value.");
        }
        long diff = max - min;
        using (var rng = RandomNumberGenerator.Create())
        {
            while (true)
            {
                byte[] data = new byte[sizeof(int)];
                rng.GetBytes(data);
                int value = BitConverter.ToInt32(data, 0);
                long remainder = uint.MaxValue % diff;
                if (value >= int.MaxValue - remainder || value < 0)
                {
                    continue;
                }
                return (int)(min + value % diff);
            }
        }
    }
}
