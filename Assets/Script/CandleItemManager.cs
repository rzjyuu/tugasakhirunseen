using System.Collections;
using UnityEngine;
using TMPro;

public class CandleItemManager : MonoBehaviour
{
    [Header("Inventory")]
    public int candleCount;
    public int maxCandle = 3;

    [Header("Respawn")]
    public float respawnTime = 30f;

    [Header("UI")]
    public TextMeshProUGUI candleText;

    void Start()
    {
        UpdateUI();
    }

    // ================= PICKUP =================
    public void PickupCandle(GameObject candleObject)
    {
        if (candleCount >= maxCandle)
        {
            Debug.Log("LILIN PENUH!");
            return;
        }

        candleCount++;
        UpdateUI();

        StartCoroutine(RespawnRoutine(candleObject));
    }

    // ================= USE =================
    public bool UseCandles(int amount)
    {
        if (candleCount < amount)
        {
            Debug.Log("LILIN TIDAK CUKUP!");
            return false;
        }

        candleCount -= amount;
        UpdateUI();
        return true;
    }

    // ================= RESPAWN =================
    IEnumerator RespawnRoutine(GameObject candle)
    {
        candle.SetActive(false);
        yield return new WaitForSeconds(respawnTime);
        candle.SetActive(true);
    }

    void UpdateUI()
    {
        if (candleText != null)
            candleText.text = "Lilin: " + candleCount + " / " + maxCandle;
    }
}