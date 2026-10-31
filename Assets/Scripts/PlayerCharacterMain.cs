using UnityEngine;

public class PlayerCharacterMain : MonoBehaviour
{
    [Header("Referencias")]

    private EntityMovement Ent;
    private PlatformerStats Stats;
    [SerializeField] private AudioManager _sfx; // PUEDE SER MANUAL??
    
    [Header("Variables")]
    [SerializeField] private float MoveSpd = 12.5f;
    [SerializeField] private float MoveAcc = 3f;
    

    void Awake()
    {
        // PENDIENTE
    }
}
