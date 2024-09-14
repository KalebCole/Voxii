# nullable enable

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

public class Scorer
{
    public GroqApiClient groqApi = new GroqApiClient();

    private static readonly JObject systemPrompt = new JObject
    {
        ["role"] = "system",
        ["content"] = "As a proficient english grader. You will grade the english proficiency of the user and provide tips on how to improve their english. You will grade their english by analyzing the previous messages."
    };
    private string chatLogFilePath;

    public Scorer(string chatLogFilePath)
    {
        this.chatLogFilePath = chatLogFilePath;
    }

    private JArray GetUserMsgs()
    {
        // Initialize this with system prompt
        JArray userMsgs = new JArray
        {
            systemPrompt
        };

        if (File.Exists(chatLogFilePath))
        {
            string[] messages = File.ReadAllLines(chatLogFilePath);

            foreach (string message in messages)
            {
                JObject userMessage = new()
                {
                    ["role"] = "user",
                    ["content"] = message
                };

                userMsgs.Add(userMessage);
            }
        }

        // userMsgs.Add(new JObject
        // {
        //     ["role"] = "user",
        //     ["content"] = "Can you grade my english please?"
        // });

        return userMsgs;
    }

    public async Task<string> GetScore()
    {
        JObject request = new JObject
        {
            ["model"] = "llama-3.1-8b-instant",
            ["messages"] = GetUserMsgs(),
            ["max_tokens"] = 100,
            ["temperature"] = 1.2
        };

        // log the request
        File.AppendAllText(chatLogFilePath, request.ToString());

        JObject? response = await groqApi.CreateChatCompletionAsync(request);
        var content = response?["choices"]?[0]?["message"]?["content"];

        return content?.ToString();
    }
}
