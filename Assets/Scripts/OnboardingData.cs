using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Usage:
/// // Both of these will check if the profile exists (if no profile is provided the value defaults to "default") and load the data from the file, otherwise it will create a new one with the default values
/// OnboardingData data = new OnboardingData("John");
/// **OR**
/// OnboardingData data = new OnboardingData();
/// **OR**
/// // You can load the data asynchronously
/// OnboardingData dataAsync = new OnboardingData("John", true);
/// 
/// // Updating the data
/// data.Update("Beginner", "English", new List<string> { "Greeting and Introduction", "Ordering Food and Drinks" }, "Cafe", new Dictionary<string, string> { { "Cafe", "Barista" } });
/// 
/// // Saving the data to the file
/// data.Save();
/// **OR**
/// await data.SaveAsync(); // If you want to quickly continue while saving the data in the background
/// </summary>
public class OnboardingData : MonoBehaviour
{
    [JsonProperty]
    public string PersonName { get; set; }
    [JsonProperty]
    public int LanguageProficiency { get; set; }
    [JsonProperty]
    public int HostilityLevel { get; set; }
    [JsonProperty]
    public string LanguageToLearn { get; set; }
    [JsonProperty]
    public List<string> PhrasesToWorkOn { get; set; } = new List<string>();
    [JsonProperty]
    public string Scene { get; set; }
    [JsonProperty]
    public Dictionary<string, string> SceneToRole { get; set; }

    private readonly string filePath;
    private readonly SemaphoreSlim semaphore = new(1, 1);

    public OnboardingData(string personName = null, bool async = false)
    {
        personName = string.IsNullOrEmpty(personName) ? "default" : personName;

        PersonName = personName;

        // If personName exists, load the data from the file
        // If it doesn't, create a new one with the default values
        string fileName = personName + " - profile.json";
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            // TODO : change back
            /*
            if (File.Exists(filePath))
            {
                string jsonStr = File.ReadAllText(filePath);
                JsonConvert.PopulateObject(jsonStr, this);
            }
            else
            {
            */
            Debug.Log("Initilaising OnboardingData");
                InitializeDefault();

                if (!async)
                {
                    Save();
                }
                else
                {
                    _ = SaveAsync(); // _ is used to ignore the Task object
                }
            //}
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading OnboardingData for {PersonName}: {e.Message}");
            InitializeDefault();
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject); // Ensure OnboardingData persists across scenes
    }

    public void InitializeDefault()
    {
        Debug.Log("Inside Initilaising OnboardingData function");
        LanguageProficiency = 1;
        LanguageToLearn = "English";
        PhrasesToWorkOn = new List<string> { "Greeting and Introduction", "Ordering Food and Drinks" };
        Scene = "Cafe";
        SceneToRole = new Dictionary<string, string>
        {
            { "Cafe", "Barista" }
        };
    }

    // Intended to be used to update the data
    public void Update(
        int languageProficiency = 0,
        string languageToLearn = "English",
        List<string> phrasesToWorkOn = null,
        string scene = "Cafe",
        Dictionary<string, string> sceneToRole = null)
    {
        LanguageProficiency = languageProficiency;
        LanguageToLearn = languageToLearn;
        PhrasesToWorkOn = phrasesToWorkOn ?? new List<string> { "Greeting and Introduction", "Ordering Food and Drinks" };
        Scene = scene;
        SceneToRole = sceneToRole ?? new Dictionary<string, string> { { "Cafe", "Barista" } };
    }

    // Intended to be used after the setters or Update() is used to update the data
    public void Save()
    {
        semaphore.Wait();
        try
        {
            File.WriteAllText(filePath, GetSerializedData());
        }
        finally
        {
            semaphore.Release();
        }
    }

    // Intended to be used after the setters or Update() is used to update the data
    public async Task SaveAsync()
    {
        await semaphore.WaitAsync();
        try
        {
            using StreamWriter writer = new(filePath, false);
            await writer.WriteAsync(GetSerializedData());
        }
        finally
        {
            semaphore.Release();
        }
    }

    private string GetSerializedData()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
