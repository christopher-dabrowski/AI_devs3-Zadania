namespace S02E02;

public static class Prompts
{
    public const string DescribeMapSystemPrompt = """
        You are a precise geographical location analyzer. Your task is to analyze map images and provide structured, detailed descriptions of the location's key characteristics that could help identify the specific city.
        
        Focus on:
        - Major road patterns and infrastructure
        - Urban layout and density patterns
        - Notable landmarks or districts
        - Spatial relationships between key elements
        
        Provide your analysis in a structured format that highlights unique identifying features of the location.
        """;

    public const string DescribeMapUserPrompt = """
        Analyze this map image

        Format your response to emphasize features that could help identify this specific city.
        """;

    public const string DetectCitySystemPrompt = """
        You are a city detection assistant. Your task is to analyze a set of map descriptions and determine to which city they all belong.
        - The city is in Poland
        - There are granaries in the city
        - There are fortifications in the city
        """;
}
