using UnityEngine;

// [RequireComponent(typeof(Rigidbody2D))]
// [RequireComponent(typeof(Collider2D))]
public class EnemyBouncerMain : MonoBehaviour
{
    #region REFS ############################################################
        
    [Header("Referencias")]

    [SerializeField] private PlatformerStats _mover;

    private Vector2 _moveVel;
    private Vector2 _moveVelPrev;

    private bool _grounded;
    private bool _faceDir;

    private bool _leftColl;
    private bool _rightColl;
    private bool _bumpHead;

    private Rigidbody2D _rb;
    [SerializeField] private Collider2D _coll; // MANUAL O AUTO
    // private RaycastHit2D _collHit;
    [SerializeField] private AudioManager _sfx; // PUEDE SER MANUAL??
    
    [Header("Variables del enemigo")]
    [SerializeField] private float MoveSpd = 12.5f;
    [SerializeField] private float MoveAcc = 0.5f;
    [SerializeField] private int MoveDirX;

    [SerializeField] private float JumpSpd = 16f;
    [SerializeField] private float JumpTimeAmt = 3f;
    private float JumpTimer;

    // [SerializeField] private float gravSpd;
    
    #endregion ############################################################
    #region EVENTS ############################################################
    #endregion ############################################################
    
    void Awake()
    {
        _faceDir = true;
        if (MoveDirX == -1)
            Turn(false);
        
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        EnemyJumpTimer();
    }

    void FixedUpdate()
    {
        _moveVelPrev = _moveVel;

        GetCollisionChecks(); // GET CHECKS
        EnemyPostColl(); // STOP SPEEDS ON DETECTED COLLISIONS

        EnemyMove(); // BEHAVIOR

        MoveApplyVelocity(); // APPLY VELOCITIES
    }

    #region ENEMY MOVE ############################################################
    
    ///<summary> enemy jumpin timin thing stuff </summary>
    private void EnemyJumpTimer()
    {
        JumpTimer = (JumpTimer > 0) ? JumpTimer -Time.deltaTime : JumpTimer;
    }

    ///<summary> enemy moving stuff </summary>
    private void EnemyMove()
    {
        _moveVel.x = Mathf.MoveTowards(_moveVel.x, MoveSpd *MoveDirX, 
            MoveAcc *60 *Time.fixedDeltaTime);

        _moveVel.y = !_grounded ? 
            Mathf.MoveTowards(_moveVel.y, -_mover.MaxFallSpd,
                _mover.GravSpd *60 *Time.fixedDeltaTime) : 0;
        
        if (GetOnGround() && JumpTimer <= 0)
        {
            _moveVel.y = JumpSpd;
            _sfx.PlayAudioClip("jump");
            JumpTimer = JumpTimeAmt;
        }

        if ((_faceDir && _rightColl) || (!_faceDir && _leftColl))
        {
            Turn(!_faceDir);
            MoveDirX = _faceDir ? 1 : -1;
            _moveVel.x = _moveVelPrev.x;
        }   
    }

    #endregion ############################################################
    #region MOVEMENT ############################################################

    /// <summary> Set Collision Flags if they're detected </summary>
    private void GetCollisionChecks()
    {
        _grounded = GetOnGround();
        _leftColl = HitCollSides(false).collider != null;
        _rightColl = HitCollSides(true).collider != null;
        _bumpHead = HitCollAbove().collider != null;

        if (_mover.debugBboxEnable)
            DebugDrawBoxCast(_coll.bounds.center, _coll.bounds.size, Vector2.zero, 0f);
    }

    /// <summary> Stop _moveVel on collisions </summary>
    private void EnemyPostColl()
    {
        if ((_leftColl && _moveVel.x < 0) || (_rightColl && _moveVel.x > 0))
            _moveVel.x = 0f;
        if ((_grounded && _moveVel.y < 0) || (_bumpHead && _moveVel.y > 0))
            _moveVel.y = 0f;
    }

    /// <summary> Turn player left or right + rotate 180 </summary>
    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            _faceDir = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            _faceDir = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    /// <summary> _moveVel -> RigidBody2D.linearVelocity </summary>
    private void MoveApplyVelocity()
    {
        _rb.linearVelocity = new Vector2(_moveVel.x, _moveVel.y);
    }

    #endregion ############################################################
    #region COLLISIONS ############################################################

    // THIS ONE SHOULD BE USED BY MANY
    private RaycastHit2D HitCollGrounded(bool OneWay)
    {
        LayerMask GroundFlags = OneWay ? _mover.GroundOneWayFlags : _mover.GroundFlags;

        Vector2 origin = new Vector2(_coll.bounds.center.x, _coll.bounds.min.y);
        Vector2 size = new Vector2(_coll.bounds.size.x -_mover.ColReachDif,
            _mover.GroundedColReach /2);
        Vector2 offsetDir = Vector2.down;
        float offsetDist = _mover.GroundedColReach /2;
        
        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, GroundFlags);
    }

    private RaycastHit2D HitCollAbove()
    {
        // OCUPA GROUNDEDCOLS POR MIENTRAS UWU
        Vector2 origin = new(_coll.bounds.center.x, _coll.bounds.max.y);
        Vector2 size = new Vector2(_coll.bounds.size.x -_mover.HeadWidthDif -_mover.ColReachDif,
            _mover.GroundedColReach /2);
        Vector2 offsetDir = Vector2.up;
        float offsetDist = _mover.GroundedColReach /2;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, _mover.CeilFlags);
    }

    private RaycastHit2D HitCollSides(bool isRightSide)
    {
        Vector2 origin = new Vector2(
            isRightSide? _coll.bounds.max.x : _coll.bounds.min.x,
            _coll.bounds.center.y);
        Vector2 size = new Vector2(
            _mover.SideColReach /2,
            _coll.bounds.size.y -_mover.GroundedColReach -_mover.ColReachDif);
        Vector2 offsetDir = isRightSide? Vector2.right : Vector2.left;
        float offsetDist = _mover.SideColReach /2;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, _mover.SideFlags);
    }
    
    /// <summary> Detect if on ground </summary>
    private bool GetOnGround()
    {
        bool _ground = HitCollGrounded(false).collider != null;
        bool _onewayground = HitCollGrounded(true).collider != null;

        if (_ground || (_onewayground && _moveVelPrev.y <= 0f))
            return true;
        
        return false;
    }

    public void DebugDrawBoxCast(Vector2 origin, Vector2 size, Vector2 dir, float dist)
    {
        Color col = Color.lightGray;

        Vector2 pointA = new Vector2(
            origin.x - (size.x /2) +(dir.x *dist),
            origin.y - (size.y /2) +(dir.y *dist));
        Vector2 pointB = new Vector2(
            origin.x + (size.x /2) +(dir.x *dist),
            origin.y + (size.y /2) +(dir.y *dist));

        Debug.DrawLine(new Vector2(pointA.x, pointA.y), new Vector2(pointB.x, pointA.y), col);
        Debug.DrawLine(new Vector2(pointA.x, pointA.y), new Vector2(pointA.x, pointB.y), col);
        Debug.DrawLine(new Vector2(pointA.x, pointB.y), new Vector2(pointB.x, pointB.y), col);
        Debug.DrawLine(new Vector2(pointB.x, pointA.y), new Vector2(pointB.x, pointB.y), col);
    }
    
    #endregion ############################################################

}
