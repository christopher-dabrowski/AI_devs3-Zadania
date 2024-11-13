# Spis Treści

- [Spis Treści](#spis-treści)
- [AI_devs3-Zadania](#ai_devs3-zadania)
  - [PreworkApi](#preworkapi)
  - [S01E01 — Interakcja z dużym modelem językowym](#s01e01--interakcja-z-dużym-modelem-językowym)
  - [S01E02 — Przygotowanie własnych danych dla modelu](#s01e02--przygotowanie-własnych-danych-dla-modelu)
  - [S01E03 — Limity Dużych Modeli językowych i API](#s01e03--limity-dużych-modeli-językowych-i-api)
  - [S01E04 — Techniki optymalizacji](#s01e04--techniki-optymalizacji)
  - [S01E05 — Produkcja](#s01e05--produkcja)
  - [S02E01 — Audio i interfejs głosowy](#s02e01--audio-i-interfejs-głosowy)

# AI_devs3-Zadania

[![.NET Build](https://github.com/christopher-dabrowski/AI_devs3-Zadania/actions/workflows/dotnet-build.yml/badge.svg?branch=main)](https://github.com/christopher-dabrowski/AI_devs3-Zadania/actions/workflows/dotnet-build.yml)

Moje rozwiązania do zadań z kursu AI_devs3.  
Gdy spisuję moje rozwiązania i przemyślenia to więcej wynoszę z nauki 😃

## PreworkApi

Zadanie polegało na przetestowaniu komunikacji z API, którego będziemy używać w dalszych zadaniach.  
Była do dla mnie świetna okazja, żeby nauczyć się korzystać z [Cursor IDE](https://www.cursor.com/).

Moje rozwiązanie: [PreworkApi](./dotnet/PreworkApi/Program.cs)

Jestem pod wrażeniem, jak przyjemnie programowało mi się z pomocą AI.
Do tej pory używałem trochę GitHub Copilot, ale nie byłem pod jakimś dużym wrażeniem.
Teraz gdy użyłem znacznie bardziej zintegrowanego z AI narzędzia, czułem się bardzo fajnie.
AI sprawdziło się bardzo dobrze, może dlatego, że zadanie było dość prostsze, jednak zawsze w pracy są też prostsze czynności, które często są nudne.
Chciałbym częściej używać do nich AI.

## S01E01 — Interakcja z dużym modelem językowym

Zadanie polegało na automatyzacji procesu logowania z użyciem AI do rozwiązywania pytań ANTY CAPTCHA - to jak CAPTCHA, ale może je przejść tylko robot 🤖.

Moje rozwiązanie: [S01E01](./dotnet/S01E01/Program.cs)

W implementacji wykorzystałem:

- HtmlAgilityPack do parsowania strony i wydobycia pytania
- OpenAI API (model GPT-4) do analizy pytania i generowania odpowiedzi
- HttpClient do komunikacji z API
- Dependency Injection do zarządzania zależnościami
- User Secrets do bezpiecznego przechowywania kluczy API i danych logowania

Ciekawe było wykorzystanie System Message w OpenAI API, który wymusza zwracanie wyłącznie liczby, bez dodatkowych wyjaśnień. To pokazuje, jak ważny jest odpowiedni prompt engineering w pracy z AI.

## S01E02 — Przygotowanie własnych danych dla modelu

Zadanie polegało na wykorzystaniu modelu językowego do przetwarzania i analizy wiadomości w dwóch krokach.

Moje rozwiązanie: [S01E02](./dotnet/S01E02/Program.cs)

W implementacji zastosowałem ciekawe podejście wykorzystujące sekwencyjne połączenie dwóch promptów, gdzie:

- Pierwszy prompt przygotowywał dane dla drugiego
- Model efektywnie pracował z własnymi wynikami
- Większość kodu została wygenerowana przez Cursor IDE, co pokazuje potencjał AI w codziennej pracy programisty

To doświadczenie pokazało, jak skuteczne może być łączenie różnych technik prompt engineeringu oraz wykorzystanie narzędzi AI do wspomagania procesu programowania.

## S01E03 — Limity Dużych Modeli językowych i API

Zadanie polegało na analizie dużego pliku z pytaniami i poprawienie w nim błędów.

Moje rozwiązanie: [S01E03](./dotnet/S01E03/Program.cs)

Głównym wyzwaniem była wielkość pliku wejściowego — zbyt duża, by przekazać całość do modelu językowego.
Zastosowałem więc podejście hybrydowe:

- Obliczenia matematyczne wykonałem programistycznie, bez użycia LLM
- Model językowy wykorzystałem tylko do analizy pytań i generowania odpowiedzi na podstawie wcześniej obliczonych wyników

To pokazuje, jak ważne jest odpowiednie rozdzielenie zadań między tradycyjne programowanie a AI, szczególnie przy ograniczeniach technicznych modeli językowych.

## S01E04 — Techniki optymalizacji

Zadanie polegało na napisaniu promptu, który nakieruje robota do celu, omijając przeszkody.
Wykorzystanie do tego LLM okazało się zaskakująco trudne.
Na początku próbowałem zrobić to bez wyznaczania konkretnej trasy modelowi, jednak bez skutku.
Na razie roziwąwiązałem to zadanie w łatwiejszej wersji, gdzie model miał podążać za z góry ustaloną trasą.
Moje rozwiązanie to [definedTrackSolution](prompty/S01E04/definedTrackSolution.txt).

## S01E05 — Produkcja

W tym zadaniu przetwarzam pliki przy pomocy lokalnie uruchomionego modelu.

Uruchomiłem w [Dockerze](https://www.docker.com/) narzędzie [ollama](https://ollama.com/).  
Najpierw w terminalu pobawiłem się modelem llama31 w wersji 8b.
Wygenerowałem sobie plan treningowy na siłownię 💪  
Trochę to trwało, prawdopodobnie dlatego, że mój laptop nie ma dedykownego GPU.

Jeszcze nie zrobiłem, ale jeszcze tu wrócę ^^

## S02E01 — Audio i interfejs głosowy

Zadanie polegało na wygenerowaniu transkrypcji z plików audio z zeznaniami świadków oraz wykorzystanie modelu do przeanalizowania ich w celu ustalenia pewnego konkretnego adresu.

Tym razem dla odmiany zdecydowałem się użyć pythona.
Generowanie transkrypcji: [S02E01](python/S02E01/transcribe.py)  
Podczas pracy z modelem whisper w pythonie miałem kilka kłopotów. Najpierw ze zbyt nową wersją pythona, a potem z wersją numpy.
Już od paru lat nie używałem pythona, więc znalezienie rozwiązania mogłoby mi zając sporo czasu.
Cursor na szczęście niezwykle sprawnie podsunął mi gotowe rozwiązania :D

Mając gotowe transkrypcje również przy pomocy Cursor'a przeanalizowałem je i znalazłem odowiedź.  
Mój prompt: [prompt.txt](python/S02E01/prompt.txt)

Co ciekawe najpierw uruchmiłem go na modelu gpt-4o. Model ten dał znacznie gorszy wynik niż claude-3.5-sonnet.
