using System;
using UnityEngine;
using UnityEngine.Events;
using UnityRawInput;

public class PressEnterPrompt : MonoBehaviour
{
    public UnityEvent onEnter = new UnityEvent();
    
    private void Update()
    {
        if (!RawInput.IsKeyDown(RawKey.Return)) return;
        
        onEnter.Invoke();
        gameObject.SetActive(false);
        enabled = false;
    }
}
