using UnityEngine;

[RequireComponent(typeof(PlatformerBody))]
public class EnemyBouncerMain : MonoBehaviour
{
    #region REFS ############################################################
        
    [Header("Referencias")]

    private PlatformerBody Ent;
    private PlatformerStats Stats;

    [SerializeField] private AudioManager _sfx; // PUEDE SER MANUAL??
    
    [Header("Variables del enemigo")]
    [SerializeField] private float MoveSpd = 12.5f;
    [SerializeField] private float MoveAcc = 0.5f;
    [SerializeField] private int MoveDirX;

    [SerializeField] private float JumpSpd = 16f;
    [SerializeField] private float JumpTimeAmt = 3f;
    private float JumpTimer;
    
    #endregion ############################################################
    
    #region EVENTS ############################################################

    void Awake()
    {
        Ent = GetComponent<PlatformerBody>();
        Stats = Ent.stats;
    }

    void Update()
    {
        EnemyJumpTimer();
    }

    void FixedUpdate()
    {
        Ent.PreCollisions();
        Ent.GetCollisionChecks(); // GET CHECKS
        Ent.PostCollisions(); // STOP SPEEDS ON DETECTED COLLISIONS

        EnemyMove();
        
        Ent.MoveApplyVelocity(); // APPLY VELOCITIES
    }
    
    #endregion ############################################################

    #region ENEMY MOVE ############################################################
    
    ///<summary> enemy jumpin timin thing stuff </summary>
    void EnemyJumpTimer()
    {
        JumpTimer = (JumpTimer > 0) ? JumpTimer -Time.deltaTime : JumpTimer;
    }

    /// <summary> enemy moving stuff </summary>
    void EnemyMove()
    {
        Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, MoveSpd *MoveDirX, 
            MoveAcc *60 *Time.fixedDeltaTime);

        Ent.moveVel.y = !Ent.isGrounded ? 
            Mathf.MoveTowards(Ent.moveVel.y, -Stats.MaxFallSpd, 
            Stats.GravSpd *60 *Time.fixedDeltaTime) : 0;
        
        if (Ent.isGrounded && JumpTimer <= 0)
        {
            Ent.moveVel.y = JumpSpd;
            _sfx.PlayAudioClip("jump");
            JumpTimer = JumpTimeAmt;
        }

        if ((Ent.lookDir && Ent.rightColl) || (!Ent.lookDir && Ent.leftColl))
        {
            Ent.Turn(!Ent.lookDir);
            MoveDirX = Ent.lookDir ? 1 : -1;
            Ent.moveVel.x = Ent.moveVelPrev.x;
        }   
    }

    #endregion ############################################################

}
