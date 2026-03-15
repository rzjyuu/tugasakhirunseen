using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ClickTeleportManager : MonoBehaviour
{
    public CameraSystem cameraSystem;
    public RoomCandleManager roomCandleManager;
    public Transform cameraRoot;
    public SanityManager sanityManager;

    [Header("Sound")]
    public AudioClip doorClickSound;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeTime = 0.2f;
    public float teleportDelay = 0.15f;

    bool isTeleporting;

    [System.Serializable]
    public class Door
    {
        public Collider collider;

        public Transform roomA;
        public Transform roomB;

        public GameObject roomARoot;
        public GameObject roomBRoot;
    }

    public Door[] doors;

    void Update()
    {
        if (isTeleporting)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit))
            return;

        foreach (var door in doors)
        {
            if (hit.collider != door.collider)
                continue;

            float distA = Vector3.Distance(cameraRoot.position, door.roomA.position);
            float distB = Vector3.Distance(cameraRoot.position, door.roomB.position);

            bool currentlyAtA = distA < distB;

            Transform targetPoint = currentlyAtA ? door.roomB : door.roomA;
            GameObject targetRoot = currentlyAtA ? door.roomBRoot : door.roomARoot;

            // ===== CORRIDOR =====
            if (targetRoot.CompareTag("Corridor"))
            {
                StartCoroutine(TeleportSequence(targetPoint, targetRoot));
                return;
            }

            // ===== ROOM (CEK LILIN) =====
            if (targetRoot.CompareTag("Room") || targetRoot.CompareTag("MainRoom"))
            {
                if (!roomCandleManager.IsRoomActive(targetRoot))
                {
                    Debug.Log("Ruangan gelap, teleport DIBLOK");
                    return;
                }

                StartCoroutine(TeleportSequence(targetPoint, targetRoot));
                return;
            }

            return;
        }
    }

    IEnumerator TeleportSequence(Transform targetPoint, GameObject targetRoot)
    {
        isTeleporting = true;

        // 🔊 SOUND
        if (doorClickSound)
            AudioSource.PlayClipAtPoint(doorClickSound, Camera.main.transform.position);

        // 🌑 FADE OUT
        yield return StartCoroutine(Fade(0f, 1f));

        // ⏱ DELAY
        yield return new WaitForSeconds(teleportDelay);

        // 📷 TELEPORT
        cameraSystem.TeleportTo(targetPoint);

        // 🔥 UPDATE SANITY
        if (sanityManager != null)
        {
            if (targetRoot.CompareTag("MainRoom"))
                sanityManager.SetMainRoom();
            else if (targetRoot.CompareTag("Corridor"))
                sanityManager.SetCorridor();
            else if (targetRoot.CompareTag("Room"))
                sanityManager.SetRoom();
        }

        // 🌑 FADE IN
        yield return StartCoroutine(Fade(1f, 0f));

        isTeleporting = false;
    }

    IEnumerator Fade(float from, float to)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeTime);
            fadeImage.color = c;
            yield return null;
        }

        c.a = to;
        fadeImage.color = c;
    }
}