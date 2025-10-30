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
        RawKey.Numpad9,
        RawKey.N0,
        RawKey.N1,
        RawKey.N2,
        RawKey.N3,
        RawKey.N4,
        RawKey.N5,
        RawKey.N6,
        RawKey.N7,
        RawKey.N8,
        RawKey.N9
    };
    
    private List<RawKey> hadKeys = new();

    [HideInInspector]
    public string inputString;

    private void Start()
    {
        RawInput.OnKeyDown += UpdateInputString;
        CalculatorReader.instance.numberCallback += (i) =>
        {
            inputString += i;
        };
    }

    private void UpdateInputString(RawKey key)
    {
        if (!RawInput.IsRunning)
            RawInput.Start();
        if (!enabled || hadKeys.Contains(key) || !includeKeys.Contains(key)) return;
        hadKeys.Add(key);
        string theNumber = key.ToString().Replace("Numpad", "").Replace("N", "");
        inputString += theNumber;
    }

    public void ClearInputString()
    {
        inputString = "";
        hadKeys.Clear();
    }
}