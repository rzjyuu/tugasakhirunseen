using UnityEngine;
using UnityEngine.InputSystem;

public class CameraSystemm : MonoBehaviour
{
    [Header("Camera")]
    public Transform cam;
    public Transform spawnCam;

    [HideInInspector] public Transform currentRoom; // simpan room saat ini

    [Header("Rotation")]
    public float sensitivity = 0.12f;
    public float pitchClamp = 25f;

    float yaw;
    float pitch;
    Vector2 lastMousePos;
    bool rotating;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (spawnCam != null)
            ApplySpawnCam();
        else
            SyncFromCamera();
    }

    void Update()
    {
   
        if (Time.timeScale == 0f) return;

        if (Mouse.current == null) return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            rotating = true;
            lastMousePos = Mouse.current.position.ReadValue();
            SyncFromCamera();
            return;
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            rotating = false;
            return;
        }

        if (!rotating) return;

        Vector2 currentPos = Mouse.current.position.ReadValue();
        Vector2 delta = currentPos - lastMousePos;
        lastMousePos = currentPos;

        yaw += delta.x * sensitivity;
        pitch -= delta.y * sensitivity;
        pitch = Mathf.Clamp(pitch, -pitchClamp, pitchClamp);

        cam.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    // Teleport dengan room root
    public void TeleportTo(Transform roomSpawnCam, Transform roomRoot)
    {
        spawnCam = roomSpawnCam;
        ApplySpawnCam();

        currentRoom = roomRoot; // simpan ruangan kamera sekarang
        rotating = false;

        if (Mouse.current != null)
            lastMousePos = Mouse.current.position.ReadValue();
    }

    void ApplySpawnCam()
    {
        transform.position = spawnCam.position;
        transform.rotation = spawnCam.rotation;
        SyncFromCamera();
    }

    void SyncFromCamera()
    {
        Vector3 e = cam.localEulerAngles;
        pitch = Normalize(e.x);
        yaw = Normalize(e.y);
    }

    float Normalize(float a)
    {
        if (a > 180f) a -= 360f;
        return a;
    }

    // Getter untuk Ghost
    public Transform GetCurrentRoom()
    {
        return currentRoom;
    }

    void OnEnable()
    {
        rotating = false;

        if (Mouse.current != null)
            lastMousePos = Mouse.current.position.ReadValue();
    }
}