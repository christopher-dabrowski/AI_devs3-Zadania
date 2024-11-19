namespace S02E03.Models;

public static class Prompts
{
    public const string ExtractRobotAppearance =
        """
        Extract and summarize only the physical appearance characteristics of the robot from the given description.
        Focus on visual aspects like color, size, shape, materials, and components.
        Provide the response in a clear, concise format, such that it can be used to generate a prompt for a text-to-image model.

        Focus only on the robot's appearance, not any other parts of the message.
        """;

    public const string ConvertToImageGenerationPrompt =
        """
        Generate a DALL-E-3 prompt that will generate an image of the described robot.
        """;
}
