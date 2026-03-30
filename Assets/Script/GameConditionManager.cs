using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

public class GameConditionManager : MonoBehaviour
{
    [Header("References")]
    public GameTime gameTime;
    public RoomCandleManager roomManager;
    public SanityManager sanityManager;

    [Header("Night System")]
    public int currentNight = 1;
    public int maxNight = 2;

    [Header("Timelines")]
    public PlayableDirector nightTimeline;
    public PlayableDirector loseTimeline;

    [Header("UI")]
    public GameObject gameOverUI;

    [Header("Spawn Camera Settings")]
    public Transform mainRoomCameraSpawn;
    public Transform cameraTransform;

    bool transitioning;
    bool gameEnded;

    void Start()
    {
        ResetTimeline(nightTimeline);
        ResetTimeline(loseTimeline);
    }

    void Update()
    {
        if (gameEnded || transitioning)
            return;

        // 🔥 Lose: Semua lilin mati
        if (AllRoomsDark())
        {
            TriggerLose();
            return;
        }

        // 🌅 Night transition
        if (gameTime != null && gameTime.IsSixAM())
        {
            StartCoroutine(NightTransition());
        }
    }

    bool AllRoomsDark()
    {
        foreach (var r in roomManager.rooms)
        {
            if (r.roomLight != null &&
                r.roomLight.enabled &&
                r.roomLight.intensity > 0.05f)
            {
                return false;
            }
        }
        return true;
    }

    // ===============================
    // 🔥 LOSE SYSTEM
    // ===============================

    public void TriggerLose()
    {
        if (gameEnded)
            return;

        StartCoroutine(PlayLoseSequence());
    }

    IEnumerator PlayLoseSequence()
    {
        gameEnded = true;
        transitioning = true;

        // 🔥 Stop semua sistem gameplay
        if (roomManager != null) roomManager.pauseDecay = true;
        if (gameTime != null) gameTime.enabled = false;
        if (sanityManager != null) sanityManager.enabled = false;

        // 🔥 Freeze gameplay tapi timeline tetap jalan
        Time.timeScale = 0f;

        if (loseTimeline != null)
        {
            loseTimeline.timeUpdateMode = DirectorUpdateMode.UnscaledGameTime;
            loseTimeline.Play();

            yield return new WaitForSecondsRealtime((float)loseTimeline.duration);
        }

        // 🔥 Game benar-benar berhenti
        EndGame();
    }

    // ===============================
    // 🌅 NIGHT TRANSITION
    // ===============================

    IEnumerator NightTransition()
    {
        transitioning = true;

        // 🔥 Stop semua sistem gameplay
        if (roomManager != null) roomManager.pauseDecay = true;
        if (sanityManager != null) sanityManager.enabled = false;
        if (gameTime != null) gameTime.enabled = false;

        if (nightTimeline != null)
        {
            nightTimeline.timeUpdateMode = DirectorUpdateMode.GameTime;
            nightTimeline.Play();

            yield return new WaitForSeconds((float)nightTimeline.duration);
            ResetTimeline(nightTimeline);
        }

        if (currentNight >= maxNight)
        {
            TriggerLose();
            yield break;
        }

        currentNight++;

        // Teleport ke main room
        if (mainRoomCameraSpawn != null && cameraTransform != null)
        {
            cameraTransform.position = mainRoomCameraSpawn.position;
            cameraTransform.rotation = mainRoomCameraSpawn.rotation;
        }

        if (sanityManager != null)
        {
            sanityManager.ResetSanity();
            sanityManager.SetMainRoom();
        }

        if (gameTime != null) gameTime.ResetTime();
        if (roomManager != null) roomManager.ResetAllRooms();

        // 🔥 Aktifkan lagi gameplay
        if (sanityManager != null) sanityManager.enabled = true;
        if (gameTime != null) gameTime.enabled = true;
        if (roomManager != null) roomManager.pauseDecay = false;

        transitioning = false;
    }

    void ResetTimeline(PlayableDirector director)
    {
        if (director == null) return;

        director.Stop();
        director.time = 0;
        director.Evaluate();
    }

    void EndGame()
    {
        // Freeze game
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Tampilkan UI Game Over
        if (gameOverUI != null)
            gameOverUI.SetActive(true);

        Debug.Log("GAME OVER - UI SHOWN");
    }
}