using Mscc.GenerativeAI;
using System.Text.Json;
using CVision.BLL.DTOs.CvAnalyses;
using CVision.BLL.Interfaces;

namespace CVision.BLL.Services;

public class GeminiService : IAIService
{
    private readonly GenerativeModel _model;

    public GeminiService(string apiKey)
    {
        var googleAi = new GoogleAI(apiKey);

        _model = googleAi.GenerativeModel("gemini-2.5-flash");
    }

    public async Task<CvAnalysisResultDto> AnalyzeResumeAsync(string rawText)
    {
        string prompt = $@"
            Ти — професійний HR-експерт та рекрутер. Твоє завдання — провести глибокий аудит резюме.
            Проаналізуй текст резюме нижче та поверни результат СУВОРО у форматі JSON. Текст нижче отримано через OCR
            і він може містити помилки розпізнавання, змішані колонки або дивні символи.
            Будь ласка, спочатку очисти текст, віднови структуру логічно і тільки потім аналізуй.
            
            Структура JSON:
            {{
                ""FeedBack"": ""Загальний професійний висновок про кандидата та структуру його CV, також проаналізуй на наявність граматичних помилок."",
                ""Score"": 85,
                ""SectionsResults"": [
                    {{
                        ""Title"": ""Назва секції (наприклад: Контакти, Досвід, Навички, Освіта)"",
                        ""Content"": ""Детальний відгук: що заповнено добре, а що варто додати чи змінити"",
                        ""SectionScore"": 90
                    }}
                ]
            }}

            Оцінюй критично. Якщо секція порожня або слабка — став низький бал.
            Текст резюме для аналізу:
            ---
            {rawText}
            ---";

        var response = await _model.GenerateContent(prompt);
        string? jsonResponse = response.Text;

        if (string.IsNullOrEmpty(jsonResponse))
        {
            throw new Exception("AI відмовився аналізувати це резюме.");
        }

        string cleanJson = jsonResponse.Replace("```json", string.Empty).Replace("```", string.Empty).Trim();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<CvAnalysisResultDto>(cleanJson, options);

        return result ?? new CvAnalysisResultDto();
    }
}