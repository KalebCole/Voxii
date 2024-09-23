using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManagement : MonoBehaviour
{

    public void goToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
