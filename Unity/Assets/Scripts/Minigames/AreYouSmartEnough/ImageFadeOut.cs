using System;
using UnityEngine;
using UnityEngine.UI;

public class ImageFadeOut : MonoBehaviour
{
    public float speed = 1;
    [SerializeField]
    private Image image;
    private Color originalColor;
    private bool isFading = false;
    
    private void Start()
    {
        image = image.GetComponent<Image>();
        UpdateColor();
        image.color = new Color(originalColor.r, originalColor.g,originalColor.b, 0);
    }

    public void UpdateColor()
    {
        originalColor = image.color;
    }

    public void Appear()
    {
        image.color = new Color(originalColor.r, originalColor.g,originalColor.b, 1);
    }

    public void TriggerFadeOut()
    {
        isFading = true;
    }

    private void Update()
    {
        if (!isFading)
            return;
        image.color = new Color(originalColor.r, originalColor.g, originalColor.b, image.color.a - Time.deltaTime * speed);
        if (image.color.a <= 0)
            isFading = false;
    }
}
