using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public List<Toggle> optionToggles;
    public List<Toggle> sceneToggles;
    public Slider languageProficiencySlider;
    public Slider avatarHostilitySlider;
    public Button doneButton;

    void Start()
    {
        // Initialize
        MenuData.SetFilePath(Application.persistentDataPath);

        // Load previously saved data (if it exists)
        MenuData.LoadDataFromJson();

        // Set initial UI values
        SetInitialUIValues();

        // Add a listener to the Done button
        doneButton.onClick.AddListener(OnDoneButtonPressed);

        // Add listeners for the scene segmented toggles
        foreach (var toggle in sceneToggles)
        {
            toggle.onValueChanged.AddListener(OnSceneToggleChanged);
        }
    }

    private void SetInitialUIValues()
    {
        // Set toggle values based on persisted data
        for (int i = 0; i < optionToggles.Count; i++)
        {
            optionToggles[i].isOn = i < MenuData.OptionsSelected.Count && MenuData.OptionsSelected[i];
        }

        // Set the segmented control for the scene
        for (int i = 0; i < sceneToggles.Count; i++)
        {
            sceneToggles[i].isOn = (sceneToggles[i].name == MenuData.SceneSelection);
        }

        // Set sliders to match saved values
        languageProficiencySlider.value = MenuData.LanguageProficiency;
        avatarHostilitySlider.value = MenuData.AvatarHostility;
    }

    private void OnSceneToggleChanged(bool isOn)
    {
        if (!isOn) return; // Skip if toggle is being turned off

        // Ensure only one toggle is selected at a time
        for (int i = 0; i < sceneToggles.Count; i++)
        {
            if (sceneToggles[i].isOn)
            {
                MenuData.SceneSelection = sceneToggles[i].name;
                break;
            }
        }
    }

    private void OnDoneButtonPressed()
    {
        // Save current UI values to the persistent MenuData class
        MenuData.OptionsSelected.Clear();
        foreach (var toggle in optionToggles)
        {
            MenuData.OptionsSelected.Add(toggle.isOn);
        }

        MenuData.LanguageProficiency = languageProficiencySlider.value;
        MenuData.AvatarHostility = avatarHostilitySlider.value;

        // Persist data to JSON
        MenuData.SaveDataToJson();

        // Load the next scene
        SceneManager.LoadScene(MenuData.SceneSelection);
    }
}

