using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Mini AI Chatbot ===");

        string endpoint = "https://YOUR-ENDPOINT.openai.azure.com/";
        string apiKey = "YOUR_API_KEY";
        string deployment = "gpt-4o-mini";

        while (true)
        {
            Console.Write("You: ");
            string userInput = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(userInput)) break;

            string response = await AskOpenAI(endpoint, apiKey, deployment, userInput);
            Console.WriteLine($"Bot: {response}\n");
        }
    }

    static async Task<string> AskOpenAI(string endpoint, string apiKey, string deployment, string question)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("api-key", apiKey);

        var requestBody = new
        {
            messages = new[] { new { role = "user", content = question } }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var response = await client.PostAsync(
            $"{endpoint}openai/deployments/{deployment}/chat/completions?api-version=2024-02-15-preview",
            new StringContent(json, Encoding.UTF8, "application/json")
        );

        string result = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(result);

        return doc.RootElement
                  .GetProperty("choices")[0]
                  .GetProperty("message")
                  .GetProperty("content")
                  .GetString();
    }
}
