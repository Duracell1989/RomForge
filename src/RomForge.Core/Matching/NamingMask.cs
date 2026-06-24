using System.Numerics;
using RomForge.Core.Models;

namespace RomForge.Core.Matching;

public static class NamingMask
{
    public static string Expand(string mask, Game game) =>
        mask.Replace("%u", game.ReleaseNumber.ToString("D4"))
            .Replace("%n", game.Title)
            .Replace("%s", game.SourceRom ?? string.Empty)
            .Replace("%o", game.Comment ?? string.Empty)
            .Replace("%m", MultiLangMarker(game.Language));

    private static string MultiLangMarker(int language)
    {
        int count = BitOperations.PopCount((uint)language);
        return count > 1 ? $"(M{count})" : string.Empty;
    }
}
