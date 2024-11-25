using System.Collections.Generic;
using System.Text.RegularExpressions;

public static partial class MarkdownParser
{
    [GeneratedRegex(@"!\[.*?\]\((.*?)\)")]
    private static partial Regex ImageRegex();

    [GeneratedRegex(@"(?:Twoja przeglądarka nie obsługuje elementu audio\.\s*)?\[(.*?)\]\((.*?\.mp3)\)")]
    private static partial Regex AudioRegex();

    public static IEnumerable<MarkdownImage> FindImages(string markdown)
    {
        var matches = ImageRegex().Matches(markdown);

        return matches.Select(match => new MarkdownImage(
            StartIndex: match.Index,
            EndIndex: match.Index + match.Length,
            ImageUrl: match.Groups[1].Value
        ));
    }

    public static IEnumerable<MarkdownAudio> FindAudioFiles(string markdown)
    {
        var matches = AudioRegex().Matches(markdown);

        return matches.Select(match => new MarkdownAudio(
            StartIndex: match.Index,
            EndIndex: match.Index + match.Length,
            Description: match.Groups[1].Value,
            AudioUrl: match.Groups[2].Value
        ));
    }
}
