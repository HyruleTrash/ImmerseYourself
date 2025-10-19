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

    [HideInInspector]
    public string inputString;
    
    private void Update()
    {
        inputString = "";
        var hadKeys = new List<RawKey>();
        foreach (var key in RawInput.PressedKeys)
        {
            if (!includeKeys.Contains(key) || hadKeys.Contains(key)) continue;
            hadKeys.Add(key);
            inputString += key.ToString().Replace("Numpad", "");
        }
    }
}