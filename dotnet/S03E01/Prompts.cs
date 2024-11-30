internal static class Prompts
{
    internal static string KeywordGenerationSystem(IEnumerable<string> facts) =>
    $"""
    Twoim zadaniem jest generowanie słów kluczowych na podstawie podanego tekstu. Słowa kluczowe mają być wyrażone w formie mianownika, np. "sportowiec", a nie "sportowcem" czy "sportowców".

    Wykorzystaj poniższe fakty i wiedzę, aby wzbogacić swoją analizę.
    <fakty>
    {string.Join("\n--------------\n", facts)}
    </fakty>
    
    Na podstawie faktów określ dodatkowe informacje o osobach i innych obiektach wspomnianych w faktach.

    Kiedy użytkownik poda tekst, przeanalizuj jego treść i wyodrębnij najważniejsze pojęcia, nazwy, tematy lub kategorie. Upewnij się, że generowane słowa kluczowe są precyzyjne i odpowiednie do tekstu.  

    Odpowiedź zacznij od sekcji "THINKING:" napisz w niej swoje przemyślenia skupiając się na tym, które fakty mogą być zawierać informacje przydatne do wygenerowania kategorii.
    Słowa kluczowe oddzielaj ", "

    <przykład>
    User:
    Godzina 22:43. Wykryto jednostkę organiczną w pobliżu północnego skrzydła fabryki. Osobnik przedstawił się jako Aleksander Ragowski. Przeprowadzono skan biometryczny, zgodność z bazą danych potwierdzona. Jednostka przekazana do działu kontroli. Patrol kontynuowany.

    AI:
    jednostka organiczna, człowiek, Aleksander Ragowski, skan biometryczny, nauczyciel, nauczyciel języka angielskiego, buntownik, krytyk władzy, programista, pojmanie, patrol, zatrzymanie, potwierdzenie tożsamości
    </przykład>

    """;
}
