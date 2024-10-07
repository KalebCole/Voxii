using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManagement : MonoBehaviour
{
    public GameObject display1;
    public GameObject display2;

    public void goToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void goToPostLevel()
    {
        SceneManager.LoadScene("PostLevel");
    }

    public void switchDisplays()
    {
        display1.SetActive(false);
        display2.SetActive(true);
    }

}
