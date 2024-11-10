using System.Text.Json.Serialization;

namespace S01E02.XyzApi.Models;

public record XyzMessage
{
    [JsonPropertyName("text")]
    public required string Text { get; set; }

    [JsonPropertyName("msgID")]
    public int MsgID { get; set; } = 0;
}
