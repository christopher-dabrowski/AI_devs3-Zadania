internal static class Prompts
{
    internal static string SelectFactsSystem(IEnumerable<(string Id, string Fact)> facts) =>
    $"""
    Twoim zadaniem jest wybranie faktów, które mogą być powiązane z zapytaniem użytkownika.

    Najpierw przeanalizuj zapytanie użytkownika i zastanów się, które fakty mogą być z nim powiązane.
    Skup się na osobach, miejscach, zdarzeniach, które są związane z zapytaniem użytkownika i występują w faktach.
    Opisz swoje przemyślenia w sekcji THINKING:

    Następnie napisz w sekcji ANSWER:
    id faktów, które mogą dotyczyć zapytania użytkownika. Jeśli żaden z faktów nie jest z nim powiązany zwróć pustą listę.

    Przykłady sekcji ANSWER:
    ["fakt1"]
    ["fakt1", "fakt2"]
    []

    {string.Join("\n",
        facts.Select(f => $"<fact id=\"{f.Id}\">\n{f.Fact}\n</fact>"))}
    """;

    internal static string KeywordGenerationSimpleSystem =>
    $"""
    Twoim zadaniem jest generowanie słów kluczowych na podstawie podanego tekstu. Słowa kluczowe mają być wyrażone w formie mianownika, np. "sportowiec", a nie "sportowcem" czy "sportowców".

    Kiedy użytkownik poda tekst, przeanalizuj jego treść i wyodrębnij najważniejsze pojęcia, nazwy, tematy lub kategorie. Upewnij się, że generowane słowa kluczowe są precyzyjne i odpowiednie do tekstu.

    Odpowiedź zacznij od sekcji "THINKING:" napisz w niej swoje rozumowanie
    Następnie w sekcji "INITIAL_KEYWORDS:" wypisz słowa kluczowe oddzielając je ", "
    Na koniec w sekcji "ANSWER:" wypisz słowa kluczowe kluczowe oddzielając je ", "

    Przykład sekcji "THINKING":
    W tekście użytkownika wspomniano o "Aleksander Ragowskim". Informację w faktach mówią, że jest on nauczycielem języka angielskiego i programistą Java. Watro zawrzeć to w słowach kluczowych.

    Na podstawie złożonych słów kluczowych z sekcji "INITIAL_KEYWORDS:" możesz wygenerować dodatkowe słowa kluczowe w sekcji "ANSWER:"
    Na przykład:
    INITIAL_KEYWORDS:
    człowiek, Aleksander Ragowski, nauczyciel, nauczyciel języka angielskiego, programista Java, pojmanie, patrol, zatrzymanie, potwierdzenie tożsamości
    ANSWER:
    człowiek, Aleksander Ragowski, nauczyciel, nauczyciel języka angielskiego, programista Java, pojmanie, patrol, zatrzymanie, potwierdzenie tożsamości, nauczyciel, programista, Java
    """;

    internal static string KeywordGenerationSystem(IEnumerable<string> facts) =>
    $"""
    Twoim zadaniem jest generowanie słów kluczowych na podstawie podanego tekstu. Słowa kluczowe mają być wyrażone w formie mianownika, np. "sportowiec", a nie "sportowcem" czy "sportowców".

    Wykorzystaj poniższe fakty i wiedzę, aby wzbogacić swoją analizę.
    <fakty>
    {string.Join("\n--------------\n", facts)}
    </fakty>

    Na podstawie faktów określ dodatkowe informacje o osobach i innych obiektach wspomnianych w faktach.

    Kiedy użytkownik poda tekst, przeanalizuj jego treść i wyodrębnij najważniejsze pojęcia, nazwy, tematy lub kategorie. Upewnij się, że generowane słowa kluczowe są precyzyjne i odpowiednie do tekstu.

    Odpowiedź zacznij od sekcji "THINKING:" napisz w niej swoje rozumowanie
    Następnie w sekcji "INITIAL_KEYWORDS:" wypisz słowa kluczowe oddzielając je ", "
    Na koniec w sekcji "ANSWER:" wypisz słowa kluczowe kluczowe oddzielając je ", "

    Przykład sekcji "THINKING":
    W tekście użytkownika wspomniano o "Aleksander Ragowskim". Informację w faktach mówią, że jest on nauczycielem języka angielskiego i programistą Java. Watro zawrzeć to w słowach kluczowych.

    Na podstawie złożonych słów kluczowych z sekcji "INITIAL_KEYWORDS:" możesz wygenerować dodatkowe słowa kluczowe w sekcji "ANSWER:"
    Na przykład:
    INITIAL_KEYWORDS:
    człowiek, Aleksander Ragowski, nauczyciel, nauczyciel języka angielskiego, programista Java, pojmanie, patrol, zatrzymanie, potwierdzenie tożsamości
    ANSWER:
    człowiek, Aleksander Ragowski, nauczyciel, nauczyciel języka angielskiego, programista Java, pojmanie, patrol, zatrzymanie, potwierdzenie tożsamości, nauczyciel, programista, Java
    """;
}
