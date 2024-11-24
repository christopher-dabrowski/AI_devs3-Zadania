using System.Collections.Generic;
using System.Text.RegularExpressions;

public static partial class MarkdownParser
{
    [GeneratedRegex(@"!\[.*?\]\((.*?)\)")]
    private static partial Regex ImageRegex();

    public static IEnumerable<MarkdownImage> FindImages(string markdown)
    {
        var matches = ImageRegex().Matches(markdown);

        return matches.Select(match => new MarkdownImage(
            StartIndex: match.Index,
            EndIndex: match.Index + match.Length,
            ImageUrl: match.Groups[1].Value
        ));
    }
}
