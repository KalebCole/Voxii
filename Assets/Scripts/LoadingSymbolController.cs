using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSymbolController : MonoBehaviour
{
    public GameObject loadingSymbol; // Reference to the loading symbol GameObject

    private void Start()
    {
        // Ensure the loading symbol is hidden initially
        if (loadingSymbol != null)
        {
            loadingSymbol.SetActive(false);
        }
    }

    public void ShowLoadingSymbol()
    {
        if (loadingSymbol != null)
        {
            loadingSymbol.SetActive(true);
        }
    }

    public void HideLoadingSymbol()
    {
        if (loadingSymbol != null)
        {
            loadingSymbol.SetActive(false);
        }
    }
}
