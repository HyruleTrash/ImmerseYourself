using UnityEngine;

public class UIRotator : MonoBehaviour
{
    private RectTransform rectTransform;
    [SerializeField]
    private float speed;
    [SerializeField]
    private Vector3 axis;
    
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    void Update()
    {
        rectTransform.RotateAround(rectTransform.position, axis, speed * Time.deltaTime);
    }
}
