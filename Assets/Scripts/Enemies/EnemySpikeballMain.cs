using System;
using UnityEngine;

[RequireComponent(typeof(EntityMovement))]
public class EnemySpikeballMain : MonoBehaviour
{
    #region REFS ############################################################
        
    [Header("Referencias")]

    EntityMovement Ent;
    PlatformerStats stats;
    AudioManager _sfx;
    
    [Header("Variables del enemigo")]
    [SerializeField] private float MoveMaxSpd = 5f;
    [SerializeField] private float MoveAcc = 5f;
    [SerializeField] private int MoveDirX;

    float MoveYWaveTimer;

    float MoveX { get => Ent.moveVel.x; set => Ent.moveVel.x = value; }
    float MoveY { get => Ent.moveVel.y; set => Ent.moveVel.y = value; }


    // private float movex { get ; set; }

    #endregion ############################################################

    #region MESSAGES ############################################################

    void Awake()
    {
        Ent = GetComponent<EntityMovement>();
        stats = Ent.stats;

        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();

        if (MoveDirX == -1)
            Ent.Turn(false);
    }

    #endregion

    #region METODOS ############################################################
    
    private void EnemyMove()
    {
        MoveX = Mathf.MoveTowards(MoveX, MoveMaxSpd, MoveAcc *60 *Time.fixedDeltaTime);

        if ((Ent.lookDir && Ent.rightColl) || (!Ent.lookDir && Ent.leftColl))
        {
            Ent.Turn(!Ent.lookDir);
            MoveDirX = Ent.lookDir ? 1 : -1;
            MoveX = Ent.moveVelPrev.x;
        }   
    }

    private void CountTimers()
    {
        
    }

    #endregion
}
