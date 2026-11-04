// using System;
using UnityEngine;

[RequireComponent(typeof(GameEntity))]
public class EnemySpikeballMain : MonoBehaviour, IEntityEnemy
{
    #region REFS ############################################################

    GameEntity Ent;
    float MoveX { get => Ent.moveVel.x; set => Ent.moveVel.x = value; }
    float MoveY { get => Ent.moveVel.y; set => Ent.moveVel.y = value; }
    PlatformerStats stats;
    AudioManager _sfx;
    
    float MoveYWaveTimer;

    [Header("BOLA DE PINCHOS")]
    [SerializeField] private float MoveMaxSpd = 3f;
    [SerializeField] private float MoveAcc = 0.8f;
    [SerializeField] private float MoveYWaveSpd = 1f;
    [SerializeField] private float MoveYWaveAcc = 0.7f;
    [SerializeField] private float MoveYWaveFreq = 3f;
    [SerializeField] private int MoveDirX;

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

    public bool CanEnemyDie() => true;
    
    public void Die() => Destroy(gameObject);
    
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
