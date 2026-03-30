using UnityEngine;

public class CorridorGhost : MonoBehaviour
{
    public SanityManager sanityManager;

    void Update()
    {
        if (sanityManager == null) return;

        if (sanityManager.isCorridor)
        {
            sanityManager.SetCorridorGhost(true);
        }
        else
        {
            sanityManager.SetCorridorGhost(false);
        }
    }
}