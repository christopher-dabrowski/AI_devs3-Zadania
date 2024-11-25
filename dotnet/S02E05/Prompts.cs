namespace S02E05;

internal static class Prompts
{
    public const string SystemDescribeImage =
    """
    You are an expert image descriptor.
    Describe the image in detail, focusing on the main elements, colors, composition, and any notable features.
    Keep the description concise but informative.
    """;

    public const string UserDescribeImage =
    """
    Describe this image in detail.
    """;

    public static string AnswerQuestionsBasedOnKnowledge(string knowledge) =>
    $$"""
    Odpowiedz na pytania na podstawie załączonej pracy naukowe. Do odpowiedzi używaj wyłącznie informacji z załączonych danych

    W odpowiedzi zwróć jedynie JSON o strukturze:
    {
        "ID-pytania-01": "krótka odpowiedź w 1 zdaniu",
        "ID-pytania-02": "krótka odpowiedź w 1 zdaniu",
        "ID-pytania-03": "krótka odpowiedź w 1 zdaniu",
        "ID-pytania-NN": "krótka odpowiedź w 1 zdaniu"
    }
    Zastąp "ID-pytania-XX" wartością z zadanego pytania.
    Przykład pytań:
    01=...
    02=...
    Przykład odpowiedzi:
    {
        "01": "...",
        "02": "...",
    }

    ZAWSZE zwracaj jedynie JSON, nie zwracaj żadnego dodatkowego formatowania

    Twoja wiedza:
    {{knowledge}}
    """;
}
