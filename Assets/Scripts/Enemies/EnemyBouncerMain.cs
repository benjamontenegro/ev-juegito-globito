using UnityEngine;

[RequireComponent(typeof(EntityMovement))]
public class EnemyBouncerMain : MonoBehaviour
{
    #region REFS ############################################################
        
    [Header("Referencias")]

    EntityMovement Ent;
    PlatformerStats stats;
    AudioManager _sfx;
    
    [Header("Variables del enemigo")]
    [SerializeField] private float MoveMaxSpd = 12.5f;
    [SerializeField] private float MoveAcc = 0.5f;
    [SerializeField] private int MoveDirX;

    [SerializeField] private float JumpSpd = 16f;
    [SerializeField] private float JumpTimeAmt = 3f;
    float jumpTimer;

    float MoveX { get => Ent.moveVel.x; set => Ent.moveVel.x = value; }
    float MoveY { get => Ent.moveVel.y; set => Ent.moveVel.y = value; }

    #endregion ############################################################

    #region EVENTS ############################################################


    void Awake()
    {
        Ent = GetComponent<EntityMovement>();
        stats = Ent.stats;

        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();

        if (MoveDirX == -1)
            Ent.Turn(false);
    }

    void Update()
    {
        EnemyjumpTimer();
    }

    void FixedUpdate()
    {
        Ent.PreCollisions();
        EnemyMove();
        Ent.MoveApplyVelocity();
        Ent.GetCollisionChecks();
        Ent.PostCollisions();
    }
    
    #endregion ############################################################

    #region ENEMY MOVE ############################################################
    
    ///<summary> enemy jumpin timin thing stuff </summary>
    private void EnemyjumpTimer()
    {
        jumpTimer = (jumpTimer > 0) ? jumpTimer -Time.deltaTime : jumpTimer;
    }

    /// <summary> enemy moving stuff </summary>
    private void EnemyMove()
    {
        bool _ground = Ent.GetOnGround();

        MoveX = Mathf.MoveTowards(MoveX, MoveMaxSpd *MoveDirX, 
            MoveAcc *60 *Time.fixedDeltaTime);

        MoveY = !_ground ? 
            Mathf.MoveTowards(MoveY, -stats.MaxFallSpd, 
            stats.GravSpd *60 *Time.fixedDeltaTime) : 0;
        
        if (_ground && jumpTimer <= 0)
        {
            MoveY = JumpSpd;
            _sfx.PlayAudioClip("jump");
            jumpTimer = JumpTimeAmt;
        }

        if ((Ent.lookDir && Ent.rightColl) || (!Ent.lookDir && Ent.leftColl))
        {
            Ent.Turn(!Ent.lookDir);
            MoveDirX = Ent.lookDir ? 1 : -1;
            MoveX = Ent.moveVelPrev.x;
        }   
    }

    #endregion ############################################################

}
