using UnityEngine;
// using UnityEngine.UI;

public class GameStatsManager : MonoBehaviour
{
    public static int CoinsCollected { get; private set; }
    public int CoinsCollectedTrack;
    // public static float totalGameTime;

    void Start()
    {
        CoinsCollected = 0;
    }

    void Update()
    {
        CoinsCollectedTrack = CoinsCollected;
    }

    #region METOOS ############################################################

    /// <summary> +1 moneda </summary>
    public static void AddCoinCollected()
    {
        CoinsCollected += 1;
    }

    #endregion
}
