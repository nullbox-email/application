using System.Security.Cryptography;
using System.Text;

namespace Nullbox.Fabric.Application.Common.Extensions;

public static class HashingExtensions
{
    public static byte[] ToSha256Hash(this string graph)
    {
        return GetSha256Hash(graph);
    }

    public static string ToSha256HashString(this string graph)
    {
        return GetSha256Hash(graph).ToHashString();
    }

    public static string ToSha256HashString(this byte[] graph)
    {
        return GetSha256Hash(graph).ToHashString();
    }

    public static string ToSha256HashString(this Stream graph)
    {
        return GetSha256Hash(graph).ToHashString();
    }

    public static string ToSha256HashString(this Guid graph)
    {
        return graph.ToByteArray().ToSha256HashString();
    }

    public static string ToHashString(this byte[] graph)
    {
        var builder = new StringBuilder();

        for (int i = 0; i < graph.Length; i++)
        {
            builder.Append($"{graph[i]:X2}");
        }

        return builder.ToString().ToLower();
    }

    public static byte[] GetSha256Hash(byte[] body)
    {
        var mySHA256 = SHA256.Create();

        byte[] hashValue = mySHA256.ComputeHash(body);

        mySHA256.Dispose();

        return hashValue;
    }

    public static byte[] GetSha256Hash(Stream body)
    {
        var mySHA256 = SHA256.Create();

        byte[] hashValue = mySHA256.ComputeHash(body);

        mySHA256.Dispose();

        return hashValue;
    }

    public static byte[] GetSha256Hash(string body)
    {
        return GetSha256Hash(Encoding.Default.GetBytes(body));
    }
}