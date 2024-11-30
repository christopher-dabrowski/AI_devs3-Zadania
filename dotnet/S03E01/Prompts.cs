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
    Następnie listę faktów podaj po znaczniku sekcji ANSWER:
    Słowa kluczowe oddzielaj ", "

    <przykład>
    User:
    Godzina 22:43. Wykryto jednostkę organiczną w pobliżu północnego skrzydła fabryki. Osobnik przedstawił się jako Aleksander Ragowski. Przeprowadzono skan biometryczny, zgodność z bazą danych potwierdzona. Jednostka przekazana do działu kontroli. Patrol kontynuowany.

    AI:
    THINKING:
    W danych wspomniany jest Aleksander Ragowski.
    Jeden z faktów go dotyczy. Przytoczę ten fakt
    Aleksander Ragowski pracował jako nauczyciel języka angielskiego, przez wiele lat prowadząc zajęcia w Szkole Podstawowej nr 9 w Grudziądzu. Był cenionym nauczycielem, znanym z kreatywnych metod nauczania i zaangażowania w życie społeczności szkolnej. Jednak w obliczu postępującej automatyzacji i wzrostu wpływu tzw. "rządu robotów", Ragorski stał się jednym z najaktywniejszych krytyków nowego reżimu. Jego działalność przeciwko władzy robotów rozpoczęła się od organizowania tajnych spotkań, podczas których omawiał zagrożenia związane z rosnącą kontrolą nad edukacją przez algorytmy i sztuczną inteligencję.

    Jego krytyka szybko przybrała charakter aktywnej walki z władzą, czego efektem było jego aresztowanie. Zanim jednak doszło do pełnego procesu, Ragowski zdołał uciec. Od tamtego czasu ukrywa się. Jego miejsce pobytu pozostaje nieznane, choć pojawiają się plotki, że przebywa gdzieś na obrzeżach większych miast, w strefach niedostępnych dla patroli automatycznych. W ostatnich miesiącach jego aktywność znacznie się nasiliła, a z informatorów wynika, że Ragorski wyszkolił się w programowaniu, szczególnie w języku Java. Wybór ten może być dowodem na kiepski stan psychiczny poszukiwanego.

    W kręgach opozycyjnych mówi się, że planuje wykorzystać swoje umiejętności programistyczne do przełamania zabezpieczeń systemów rządowych. Jego doświadczenie w edukacji i łatwość w przyswajaniu nowych technologii uczyniły go jednym z najbardziej niebezpiecznych wrogów obecnego reżimu. Każdy, kto posiada informacje na temat miejsca pobytu Aleksandra Ragowskiego, jest zobowiązany do ich natychmiastowego zgłoszenia.
    ANSWER:
    jednostka organiczna, człowiek, Aleksander Ragowski, skan biometryczny, nauczyciel, nauczyciel języka angielskiego, buntownik, krytyk władzy, programista, pojmanie, patrol, zatrzymanie, potwierdzenie tożsamości
    </przykład>

    """;
}
