using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour
{
    public LoadingSymbolController loadingSymbolController;
    public Button testButton; // Reference to the button

    private void Start()
    {
        // Ensure that the button is assigned and add a listener to it
        if (testButton != null)
        {
            Debug.Log("Listener added");
            testButton.onClick.AddListener(StartLoading);
        }
        else
        {
            Debug.LogError("Test Button is not assigned.");
        }
    }

    public void StartLoading()
    {
        if (loadingSymbolController != null)
        {
            Debug.Log("ShowLoadingController");
            loadingSymbolController.ShowLoadingSymbol();
            StartCoroutine(LoadingSequence());
        }
        else
        {
            Debug.LogError("LoadingSymbolController is not assigned.");
        }
    }

    private IEnumerator LoadingSequence()
    {
        // Wait for 1 second
        Debug.Log("Wait");
        yield return new WaitForSeconds(1f);
        loadingSymbolController.HideLoadingSymbol();
        Debug.Log("CloseLoadingController");
    }
}
