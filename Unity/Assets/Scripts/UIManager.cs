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
    [SerializeField]
    private GameObject inGameWaitingUI;
    
    private void Start()
    {
        DisableGameUI();
    }

    public void EnableGameUI()
    {
        inGameUI.SetActive(true);
        loadingUI.SetActive(false);
    }
    
    public void DisableGameUI()
    {
        DisableGameWaitingUI();
        inGameUI.SetActive(false);
        loadingUI.SetActive(true);
    }
    
    public void EnableGameWaitingUI()
    {
        if (inGameUI.activeInHierarchy)
            inGameWaitingUI.SetActive(true);
    }
    
    public void DisableGameWaitingUI()
    {
        inGameWaitingUI.SetActive(false);
    }
}
