using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField]
    private GameObject loadingUI;
    [SerializeField]
    private GameObject inGameUI;
    
    private void Start()
    {
        inGameUI.SetActive(false);
        loadingUI.SetActive(true);
    }

    public void EnableGameUI()
    {
        inGameUI.SetActive(true);
        loadingUI.SetActive(false);
    }
    
    public void DisableGameUI()
    {
        inGameUI.SetActive(false);
        loadingUI.SetActive(true);
    }
}
