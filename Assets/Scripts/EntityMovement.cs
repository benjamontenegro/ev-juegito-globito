using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EntityMovement : MonoBehaviour
{
    #region REFS #############################################################

    public PlatformerStats stats;

    [HideInInspector] public Vector2 moveVel;
    [HideInInspector] public Vector2 moveVelPrev;
    [HideInInspector] public bool lookDir; // TRUE == ->, FALSE == <-

    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool leftColl;
    [HideInInspector] public bool rightColl;
    [HideInInspector] public bool bumpHead;

    private Rigidbody2D _rb;
    // LLENAR CAMPO A MANO
    [SerializeField] private Collider2D _coll;

    #endregion

    void Awake() { 
        lookDir = true;
        _rb = GetComponent<Rigidbody2D>();
    }

    #region MOVEMENT ############################################################

    /// <summary> save previous velocity, other checks, etc... </summary>
    public void PreCollisions()
    {
        moveVelPrev = moveVel;
    }

    /// <summary> Set Collision Flags if they're detected </summary>
    public void GetCollisionChecks()
    {
        isGrounded = GetOnGround();
        leftColl = HitCollSides(false).collider != null;
        rightColl = HitCollSides(true).collider != null;
        bumpHead = HitCollAbove().collider != null;
    }

    /// <summary> Stop moveVel on collisions </summary>
    public void PostCollisions()
    {
        if ((leftColl && moveVel.x < 0) || (rightColl && moveVel.x > 0))
            moveVel.x = 0f;
        if ((isGrounded && moveVel.y < 0) || (bumpHead && moveVel.y > 0))
            moveVel.y = 0f;
    }

    /// <summary> Turn player left or right + rotate 180 </summary>
    public void Turn(bool turnRight)
    {
        if (turnRight)
        {
            lookDir = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            lookDir = false;
            transform.Rotate(0f, -180f, 0f);
        }
    }

    /// <summary> moveVel -> RigidBody2D.linearVelocity </summary>
    public void MoveApplyVelocity()
    {
        _rb.linearVelocity = new Vector2(moveVel.x, moveVel.y);
    }

    #endregion ############################################################

    #region COLLISIONS ############################################################
    
    /// <summary> Detect if on ground or platform </summary>
    public bool GetOnGround()
    {
        bool _ground = HitCollGrounded(false).collider != null;
        bool _onewayground = HitCollGrounded(true).collider != null;

        if (_ground || (_onewayground && moveVelPrev.y <= 0f))
            return true;
        
        return false;
    }

    /// <summary> get a raycast from ground/one way platforms </summary>
    public RaycastHit2D HitCollGrounded(bool OneWay)
    {
        LayerMask GroundFlags = OneWay ? stats.GroundOneWayFlags : stats.GroundFlags;

        Vector2 origin = new Vector2(_coll.bounds.center.x, _coll.bounds.min.y);
        Vector2 size = new Vector2(_coll.bounds.size.x -stats.ColReachDif,
            stats.GroundedColReach /2);
        Vector2 offsetDir = Vector2.down;
        float offsetDist = stats.GroundedColReach /2;
        
        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, GroundFlags);
    }

    /// <summary> get a raycast form ceiling </summary>
    public RaycastHit2D HitCollAbove()
    {
        // OCUPA GROUNDEDCOLS POR MIENTRAS UWU
        Vector2 origin = new(_coll.bounds.center.x, _coll.bounds.max.y);
        Vector2 size = new Vector2(_coll.bounds.size.x -stats.HeadWidthDif -stats.ColReachDif,
            stats.GroundedColReach /2);
        Vector2 offsetDir = Vector2.up;
        float offsetDist = stats.GroundedColReach /2;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, stats.CeilFlags);
    }

    /// <summary> get raycast from left/right side </summary>
    public RaycastHit2D HitCollSides(bool isRightSide)
    {
        Vector2 origin = new Vector2(
            isRightSide? _coll.bounds.max.x : _coll.bounds.min.x,
            _coll.bounds.center.y);
        Vector2 size = new Vector2(
            stats.SideColReach /2,
            _coll.bounds.size.y -stats.GroundedColReach -stats.ColReachDif);
        Vector2 offsetDir = isRightSide? Vector2.right : Vector2.left;
        float offsetDist = stats.SideColReach /2;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, stats.SideFlags);
    }
    
    #endregion ############################################################

}
