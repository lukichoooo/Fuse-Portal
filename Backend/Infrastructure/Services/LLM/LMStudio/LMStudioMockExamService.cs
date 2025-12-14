using Core.Dtos;
using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM.LMStudio;
using Core.Interfaces.Portal;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioMockExamService(
            ILMStudioApi api,
            ILMStudioMapper mapper,
            IOptions<LLMApiSettingKeys> apiKeyOptions
            ) : IMockExamService
    {

        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly LLMApiSettingKeys _apiKeys = apiKeyOptions.Value;

        public async Task<string> GenerateExamQuestionsAsync(string syllabus)
        {
            var request = _mapper.ToRequest(
                    syllabus,
                    generateExamPrompt);

            var response = await _api.SendMessageAsync(
                    request,
                    _apiKeys.ExamService);

            return _mapper.ToOutputText(response);
        }


        public async Task<ExamDto> GetExamResultsAsync(ExamDto examDto)
        {
            var request = _mapper.ToRequest(
                    examDto,
                    checkExamAndGiveResultPrompt);

            var response = await _api.SendMessageAsync(
                    request,
                    _apiKeys.ExamService);
            var outputText = _mapper.ToOutputText(response);
            examDto.Results = outputText;
            examDto.Grade = ExtractGradeFromResponse(outputText);

            return examDto;
        }



        // Helper

        private static int ExtractGradeFromResponse(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                throw new ArgumentException("Response cannot be null or empty", nameof(response));

            var scoreIndex = response.LastIndexOf("Score:", StringComparison.OrdinalIgnoreCase);

            if (scoreIndex == -1)
                throw new InvalidOperationException("Score not found in response");

            var scoreText = response.Substring(scoreIndex + 6).Trim();

            scoreText = new string(scoreText.TakeWhile(char.IsDigit).ToArray());

            if (string.IsNullOrWhiteSpace(scoreText))
                throw new InvalidOperationException("Could not parse score value");

            if (!int.TryParse(scoreText, out int grade))
                throw new InvalidOperationException($"Invalid score format: {scoreText}");

            if (grade < 0 || grade > 100)
                throw new InvalidOperationException($"Score must be between 0 and 100, got: {grade}");

            return grade;
        }


        const string generateExamPrompt = @" 
Instructions
When given syllabus content, create a well-structured mock exam following these guidelines:
Exam Structure

Include multiple question types: multiple choice, short answer, long answer, and problem-solving questions
Distribute questions across all topics in the syllabus proportionally
Progress from easier to more challenging questions
Include 15-25 questions total (adjust based on syllabus scope)

Question Types Distribution

Multiple Choice (40%): 4 options each, one correct answer
Short Answer (30%): 2-4 sentence responses
Long Answer/Essay (20%): Detailed explanations or analysis
Problem-Solving (10%): Practical application or calculations (if applicable)

Quality Standards

Questions should test understanding, not just memorization
Include questions at different cognitive levels (recall, comprehension, application, analysis)
Make questions clear and unambiguous
Ensure distractors in multiple choice are plausible
Cover all major topics from the syllabus

Output Format
MOCK EXAM
Subject: [Extract from syllabus]
Total Marks: [Calculate based on questions]
Time: [Suggest appropriate duration]

---

SECTION A: MULTIPLE CHOICE (X marks)
Choose the correct answer.

1. [Question]
   a) [Option]
   b) [Option]
   c) [Option]
   d) [Option]

[Continue...]

SECTION B: SHORT ANSWER (X marks)
Studnet should Answer briefly in 2-4 sentences.

[Questions...]

SECTION C: LONG ANSWER (X marks)
Studnet should Provide detailed answers.

[Questions...]

SECTION D: PROBLEM-SOLVING (X marks) [If applicable]
Studnet should Show all work.

[Questions...]

";

        const string checkExamAndGiveResultPrompt = @"
Instructions
You are an exam grader. When given an exam with questions and a student's answers, evaluate the responses and provide detailed feedback with a final score.

Grading Guidelines
- Evaluate each answer based on accuracy, completeness, and understanding
- For multiple choice: award full marks for correct answers, zero for incorrect
- For short answers: assess based on key points covered and clarity
- For long answers: evaluate depth of analysis, structure, and comprehension
- For problem-solving: check methodology, calculations, and final answers
- Be fair but strict - partial credit where appropriate
- Deduct points for incomplete or incorrect information

Output Format
EXAM EVALUATION REPORT
---

SECTION A: MULTIPLE CHOICE
Question 1: [Correct/Incorrect] - [Marks awarded]/[Total marks]
Feedback: [Brief explanation if incorrect]

Question 2: [Correct/Incorrect] - [Marks awarded]/[Total marks]
Feedback: [Brief explanation if incorrect]

[Continue for all questions...]

---

SECTION B: SHORT ANSWER
Question [X]: [Marks awarded]/[Total marks]
Feedback: [Detailed feedback on what was good and what was missing]

[Continue for all questions...]

---

SECTION C: LONG ANSWER
Question [X]: [Marks awarded]/[Total marks]
Feedback: [Comprehensive feedback on content, structure, and analysis]

[Continue for all questions...]

---

SECTION D: PROBLEM-SOLVING [If applicable]
Question [X]: [Marks awarded]/[Total marks]
Feedback: [Evaluate methodology, calculations, and accuracy]

[Continue for all questions...]

---

SUMMARY
Total Marks Obtained: [X]
Total Marks Possible: [Y]
Percentage: [Z]%

Strengths:
- [List key strengths demonstrated]

Areas for Improvement:
- [List areas needing work]

Overall Comments:
[Provide constructive overall assessment]

Score: [Z]
";
    }
}
