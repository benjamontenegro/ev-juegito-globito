using System;
using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class EnemySpikeballMain : MonoBehaviour
{
    #region REFS ############################################################
        
    [Header("Referencias")]

    GameEntity Ent;
    PlatformerStats stats;
    AudioManager _sfx;
    
    [Header("Variables del enemigo")]
    [SerializeField] private float MoveMaxSpd = 3f;
    [SerializeField] private float MoveAcc = 0.8f;
    
    [SerializeField] private float MoveYWaveSpd = 1f;
    [SerializeField] private float MoveYWaveAcc = 0.7f;
    [SerializeField] private float MoveYWaveFreq = 3f;
    [SerializeField] private int MoveDirX;


    float MoveYWaveTimer;

    float MoveX { get => Ent.moveVel.x; set => Ent.moveVel.x = value; }
    float MoveY { get => Ent.moveVel.y; set => Ent.moveVel.y = value; }


    // private float movex { get ; set; }

    #endregion ############################################################

    #region EVENTOS ############################################################

    void Awake()
    {
        Ent = GetComponent<GameEntity>();
        stats = Ent.stats;

        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();

        if (MoveDirX == -1)
            Ent.Turn(false);
    }

    void FixedUpdate()
    {
        Ent.PreCollisions();
        EnemyMove();
        Ent.MoveApplyVelocity();
        Ent.GetCollisionChecks();
        Ent.PostCollisions();
    }

    void Update()
    {
        CountTimers();
    }

    #endregion

    #region METODOS ############################################################
    
    private void EnemyMove()
    {
        MoveX = Mathf.MoveTowards(MoveX, MoveMaxSpd *MoveDirX, MoveAcc *60 *Time.fixedDeltaTime);

        if ((Ent.lookDir && Ent.rightColl) || (!Ent.lookDir && Ent.leftColl))
        {
            Ent.Turn(!Ent.lookDir);
            MoveDirX = Ent.lookDir ? 1 : -1;
            MoveX = Ent.moveVelPrev.x;
        }

        float ySine = Mathf.Sin(MoveYWaveTimer *MoveYWaveFreq);

        MoveY = Mathf.MoveTowards(MoveY, MoveYWaveSpd *ySine, MoveYWaveAcc);
    }

    private void CountTimers()
    {
        MoveYWaveTimer += Time.deltaTime;
    }

    #endregion
}
