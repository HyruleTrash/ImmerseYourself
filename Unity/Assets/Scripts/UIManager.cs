using TMPro;
using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager>
{
    public GameObject startMenu;
    public TMP_InputField usernameField;

    public void ConnectToServer()
    {
        startMenu.SetActive(false);
        usernameField.interactable = true;
        Client.instance.ConnectToServer();
    }
}
