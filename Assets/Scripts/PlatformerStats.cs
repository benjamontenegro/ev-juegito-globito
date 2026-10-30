using UnityEngine;

/// <summary>
/// SHORTHANDS DE LA CLASE UNITYENGINE.TIME. <br/>
/// ES UN ENCHUFE TERRIBLE FLOJO HRMNOOOO
/// </summary>
public static class Dtime
{
    /// <summary>
    /// "60 *Time.deltatime" <br/>
    /// Despues cambia esto porfavorrr
    /// </summary>
    public static float time { get
        {
            return 60 *Time.deltaTime;
        }
    }
    /// <summary>
    /// "60 *Time.fixedDeltaTime" <br/>
    /// Hrmno cambia esto
    /// </summary>
    public static float fTime { get
        {
            return 60 *Time.fixedDeltaTime;
        }
    }
}

[CreateAssetMenu(fileName = "PlatformerStatsAsset", menuName = "Scriptable Objects/PlatformerStats")]
public class PlatformerStats : ScriptableObject
{
    // 16 PIXELES POR UNIDAD. OSEA
    // 8px -> 0.5f; 1px -> 0.0625f;

    [Header("Velocidades")]
    public float MaxFallSpd = 3f;
    [Tooltip("Gravedad para que sea comun")]
    public float GravSpd = 0.0156f; // 0.015625 (1px *0.25)

    [Header("Colisiones uwu")]
    // public LayerMask WallFlags;
    public LayerMask GroundFlags;
    public LayerMask GroundOneWayFlags;
    public LayerMask SideFlags;
    public LayerMask CeilFlags;

    // 0.0167f == FRACCION DE SEGUNDO QUE PASA UN FRAME
    // OSEA QUE ALMENOS LAS DISTANCIAS SE TOCAN A 1 SEG??
    // OJALA
    
    public float GroundedColDist = 0.0167f; //SIX SEVEN
    public float GroundedColHeight = 0.0167f;
    public float HeadWidthDif = 0.125f;

    public float SideColWidth = 0.0167f;
    public float SideColDist = 0.0167f;

    [Header("Debug")]
    public bool debugBboxEnable = true;
}
