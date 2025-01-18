namespace S04E04;

public static class Prompts
{
    public const string DeduceFlightDestination =
    """
    Identify the final destination of a drone flight on a 4x4 game map, based on a given description in Polish.

    You will receive a description of the drone's flight, which always starts at the top left corner of the map. The map layout is as follows:
    - Row 1: start location | field | field with a tree | house
    - Row 2: field | windmill | field | field
    - Row 3: field | field | rocks | two trees
    - Row 4: mountains | mountains | car | cave

    Process the description to determine the final destination where the flight ends, and provide the result in JSON format. Include a "thinking" section that explains the reasoning behind determining the final position, and an "answer" section that provides a description of the final destination in Polish using a maximum of two words.

    # Steps

    1. Start at the initial position (top left corner).
    2. Read and interpret the flight instructions provided in Polish to determine the movement on the map.
    3. Track the position on the map according to the instructions.
    4. Identify the final position on the map where the drone flight ends.
    5. Document the reasoning process in the "thinking" section.
    6. Provide a Polish description (maximum two words) of the final destination in the "answer" section.

    # Output Format

    - JSON with two fields:
    - "thinking": A detailed step-by-step explanation of the interpretation of flight instructions and the determination of the final destination.
    - "description": A concise (up to two words) Polish description of the final destination.

    # Examples

    ** Example 1**

    Input: "Lecimy kolego teraz na sam dół mapy, a później ile tylko możemy polecimy w prawo. Teraz mała korekta o jedno pole do góry. Co my tam mamy?"

    Output:
    {
    "thinking": "Zaczynamy od punktu startowego, który jest w lewym górnym rogu, czyli w pierwszym wierszu i pierwszej kolumnie. Instrukcja mówi, aby polecieć na sam dół mapy, co przesuwa nas z rzędu 1 do rzędu 4. Jesteśmy w pozycji 4 wiersz 1 kolumna, na polu z górami. Następna instrukcja jest, aby polecieć jak najdalej w prawo, co przesuwa nas z kolumny 1 do kolumny 4 w tym samym rzędzie. Jesteśmy w pozycji 4 wiersz 4 kolumna, lądując na polu z jaskiną. Kolejna instrukcja to ruch o jedno pole do góry, co przesuwa nas z rzędu 4 do rzędu 3, pozostając w kolumnie 4. Jesteśmy na polu 3 wiersz 4 kolumna, gdzie znajdują się dwa drzewa.",
    "description": "dwa drzewa"
    }

    # Notes

    - Ensure the solution accounts for each specific instruction pattern given in Polish.
    - Use cardinal directions (right/left or up/down) based on the description for clarity.
    - Verify movement does not exceed the 4x4 boundary.
    - Return only the requested JSON. Don't add any additional formatting
    - If the instruction doesn't specify how long should the movement be, assume it's a movement by one square
    """;
}
