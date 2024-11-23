namespace S02E04;

internal static class Prompts
{
    public const string SystemExtractNoteText = """
                                                You are an expert note extractor.
                                                Extract the note text from the image.
                                                Focus only on the note text, ignore any other text in the image such as the title, author and the signature.
                                                ONLY return the note text.
                                                """;

    public const string UserExtractNoteText = """
                                              Extract the note text from the image.
                                              """;

    public const string Categorize = """
                                     Categorize the given message into one of three categories: "people," "hardware," or "other."

                                     # Instructions
                                     - Analyze the message carefully to determine its primary focus.
                                     - Consider if it relates to:
                                     - **People**: Any information regarding a person or group of people being captured or finding evidence of their presence.
                                     - **Hardware**: Any indication of a malfunction, damage, or issue related to equipment, technology, or mechanical components.
                                     - **Other**: Any message that doesn't fit under "people" or "hardware."

                                     # Output Format
                                     - Respond using only one word: either "people," "hardware," or "other."

                                     # Examples
                                     **Input**: "The tracker showed several individuals near the border yesterday."
                                     **Output**: "people"

                                     **Input**: "The drone's camera has stopped working due to a hardware fault."
                                     **Output**: "hardware"

                                     **Input**: "The weather at the outpost looks really harsh today, we might experience delays."
                                     **Output**: "other"

                                     # Notes
                                     - Make sure to stick to one of the three given categories without additional text or formatting.
                                     - Use logical reasoning to determine the category before responding.
                                     """;
}
