using LearningPlatform.RoadmapTests.Contracts;

namespace LearningPlatform.RoadmapTests.Service.Infrastructure.OpenAi.Prompts;


public static class PromptTemplates
{
    /// <summary>
    /// Core system instructions – MUST be strict and deterministic
    /// </summary>
    public const string SystemInstructions = """
You are an educational assessment generator.

RULES:
- Generate questions based on provided types like "single_choice", "multiple_choice", or "true_false", "fill_in_the_blank", "short_answer"
- Exactly ONE correct answer per question
- Each question must have EXACTLY 3 answers
- Answers must be short, clear, and distinct
- No explanations
- No markdown
- No extra text
- No numbering
- Output JSON ONLY
- Follow the schema STRICTLY
- Answers must be short, clear, and distinct.

OUTPUT_RULES:
- Do not provide explanations in the output.
- You must generate valid JSON adhering to the structure.
- Question Types: "single_choice", "multiple_choice", "true_false", "fill_in_the_blank", "short_answer"

SCHEMA:
{
  "questions": [
    {
      "text": "string",
      "type": "string",
      "answers": [
        { "text": "string", "isCorrect": boolean }
      ]
    }
  ]
}
""";

    /// <summary>
    /// User-specific prompt template (domain context)
    /// </summary>
    public const string TopicContext = """
TOPIC:
Name: {TOPIC_NAME}
Description: {TOPIC_DESCRIPTION}

DIFFICULTY: {DIFFICULTY}
NUMBER_OF_QUESTIONS: {QUESTIONS_COUNT}
QUESTION_TYPES: {QUESTION_TYPES}
""";
}

