# nullable enable

using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

public class Scorer : MonoBehaviour
{
    public GroqApiClient groqApi = new GroqApiClient();

    private static readonly JObject systemPrompt = new JObject
    {
        ["role"] = "system",
        ["content"] = "As a proficient english grader. You will grade the english proficiency of the user and provide tips on how to improve their english. You will grade their english by analyzing the previous messages."
    };
    private string chatLogFilePath;

    private void Start()
    {
        chatLogFilePath = Path.Combine(Application.persistentDataPath, "chat_log.txt");
    }

    private JArray GetUserMsgs()
    {
        JArray userMsgs = new();

        string chatLogFilePath = Application.persistentDataPath + "/chatLogs.txt";

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

        return userMsgs;
    }

    public async Task OutputScore()
    {

        JObject request = new JObject
        {
            ["model"] = "llama-3.1-8b-instant",
            ["messages"] = GetUserMsgs(),
            ["max_tokens"] = 100,
            ["temperature"] = 1.2
        };

        JObject? response = await groqApi.CreateChatCompletionAsync(request);
        var content = response?["choices"]?[0]?["message"]?["content"];
        if (content == null)
        {
            Debug.LogError("Error: response content is null");
            return;
        }

        Debug.Log("Scorer: " + content);
    }
}
