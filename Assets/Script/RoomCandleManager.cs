using UnityEngine;
using System.Collections.Generic;

public class RoomCandleManager : MonoBehaviour
{
    [System.Serializable]
    public class RoomCandle
    {
        public GameObject roomObject;
        public Light roomLight;

        public float maxSanitasi = 100f;
        public float sanitasi = 100f;

        [Header("Decay Per Night")]
        public float night1Decay = 1f;
        public float night2Decay = 2f;

        [Header("Special")]
        public bool isMainRoom;

        [HideInInspector] public float startIntensity;
    }

    [Header("Rooms")]
    public List<RoomCandle> rooms = new List<RoomCandle>();

    [Header("Night System")]
    public GameConditionManager gameConditionManager;

    [Header("Replace Settings")]
    [Range(0f, 1f)]
    public float replaceThreshold = 0.8f; // 80%

    bool skipFirstFrame = true;
    public bool pauseDecay = false;

    void Awake()
    {
        foreach (var r in rooms)
        {
            if (r.roomObject == null || r.roomLight == null)
            {
                Debug.LogError("Room atau Light belum diisi!");
                continue;
            }

            r.sanitasi = r.maxSanitasi;

            r.startIntensity = r.roomLight.intensity > 0f
                ? r.roomLight.intensity
                : 1.5f;

            r.roomLight.enabled = true;
            r.roomLight.intensity = r.startIntensity;
        }
    }

    void Update()
    {
        if (skipFirstFrame)
        {
            skipFirstFrame = false;
            return;
        }

        if (pauseDecay) return;

        foreach (var r in rooms)
        {
            if (r.roomLight == null) continue;

            float currentDecay = r.night1Decay;

            if (gameConditionManager != null)
            {
                if (gameConditionManager.currentNight == 1)
                    currentDecay = r.night1Decay;
                else if (gameConditionManager.currentNight == 2)
                    currentDecay = r.night2Decay;
            }

            r.sanitasi -= currentDecay * Time.deltaTime;
            r.sanitasi = Mathf.Clamp(r.sanitasi, 0f, r.maxSanitasi);

            if (r.sanitasi <= 0f)
            {
                r.roomLight.enabled = false;
            }
            else
            {
                r.roomLight.enabled = true;
                r.roomLight.intensity =
                    r.startIntensity * (r.sanitasi / r.maxSanitasi);
            }
        }
    }

    RoomCandle GetRoom(GameObject obj)
    {
        return rooms.Find(r =>
            obj == r.roomObject ||
            obj.transform.IsChildOf(r.roomObject.transform)
        );
    }

    // ================= REQUIRED CANDLES =================

    public int GetRequiredCandles(GameObject obj)
    {
        var r = GetRoom(obj);
        if (r == null) return 1;

        if (gameConditionManager != null &&
            gameConditionManager.currentNight == 2 &&
            r.isMainRoom)
        {
            return 2;
        }

        return 1;
    }

    // ================= REPLACE =================

    public void Replace(GameObject obj)
    {
        var r = GetRoom(obj);
        if (r == null) return;

        r.sanitasi = r.maxSanitasi;

        if (r.roomLight != null)
        {
            r.roomLight.enabled = true;
            r.roomLight.intensity = r.startIntensity;
        }
    }

    // ================= UTILITY =================

    public bool IsRoomActive(GameObject obj)
    {
        var r = GetRoom(obj);
        if (r == null) return false;

        return r.roomLight != null &&
               r.roomLight.enabled &&
               r.roomLight.intensity > 0.05f;
    }

    public bool CanReplace(GameObject obj)
    {
        var r = GetRoom(obj);
        if (r == null) return false;

        return r.sanitasi <= r.maxSanitasi * replaceThreshold;
    }

    public void ResetAllRooms()
    {
        pauseDecay = true;
        skipFirstFrame = true;

        foreach (var r in rooms)
        {
            if (r.roomLight == null) continue;

            r.sanitasi = r.maxSanitasi;
            r.roomLight.enabled = true;
            r.roomLight.intensity = r.startIntensity;
        }

        StartCoroutine(ResumeDecayDelay());
    }

    System.Collections.IEnumerator ResumeDecayDelay()
    {
        yield return new WaitForSeconds(0.5f);
        pauseDecay = false;
    }
}