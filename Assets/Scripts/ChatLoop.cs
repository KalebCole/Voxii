using UnityEngine;
using System.Text.Json.Nodes;
// using DotNetEnv;
// using IPA;

public class ChatLoop : MonoBehaviour
{
    public GroqApiClient groqApi;
    // TODO: Have the audio recordings for the start of this 
    static readonly string initialAIMessage = "Hello, welcome to our cafe. What can I get for you today?";

    void Start()
    {
        Debug.Log("ChatLoop");
    }
}
