using System;
using UnityEngine;
using WiimoteApi;

public class CartController : MonoBehaviour
{
    [SerializeField]
    private WiiMoteInterpreter wiimote;
    private Vector3 lastRotation;
    [SerializeField]
    private float smoothingFactor = 5f;

    private void Start()
    {
        wiimote = GetComponent<WiiMoteInterpreter>();
        wiimote.onWiiRemoteUpdate.AddListener(CustomUpdate);
    }

    private void CustomUpdate()
    {
        lastRotation = wiimote.rotation;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            Quaternion.Euler(lastRotation),
            smoothingFactor * Time.deltaTime
        );
        
        // x 10 limit, z 25 limit
        // Vector3 eulerRotation = transform.eulerAngles;
        // eulerRotation.x = Mathf.Clamp(eulerRotation.x, -10f, 10f);
        // eulerRotation.z = Mathf.Clamp(eulerRotation.z, -25f, 25f);
        //
        // // Normalize rotation values to handle wraparound
        // eulerRotation.x = eulerRotation.x % 360f;
        // eulerRotation.z = eulerRotation.z % 360f;
        //
        // transform.rotation = Quaternion.Euler(eulerRotation);
    }
}