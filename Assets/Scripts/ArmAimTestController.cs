using UnityEngine;
using UnityEngine.InputSystem;

public class ArmAimTestController : MonoBehaviour
{
    public float testAngle = 30f;



    private void Update()
    {
        
        transform.localRotation = Quaternion.Euler(0f, 0f, testAngle);

        Vector3 mouseScreen = Input.mousePosition;

        Debug.Log(mouseScreen);

        mouseScreen.z = Camera.main.WorldToScreenPoint(transform.position).z;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);

        Debug.DrawLine(transform.position, mouseWorld, Color.red);
    }
}