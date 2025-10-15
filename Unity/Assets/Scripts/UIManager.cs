using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField]
    private Image loadingUI;
    [SerializeField]
    private Image inGameUI;
    
    private void Start()
    {
        inGameUI.enabled = false;
        loadingUI.enabled = true;
    }

    public void EnableGameUI()
    {
        loadingUI.enabled = false;
        inGameUI.enabled = true;
    }
    
    public void DisableGameUI()
    {
        loadingUI.enabled = true;
        inGameUI.enabled = false;
    }
}
