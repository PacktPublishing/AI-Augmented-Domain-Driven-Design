using System.Security.Cryptography;

namespace BrewUp.Orchestration.Infrastructure;

public static class ArtifactHash
{
    public static string Sha256(string repositoryRoot, string relativePath)
    {
        var absolutePath = Path.Combine(
            repositoryRoot,
            relativePath.Replace('/', Path.DirectorySeparatorChar));
        using var content = File.OpenRead(absolutePath);
        return Convert.ToHexString(SHA256.HashData(content)).ToLowerInvariant();
    }
}
