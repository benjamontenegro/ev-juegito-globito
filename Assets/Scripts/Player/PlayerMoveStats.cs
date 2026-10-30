using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerMoveStats", menuName = "Stats Movimiento Jugador")]
public class PlayerMoveStats : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)] public float MaxMoveSpd = 12.5f;
    [Range(0.25f, 100f)] public float MoveAcc = 5f;
    [Range(0.25f, 100f)] public float MoveFri = 20f;
    [Range(0.25f, 100f)] public float MoveAccAir = 5f;
    [Range(0.25f, 100f)] public float MoveFriAir = 3f;

    [Header("Colisiones Del Piso")]
    public LayerMask GroundLy;
    public float GroundDetectRayDist = 0.02f;
    public float HeadDetectRayDist = 0.02f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;

    [Header("Salto")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)]public float JumpHeightCompensationFactor = 1.054F;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)]public float GravityOnReleaseMult = 2f;
    public float MaxFallSpd = 20f;
    [Range(1, 5)]public int JumpsCanAmount = 2;

    [Header("Salto detenido")]
    [Range(0.02f, 0.3f)] public float timeForUpcancel = 0.027f;

    [Header("Tope del salto")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.07f;
    
    [Header("Delays salto")]
    [Range(0f, 1f)] public float JumpBuffer = 0.125f;
    [Range(0f, 1f)] public float JumpCoyote = 0.1f;

    [Header("Debug")]
    public bool DebugShowGrounded = true;

    public float Gravity { get; private set; }
    public float InicialJumpVel { get; private set; }
    public float jumpHeightAdjust {get; private set; }

    private void OnValidate() { CalculateValues(); }
    private void OnEnable() { CalculateValues(); }

    private void CalculateValues()
    {
        jumpHeightAdjust = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f *jumpHeightAdjust) / Mathf.Pow(TimeTillJumpApex, 2f);
        InicialJumpVel = Mathf.Abs(Gravity) * TimeTillJumpApex;
    }

}
