# nullable enable

using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.IO;

public class Scorer
{
    public GroqApiClient groqApi = new GroqApiClient();

    private JObject systemPrompt // this dynamically generates upon instantiation of the scorer class (so we can interpolate the chat log file)
    {
        get
        {
            return new JObject
            {
                ["role"] = "system",
                ["content"] = $@"
                  You are proficient in the English Language 


                        I want you to grade the user in a conversation between a user and an assistant based on the following criteria: 
                        1. Number of errors in the user's grammar
                        2. Average time for the response in seconds
                        3. Analysis of the relevance of the user response to gauge if they understood what the assistant was asking

                        Here is the conversation:
                        

                "
            };
        }
    }

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

        JObject? response = await groqApi.CreateChatCompletionAsync(request);
        var content = response?["choices"]?[0]?["message"]?["content"];

        return content?.ToString();
    }
}
