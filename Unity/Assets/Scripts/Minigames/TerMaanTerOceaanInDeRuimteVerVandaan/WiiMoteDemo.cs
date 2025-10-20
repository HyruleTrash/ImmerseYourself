using System;
using System.Collections;
using UnityEngine;
using WiimoteApi;

public class WiiMoteDemo : MonoBehaviour
{
    private Wiimote wiimote;
    private float lastUpdate = 0f;
    private const float UPDATE_INTERVAL = 0.02f;

    private void Start()
    {
        CheckForWiimotes();
    }

    public void CheckForWiimotes()
    {
        WiimoteManager.FindWiimotes();
        wiimote = WiimoteManager.Wiimotes[0];
        if (wiimote == null)
            return;
        Debug.Log("Wiimote found");
        // Calibrate motion controls
        wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
        wiimote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
        // Set player LEDS
        wiimote.SendPlayerLED(true, true, true, true);
    }

    private void Update()
    {
        if (wiimote == null)
            return;

        if (Time.time - lastUpdate >= UPDATE_INTERVAL)
        {
            int ret;
            do
            {
                ret = wiimote.ReadWiimoteData();
            } while (ret > 0);

            if (wiimote.Button.home)
            {
                wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_ACCEL);
                wiimote.Accel.CalibrateAccel(AccelCalibrationStep.A_BUTTON_UP);
            }
            
            var motionControls = GetAccelVector();
            Debug.Log($"motion controls: {motionControls.x}, {motionControls.y}, {motionControls.z}");
            
            lastUpdate = Time.time;
        }
    }
    
    void OnDrawGizmos()
    {
        if (wiimote == null) return;

        Gizmos.color = Color.red;
        Vector3 motionVector = GetAccelVector();
        
        // Draw multiple segments for better visualization
        float length = 2f;
        int segments = 3;
        Vector3 startPosition = transform.position;
        
        for (int i = 0; i < segments; i++)
        {
            Vector3 endPosition = startPosition + transform.rotation * motionVector * (length / segments);
            Gizmos.DrawLine(startPosition, endPosition);
            startPosition = endPosition;
        }
    }
    
    private Vector3 GetAccelVector()
    {
        var accel = wiimote.Accel.GetCalibratedAccelData();
        var accelX = accel[0];
        var accelY = -accel[2];
        var accelZ = -accel[1];

        Vector3 vector = new Vector3(accelX, accelY, accelZ);
        
        // Add threshold check
        if (vector.magnitude < 0.1f)
            return Vector3.zero;
            
        return vector.normalized;
    }
    
    void OnApplicationQuit() {
        if (wiimote != null) {
            WiimoteManager.Cleanup(wiimote);
            wiimote = null;
        }
    }
}
