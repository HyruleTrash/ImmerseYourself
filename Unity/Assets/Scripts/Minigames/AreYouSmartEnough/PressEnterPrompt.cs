using System;
using UnityEngine;
using UnityEngine.Events;

public class PressEnterPrompt : MonoBehaviour
{
    public UnityEvent onEnter = new UnityEvent();

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.KeypadEnter)) return;
        
        onEnter.Invoke();
        gameObject.SetActive(false);
        enabled = false;
    }
}
