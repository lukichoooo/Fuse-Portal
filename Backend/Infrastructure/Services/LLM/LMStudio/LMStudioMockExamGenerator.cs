using Core.Dtos.Settings.Infrastructure;
using Core.Interfaces.LLM.LMStudio;
using Core.Interfaces.Portal;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.LLM.LMStudio
{
    public class LMStudioMockExamGenerator(
            ILMStudioApi api,
            ILMStudioMapper mapper,
            IOptions<LLMApiSettingKeys> apiKeyOptions
            ) : IMockExamCreator
    {

        private readonly ILMStudioApi _api = api;
        private readonly ILMStudioMapper _mapper = mapper;
        private readonly LLMApiSettingKeys _apiKeys = apiKeyOptions.Value;

        // TODO: write in another file
        const string rulesPrompt = @" 
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

        public async Task<string> GenerateExamAsync(string syllabi)
        {
            var request = _mapper.ToRequest(
                    syllabi,
                    rulesPrompt);

            var response = await _api.SendMessageAsync(
                    request,
                    _apiKeys.ExamGenerator);

            return _mapper.ToOutputText(response);
        }
    }
}
