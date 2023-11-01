using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lookSpeedX;
    [SerializeField] private float lookSpeedY;
    [SerializeField] private Transform orientation;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (PlayerPrefs.HasKey("Sensivity"))
        {
            lookSpeedX *= PlayerPrefs.GetFloat("Sensivity");
            lookSpeedY *= PlayerPrefs.GetFloat("Sensivity");
        }
    }

    private void Update()
    {
        Look();
    }

    private float xRotation, yRotation;
    private void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * lookSpeedX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * lookSpeedY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}
