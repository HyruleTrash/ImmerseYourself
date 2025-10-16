
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    private bool running;
    private float currentTime = 0;
    [SerializeField]
    private float savedMaxTime = 5;
    [SerializeField]
    private float minTime = 1;
    [SerializeField]
    private float maxTimeDecrementation = 1;
    private float maxTime;
    public UnityEvent onTimerEnd = new UnityEvent();
    [SerializeField]
    private Image representationImage;

    private void Start()
    {
        representationImage = GetComponent<Image>();
        maxTime = savedMaxTime;
    }

    public void Reset()
    {
        currentTime = maxTime;
    }

    public void SetMaxTimeShorter()
    {
        if (maxTime - 1 <= 0)
            maxTime = minTime;
        else
            maxTime -= maxTimeDecrementation;
    }

    public void StartTimer()
    {
        running = true;
    }

    private void OnTimeout()
    {
        running = false;
        onTimerEnd.Invoke();
    }

    private void Update()
    {
        if (!running)
            return;
        currentTime -= Time.deltaTime;
        representationImage.fillAmount = currentTime / maxTime;
        if (currentTime <= 0)
            OnTimeout();
    }
}