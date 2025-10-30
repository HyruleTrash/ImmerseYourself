using System;
using UnityEngine;
using UnityEngine.Events;
using UnityRawInput;

public class PressEnterPrompt : MonoBehaviour
{
    public UnityEvent onEnter = new UnityEvent();

    private void Start()
    {
        if (!RawInput.IsRunning)
            RawInput.Start();
        RawInput.OnKeyDown += (key) =>
        {
            if (key == RawKey.Return)
                PressEnter();
        };
    }

    private void Update()
    {
        if (!RawInput.IsRunning)
            RawInput.Start();
    }

    public void PressEnter()
    {
        onEnter.Invoke();
        gameObject.SetActive(false);
        enabled = false;
    }
}
