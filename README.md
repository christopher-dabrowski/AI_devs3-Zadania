# Spis Treści

- [Spis Treści](#spis-treści)
- [AI_devs3-Zadania](#ai_devs3-zadania)
  - [PreworkApi](#preworkapi)
  - [S01E01](#s01e01)
  - [S01E02](#s01e02)
  - [S01E03](#s01e03)
  - [S01E04](#s01e04)
  - [S01E05](#s01e05)
  - [S02E01](#s02e01)

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

## S01E01

Zadanie polegało na automatyzacji procesu logowania z użyciem AI do rozwiązywania pytań ANTY CAPTCHA - to jak CAPTCHA, ale może je przejść tylko robot 🤖.

Moje rozwiązanie: [S01E01](./dotnet/S01E01/Program.cs)

W implementacji wykorzystałem:

- HtmlAgilityPack do parsowania strony i wydobycia pytania
- OpenAI API (model GPT-4) do analizy pytania i generowania odpowiedzi
- HttpClient do komunikacji z API
- Dependency Injection do zarządzania zależnościami
- User Secrets do bezpiecznego przechowywania kluczy API i danych logowania

Ciekawe było wykorzystanie System Message w OpenAI API, który wymusza zwracanie wyłącznie liczby, bez dodatkowych wyjaśnień. To pokazuje, jak ważny jest odpowiedni prompt engineering w pracy z AI.

## S01E02

Zadanie polegało na wykorzystaniu modelu językowego do przetwarzania i analizy wiadomości w dwóch krokach.

Moje rozwiązanie: [S01E02](./dotnet/S01E02/Program.cs)

W implementacji zastosowałem ciekawe podejście wykorzystujące sekwencyjne połączenie dwóch promptów, gdzie:

- Pierwszy prompt przygotowywał dane dla drugiego
- Model efektywnie pracował z własnymi wynikami
- Większość kodu została wygenerowana przez Cursor IDE, co pokazuje potencjał AI w codziennej pracy programisty

To doświadczenie pokazało, jak skuteczne może być łączenie różnych technik prompt engineeringu oraz wykorzystanie narzędzi AI do wspomagania procesu programowania.

## S01E03

Zadanie polegało na analizie dużego pliku z pytaniami i poprawienie w nim błędów.

Moje rozwiązanie: [S01E03](./dotnet/S01E03/Program.cs)

Głównym wyzwaniem była wielkość pliku wejściowego — zbyt duża, by przekazać całość do modelu językowego.
Zastosowałem więc podejście hybrydowe:

- Obliczenia matematyczne wykonałem programistycznie, bez użycia LLM
- Model językowy wykorzystałem tylko do analizy pytań i generowania odpowiedzi na podstawie wcześniej obliczonych wyników

To pokazuje, jak ważne jest odpowiednie rozdzielenie zadań między tradycyjne programowanie a AI, szczególnie przy ograniczeniach technicznych modeli językowych.

## S01E04

Zadanie polegało na napisaniu promptu, który nakieruje robota do celu, omijając przeszkody.
Wykorzystanie do tego LLM okazało się zaskakująco trudne.
Na początku próbowałem zrobić to bez wyznaczania konkretnej trasy modelowi, jednak bez skutku.
Na razie roziwąwiązałem to zadanie w łatwiejszej wersji, gdzie model miał podążać za z góry ustaloną trasą.
Moje rozwiązanie to [definedTrackSolution](prompty/S01E04/definedTrackSolution.txt).

## S01E05

Jeszcze nie zrobiłem, ale jeszcze tu wrócę ^^

## S02E01

Zadanie polegało na wygenerowaniu transkrypcji z plików audio z zeznaniami świadków oraz wykorzystanie modelu do przeanalizowania ich w celu ustalenia pewnego konkretnego adresu.

Tym razem dla odmiany zdecydowałem się użyć pythona.
Generowanie transkrypcji: [S02E01](python/S02E01/transcribe.py)  
Podczas pracy z modelem whisper w pythonie miałem kilka kłopotów. Najpierw ze zbyt nową wersją pythona, a potem z wersją numpy.
Już od paru lat nie używałem pythona, więc znalezienie rozwiązania mogłoby mi zając sporo czasu.
Cursor na szczęście niezwykle sprawnie podsunął mi gotowe rozwiązania :D

Mając gotowe transkrypcje również przy pomocy Cursor'a przeanalizowałem je i znalazłem odowiedź.  
Mój prompt: [prompt.txt](python/S02E01/prompt.txt)

Co ciekawe najpierw uruchmiłem go na modelu gpt-4o. Model ten dał znacznie gorszy wynik niż claude-3.5-sonnet.
