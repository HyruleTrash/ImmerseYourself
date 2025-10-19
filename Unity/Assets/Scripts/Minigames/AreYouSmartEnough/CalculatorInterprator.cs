using System;
using System.Collections.Generic;
using UnityEngine;
using UnityRawInput;

public class CalculatorInterprator : MonoBehaviour
{
    private readonly List<RawKey> includeKeys = new ()
    {
        RawKey.Numpad0,
        RawKey.Numpad1,
        RawKey.Numpad2,
        RawKey.Numpad3,
        RawKey.Numpad4,
        RawKey.Numpad5,
        RawKey.Numpad6,
        RawKey.Numpad7,
        RawKey.Numpad8,
        RawKey.Numpad9
    };
    
    private List<RawKey> hadKeys = new();

    [HideInInspector]
    public string inputString;

    private void Start()
    {
        RawInput.OnKeyDown += UpdateInputString;
    }

    private void UpdateInputString(RawKey key)
    {
        if (!enabled || hadKeys.Contains(key) || !includeKeys.Contains(key)) return;
        hadKeys.Add(key);
        inputString += key.ToString().Replace("Numpad", "");
    }

    public void ClearInputString()
    {
        inputString = "";
        hadKeys.Clear();
    }
}