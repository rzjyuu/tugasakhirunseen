using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystem : MonoBehaviour
{
    [Header("Camera")]
    public Transform cam;
    public Transform spawnPoint;

    [Header("Rotation")]
    public float rotateSpeed = 40f;
    public float clampHorizontal = 40f;
    public float clampVertical = 20f;

    [Header("Edge Screen")]
    public float edgeSize = 120f;

    float rotY;
    float rotX;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // SPAWN CAMERA DI AWAL
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }

        rotX = 0f;
        rotY = 0f;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // KIRI - KANAN
        if (mousePos.x <= edgeSize)
            rotY -= rotateSpeed * Time.deltaTime;
        else if (mousePos.x >= Screen.width - edgeSize)
            rotY += rotateSpeed * Time.deltaTime;

        // ATAS - BAWAH
        if (mousePos.y >= Screen.height - edgeSize)
            rotX -= rotateSpeed * Time.deltaTime;
        else if (mousePos.y <= edgeSize)
            rotX += rotateSpeed * Time.deltaTime;

        rotY = Mathf.Clamp(rotY, -clampHorizontal, clampHorizontal);
        rotX = Mathf.Clamp(rotX, -clampVertical, clampVertical);

        cam.localRotation = Quaternion.Euler(rotX, rotY, 0);
    }

    // TELEPORT KAMERA
    public void TeleportTo(Transform target)
    {
        transform.position = target.position;
        transform.rotation = target.rotation;

        rotX = 0f;
        rotY = 0f;
        cam.localRotation = Quaternion.identity;
    }
}