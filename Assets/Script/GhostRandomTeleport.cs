using UnityEngine;

public class GhostRandomTeleport : MonoBehaviour
{
    [Header("Teleport Points")]
    public Transform[] roomPoints;

    [Header("References")]
    public SanityManager sanityManager;
    public CameraSystemm cameraSystemm;

    [Header("Teleport Settings")]
    public float minTeleportDelay = 4f;
    public float maxTeleportDelay = 8f;

    [Header("Vision Settings")]
    public float viewAngle = 60f;
    public float visibleDistance = 15f;
    public LayerMask visionMask;

    int currentIndex = -1;

    float timer;
    float nextTeleportTime;

    void Start()
    {
        if (roomPoints.Length == 0) return;

        TeleportRandom();
        SetNextTeleportTime();
    }

    void Update()
    {
        if (roomPoints.Length == 0) return;

        timer += Time.deltaTime;

        CheckIfVisible();

        if (timer >= nextTeleportTime)
        {
            TeleportRandom();
            SetNextTeleportTime();
        }
    }

    void TeleportRandom()
    {
        int newIndex;

        do
        {
            newIndex = Random.Range(0, roomPoints.Length);
        }
        while (newIndex == currentIndex && roomPoints.Length > 1);

        currentIndex = newIndex;

        transform.position = roomPoints[currentIndex].position;
    }

    void SetNextTeleportTime()
    {
        timer = 0f;
        nextTeleportTime = Random.Range(minTeleportDelay, maxTeleportDelay);
    }

    void CheckIfVisible()
    {
        if (sanityManager == null || cameraSystemm == null)
        {
            sanityManager.SetRoomGhost(false);
            return;
        }

        Transform cam = cameraSystemm.transform;

        Vector3 dir = (transform.position - cam.position).normalized;

        float angle = Vector3.Angle(cam.forward, dir);

        if (angle > viewAngle)
        {
            sanityManager.SetRoomGhost(false);
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(cam.position, dir, out hit, visibleDistance, visionMask))
        {
            if (hit.collider.CompareTag("Ghost"))
            {
                sanityManager.SetRoomGhost(true);
                return;
            }
        }

        sanityManager.SetRoomGhost(false);
    }
}