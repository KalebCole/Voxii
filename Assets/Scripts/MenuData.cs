using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class MenuData
{
    public static List<bool> OptionsSelected = new List<bool>();
    public static string SceneSelection = "";
    public static float LanguageProficiency = 0.5f;
    public static float AvatarHostility = 0.5f;

    private static string filePath = Application.persistentDataPath + "/menuData.json";

    public static void SaveDataToJson()
    {
        MenuDataModel data = new MenuDataModel()
        {
            OptionsSelected = OptionsSelected,
            SceneSelection = SceneSelection,
            LanguageProficiency = LanguageProficiency,
            AvatarHostility = AvatarHostility
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public static void LoadDataFromJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            MenuDataModel data = JsonUtility.FromJson<MenuDataModel>(json);

            OptionsSelected = data.OptionsSelected;
            SceneSelection = data.SceneSelection;
            LanguageProficiency = data.LanguageProficiency;
            AvatarHostility = data.AvatarHostility;
        }
    }

    public static string getRole() // TODO: exapand for other scenes
    {
        if (SceneSelection == "Cafe")
        {
            return "barista";
        }
        else
        {
            return "worker";
        }
    }

    [System.Serializable]
    public class MenuDataModel
    {
        public List<bool> OptionsSelected;
        public string SceneSelection;
        public float LanguageProficiency;
        public float AvatarHostility;
    }
}
