using UnityEngine;
using UnityEngine.Tilemaps;

public class CompTilemapHide : MonoBehaviour
{
    [SerializeField] private bool hideTilemaps = true;
    private TilemapRenderer[] tilemaps;
        
    void Start()
    {
        FindTilemaps();
        if (hideTilemaps)
            DisableAllTilemaps();
    }

    private void DisableAllTilemaps()
    {
        foreach(TilemapRenderer tilemap in tilemaps)
        {
            tilemap.enabled = false;
        }
    }

    private void FindTilemaps()
    {
        int tilemapCount = transform.childCount;
        tilemaps = new TilemapRenderer[tilemapCount];

        for (int i = 0; i < tilemapCount; i ++)
        {
            tilemaps[i] = transform.GetChild(i).GetComponent<TilemapRenderer>();
        }
    }
}
