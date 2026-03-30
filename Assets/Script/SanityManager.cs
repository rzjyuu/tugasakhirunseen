using UnityEngine;
using UnityEngine.UI;

public class SanityManager : MonoBehaviour
{
    [Header("References")]
    public GameConditionManager gameConditionManager;

    [Header("Sanity")]
    public float maxSanity = 100f;
    public float currentSanity;

    [Header("UI")]
    public Slider sanitySlider;

    [Header("Drain Settings")]
    public float corridorDrain = 5f;
    public float roomDrain = 5f;
    public float ghostDrain = 15f;
    public float regenRate = 10f;

    [Header("Location State")]
    public bool isMainRoom = true;
    public bool isCorridor = false;
    public bool isRoom = false;

    [Header("Ghost State")]
    public bool corridorGhostActive = false;
    public bool roomGhostVisible = false;

    bool sanityDepleted;

    void Start()
    {
        ResetSanity();
        SetMainRoom();

        if (sanitySlider != null)
        {
            sanitySlider.maxValue = maxSanity;
            sanitySlider.value = currentSanity;
        }
    }

    void Update()
    {
        HandleSanity();
        UpdateUI();
        CheckGameOver();
    }

    void HandleSanity()
    {
        if (isMainRoom)
        {
            currentSanity += regenRate * Time.deltaTime;
        }
        else
        {
            if (corridorGhostActive)
                currentSanity -= ghostDrain * Time.deltaTime;

            else if (roomGhostVisible)
                currentSanity -= ghostDrain * Time.deltaTime;

            else if (isCorridor)
                currentSanity -= corridorDrain * Time.deltaTime;

            else if (isRoom)
                currentSanity -= roomDrain * Time.deltaTime;
        }

        currentSanity = Mathf.Clamp(currentSanity, 0f, maxSanity);
    }

    void UpdateUI()
    {
        if (sanitySlider != null)
            sanitySlider.value = currentSanity;
    }

    void CheckGameOver()
    {
        if (!sanityDepleted && currentSanity <= 0f)
        {
            sanityDepleted = true;

            if (gameConditionManager != null)
                gameConditionManager.TriggerLose();
        }
    }

    public void SetMainRoom()
    {
        isMainRoom = true;
        isCorridor = false;
        isRoom = false;
    }

    public void SetCorridor()
    {
        isMainRoom = false;
        isCorridor = true;
        isRoom = false;
    }

    public void SetRoom()
    {
        isMainRoom = false;
        isCorridor = false;
        isRoom = true;
    }

    public void SetCorridorGhost(bool state)
    {
        corridorGhostActive = state;
    }

    public void SetRoomGhost(bool state)
    {
        roomGhostVisible = state;
    }

    public void ResetSanity()
    {
        currentSanity = maxSanity;
        sanityDepleted = false;
    }
}