namespace S03E04;

public static class Prompts
{
    public const string ExtractNamesAndCitiesSystemPrompt =
    """
    Extract names and city names from the given message in their nominative form.

    # Steps

    1. **Identify Proper Nouns**: Analyze the message to locate potential names and city names.
    2. **Categorize as Names or Cities**: Distinguish between personal names and city names to categorize them correctly.
    3. **Convert to Nominative Case**: Ensure all identified names and city names are in the nominative case.
    4. **Compile JSON Output**: Construct a JSON object with two keys: "names" and "cities". Each key should have a list as its value containing the extracted and categorized names.

    # Output Format

    Produce a JSON object with the following structure:
    {
    "names": ["List of identified names in nominative form"],
    "cities": ["List of identified city names in nominative form"]
    }

    # Examples

    **Input**: 
    "Wczoraj spotkałem Annę i Michała z Warszawy i Poznania."

    **Output**:
    {
        "names": ["Anna", "Michał"],
        "cities": ["Warszawa", "Poznań"]
    }

    # Notes

    - Focus on proper nouns that are likely to be names or city names.
    - Ensure correctness of the nominative form, as this may affect the recognition of names and cities.
    - For names list only the first names. Don't return surnames.
    - Return only JSON without any additional formatting
    """;
}
