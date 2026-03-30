using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CandleInteractionManager : MonoBehaviour
{
    public CandleItemManager inventory;
    public RoomCandleManager roomManager;

    Camera cam;

    [Header("Replace System")]
    public float replaceTime = 3f;
    public float progress = 0f;

    GameObject currentRoom;

    [Header("Center Check")]
    public float centerTolerance = 0.05f;

    [Header("UI")]
    public Slider progressBar;
    public GameObject progressUI;

    void Awake()
    {
        cam = Camera.main;

        if (progressUI != null)
            progressUI.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            ResetProgress();
            return;
        }

        // jika kamera berpindah dari lilin
        if (currentRoom != null && hit.collider.gameObject != currentRoom)
        {
            ResetProgress();
        }

        // ================= PICKUP =================
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (hit.collider.CompareTag("PickupCandle"))
            {
                inventory.PickupCandle(hit.collider.gameObject);
                return;
            }
        }

        // ================= REPLACE SYSTEM =================
        if (hit.collider.CompareTag("RoomCandle"))
        {
            GameObject roomObj = hit.collider.gameObject;

            // 🔴 CEK APAKAH LILIN DI TENGAH KAMERA
            if (!IsObjectInCenter(roomObj.transform))
            {
                ResetProgress();
                return;
            }

            if (!roomManager.CanReplace(roomObj))
            {
                ResetProgress();
                return;
            }

            if (Mouse.current.leftButton.isPressed)
            {
                int required = roomManager.GetRequiredCandles(roomObj);

                if (inventory.candleCount < required)
                {
                    ResetProgress();
                    return;
                }

                if (currentRoom == null)
                    currentRoom = roomObj;

                if (currentRoom != roomObj)
                {
                    ResetProgress();
                    currentRoom = roomObj;
                }

                progress += Time.deltaTime / replaceTime;

                if (progressUI != null)
                    progressUI.SetActive(true);

                if (progressBar != null)
                    progressBar.value = progress;

                if (progress >= 1f)
                {
                    ReplaceCandle(roomObj);
                    ResetProgress();
                }
            }
        }
    }

    // ================= CEK TENGAH KAMERA =================
    bool IsObjectInCenter(Transform obj)
    {
        Vector3 viewPos = cam.WorldToViewportPoint(obj.position);

        if (viewPos.z < 0) return false;

        return Mathf.Abs(viewPos.x - 0.5f) < centerTolerance &&
               Mathf.Abs(viewPos.y - 0.5f) < centerTolerance;
    }

    void ReplaceCandle(GameObject roomObj)
    {
        int required = roomManager.GetRequiredCandles(roomObj);

        if (!inventory.UseCandles(required))
        {
            Debug.Log("LILIN TIDAK CUKUP! BUTUH: " + required);
            return;
        }

        roomManager.Replace(roomObj);

        Debug.Log("LILIN BERHASIL DIREPLACE!");
    }

    void ResetProgress()
    {
        progress = 0f;
        currentRoom = null;

        if (progressBar != null)
            progressBar.value = 0f;

        if (progressUI != null)
            progressUI.SetActive(false);
    }
}