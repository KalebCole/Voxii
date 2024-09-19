# Voxii - VR Language Learning App

An immersive virtual reality language learning windows desktop application. By simulating common real-world scenarios like ordering coffee in a café, Voxii allows users to learn and practice a new language through realistic, conversational interactions. View your score and error corrections any time during the conversation by a push of a button!

## Features

- **Life-like scenes**: Immerse yourself in a different life-like scene to practice speaking
- **Immersive real-life scenarios**: Practice in a common scenario to hone your english speaking skills
- **Speech recognition and generation**: Speak directly into your microphone and the Voxii avatar will respond back, similar to real life
- **Avatar corrections**: If mistakes are made, the avatar politely corrects you
- **Score and corrections**: Get immediate feedback and improvement tips, along with your score for the conversation you've had so far, with the click of a button

## Getting started

1. Launch the game and hold the controller's primary button to start speaking
2. Listen to the response the AI gives, and continue conversing using step 1.
3. Press the controller's secondary button to get feedback on the conversation so far

## Documentation

Unity version - 2022.3.39f1

SDK version - .NET Framework

Note that only public methods are included here

### XRInputActions.inputactions
- Contains the mappings which map from the keyboard to the controller's controls

### AIVoice.cs
- Text to speech for the AI avatar

- `static async Task Speak(string msg)`
  - Asynchronously outputs AI speech for the given message using Piper TTS
 
- `static async Task SpeakRepeat()`
  - Outputs the "Can you repeat that" message audio
 
- `static async Task SpeakInitialMsg()`
  - Outputs the initial message for the specific avatar

 ### ChatLoop.cs
 - The user-avatar conversation loop

 - `async Task SendUserMessage(string msg)`
   - Sends the given message to the AI avatar, adds it to the logs and chat history

### GameController.cs
- Handles game related logic

- `void StartLoading()`
  - Initializes the loading icon logic
 
### LoadingSymbolController.cs
- Handles the loading symbol logic

- `void ShowLoadingSymbol()`
  - Shows the loading symbol
 
- `void HideLoadingSymbol()`
  - Hides the loading symbol

 ### GroqApiClient.cs
 - Handles the requests sent to Groq's llama3

 ` `GroqApiClient(string apiKey = "")`
   - Initializes the groq api client with the API key
   - If no API key is provided it will try searching for one in a `.env` file

 - `async Task<JObject?> CreateChatCompletionAsync(JObject request)`
   - The request to send to the AI, including the parameters and the chat history

### MicRecorder.cs
- Handles recording through the microphone

- `void StartRecording()`
  - Starts recording the microphone

- `void StopRecording()`
  - Stops recording the microphone
 
- `void SaveRecording()`
  - Saves the recording in the persistent data path as "recording.wav"
 
- `void PlayRecording()`
  - Plays the recording
 
### OnboardingData.cs
- Data container class to hold onboarding data instances

### PrimaryBtnHold.cs
- Logic for what should happen when the primary button is held

### SecondaryBtnPress.cs
- Logic for what should happen when the secondary button is pressed

### SaveWavFile.cs (static class - WavUtility)
- Contains logic to save an audio clip to a location in the `.wav` format

- `static byte[] FromAudioClip(AudioClip clip)`
  - Converts an audio clip into byte[]
 
- `static void SaveWav(string filePath, AudioClip clip)`
  - Saves the audio clip as a `.wav` file into the specified path
 
### Scorer.cs
- Contains the AI scoring logic for the user's english

- `Scorer(string chatLogFilePath)`
  - Initializes the scorer with the chat logs of the conversation so far
 
- `async Task<string> GetScore()`
  - Gets the score on the user's english of the conversation
 
### WhisperTranscriber.cs
- Contains the logic for transcribing the user's speech to text

- `async Task<string> TranscribeRecording()`
  - Gets the string for the user's speech stored in `recording.wav` in the application's persistent data path
 
