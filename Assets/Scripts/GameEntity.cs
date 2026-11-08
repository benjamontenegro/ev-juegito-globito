// using System;
// using UnityEditor;
using UnityEngine;

public enum EntityType
{
    NULL = -1,
    PLAYER,
    PROP,
    ENEMY,
    ENEMYPROJECTILE,
    ITEM
}

[RequireComponent(typeof(Rigidbody2D))]
// [RequireComponent(typeof(Collider2D))]
public class GameEntity : MonoBehaviour
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
    [HideInInspector] public bool isGroundClipping;
    [HideInInspector] public bool isWallSideClipping;

    Rigidbody2D _rb;

    public EntityType entityType = EntityType.NULL;
    public string entityName = "";

    [Header("Colideador colisor colison")]
    [SerializeField] private Collider2D _coll;

    #endregion

    void Awake() 
    { 
        lookDir = true;
        _rb = GetComponent<Rigidbody2D>();
    }

    void OnDrawGizmos()
    {
        if (stats.debugBboxEnable)
        {
            DrawBodyDebug();
            DrawCeilingDebug();
            DrawGroundedDebug();
            DrawSideDebug(true);
            DrawSideDebug(false);
        }
                
        // if (stats.debugTextEnable)
        // {
        //     GUIStyle _style = GUIStyle.none;
        //     _style.fontSize = 12;
        //     _style.normal.textColor = Color.white;
            
        //     Handles.Label(transform.position -new Vector3(0, -1f),
        //         $"xv {moveVel.x} yv {moveVel.y} \n gr {isGrounded} sd l{leftColl} r{rightColl}");
        // }
        
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
        leftColl = GetOnWallSide(false);
        rightColl = GetOnWallSide(true);
        bumpHead = HitCollAbove().collider != null;
        isGroundClipping = HitCollBody(true).collider != null;
        isWallSideClipping = HitCollBody(false).collider != null;
    }

    /// <summary> Stop moveVel on collisions </summary>
    public void PostCollisions()
    {
        if ((leftColl && moveVel.x < 0) || (rightColl && moveVel.x > 0))
            moveVel.x = 0f;
        if ((isGrounded && moveVel.y < 0) || (bumpHead && moveVel.y > 0))
            moveVel.y = 0f;
    }

    /// <summary> Turn player left or right + rotate 180 <br/> (true right, false left) </summary>
    public void Turn(bool turnRight)
    {
        if (turnRight && !lookDir)
        {
            lookDir = true;
            // GetComponent<SpriteRenderer>().flipX = false;
        }
        if (!turnRight && lookDir)
        {
            lookDir = false;
            // GetComponent<SpriteRenderer>().flipX = true;
            // transform.Rotate(0f, -180f, 0f);
        }
    }
    /// <summary> Turn player left or right + rotate 180 <br/> BASED ON A FLOAT SIGNUM </summary>
    public void Turn(float turnAxis)
    {
        if (Mathf.Sign(turnAxis) == -1f)
        {
            lookDir = false;
        }
        if (Mathf.Sign(turnAxis) == 1f)
        {
            lookDir = true;
        }
    }
    
    /// <summary> moveVel -> RigidBody2D.linearVelocity </summary>
    public void MoveApplyVelocity()
    {
        _rb.linearVelocity = new Vector2(moveVel.x, moveVel.y);
    }

    public void MoveToPos(Vector2 newPos)
    {
        _rb.MovePosition(newPos);
    }

    #endregion ############################################################

    #region COLLISIONS ############################################################
    
    /// <summary> Detect if on ground or platform </summary>
    public bool GetOnGround()
    {
        bool _ground = HitCollGrounded(false).collider != null;
        bool _onewayground = HitCollGrounded(true).collider != null;
        bool _bodyclipping = HitCollBody(true).collider != null;

        if (_ground || (_onewayground && moveVel.y <= 0 && !_bodyclipping))
            return true;
        
        return false;
    }

    public bool GetOnWallSide(bool isRightSide)
    {
        bool _solidwall = HitCollSides(isRightSide, false).collider != null;
        
        RaycastHit2D _onewaywall = HitCollSides(isRightSide, true);
        bool _onewaywalldid = _onewaywall.collider != null;

        // bool _onewayDiff = false;
        bool _onewayspd = false;
        
        if (_onewaywalldid)
        {
            // if (isRightSide)
            //     _onewayDiff = _coll.bounds.max.x <= _onewaywall.collider.bounds.min.x;
            // else
            //     _onewayDiff = _coll.bounds.min.x >= _onewaywall.collider.bounds.max.x;
            _onewayspd = isRightSide ? !(moveVel.x < 0) : !(moveVel.x > 0);
        }

        bool _bodyclipping = HitCollBody(false).collider != null;
        
        if (_solidwall || (_onewaywalldid && _onewayspd && !_bodyclipping))
            return true;

        return false;
    }

    /// <summary>
    /// Get raycast if its clipping with any wall
    /// </summary>
    public RaycastHit2D HitCollBody(bool IsNotSide)
    {
        LayerMask wallFlags = IsNotSide ? stats.GroundOneWayFlags : stats.SideOneWayFlags;

        Vector2 origin = new Vector2(
            _coll.bounds.center.x,
            _coll.bounds.center.y);
        Vector2 size = new Vector2(
            _coll.bounds.size.x -stats.ClippingColReach,
            _coll.bounds.size.y -stats.ClippingColReach);
        Vector2 offsetDir = Vector2.zero;
        float offsetDist = 0f;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, wallFlags);
    }

    /// <summary> get a raycast from ground/one way platforms </summary>
    public RaycastHit2D HitCollGrounded(bool OneWay)
    {
        LayerMask GroundFlags = OneWay ? stats.GroundOneWayFlags : stats.GroundFlags;

        Vector2 origin = new Vector2(_coll.bounds.center.x, _coll.bounds.min.y);
        Vector2 size = new Vector2(_coll.bounds.size.x -stats.ColReachDif,
            stats.GroundedColReach /2);
        Vector2 offsetDir = Vector2.down;
        float offsetDist = stats.GroundedColReach /4;
        
        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, GroundFlags);
    }

    /// <summary> get a raycast form ceiling </summary>
    public RaycastHit2D HitCollAbove()
    {
        // OCUPA GROUNDEDCOLS POR MIENTRAS UWU
        Vector2 origin = new(_coll.bounds.center.x, _coll.bounds.max.y);
        Vector2 size = new Vector2(_coll.bounds.size.x 
            -stats.HeadWidthDif -stats.ColReachDif,
            stats.GroundedColReach /2);
        Vector2 offsetDir = Vector2.up;
        float offsetDist = stats.GroundedColReach /4;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, stats.CeilFlags);
    }

    /// <summary> get raycast from left/right side </summary>
    public RaycastHit2D HitCollSides(bool isRightSide, bool OneWay)
    {
        LayerMask WallFlags = !OneWay ? stats.AllWallFlags : 
            (isRightSide ? stats.SideLeftFlags : stats.SideRightFlags);

        Vector2 origin = new Vector2(
            isRightSide? _coll.bounds.max.x : _coll.bounds.min.x,
            _coll.bounds.center.y);
        Vector2 size = new Vector2(
            stats.SideColReach /2,
            _coll.bounds.size.y -stats.ColReachDif);
        Vector2 offsetDir = isRightSide? Vector2.right : Vector2.left;
        float offsetDist = stats.SideColReach /4;

        return Physics2D.BoxCast(origin, size, 0f, offsetDir, offsetDist, WallFlags);
    }

    /// <summary> activar o desactivar el collider2d </summary>
    public void ColliderToggleEnable(bool Enabled)
    {
        _coll.enabled = Enabled;
    }
    
    #endregion ############################################################

    #region MORE METHODS ########################################################

    // ????

    #endregion


    #region EXTRA STUFF DEBUG ############################################################
    
    public void DrawBoxCast(Vector2 origin, Vector2 size, Vector2 dir, float dist)
    {
        Vector2 pointA = new Vector2(
            origin.x - (size.x /2) +(dir.x *dist),
            origin.y - (size.y /2) +(dir.y *dist));
        Vector2 pointB = new Vector2(
            origin.x + (size.x /2) +(dir.x *dist),
            origin.y + (size.y /2) +(dir.y *dist));
        
        Gizmos.DrawLine(new Vector2(pointA.x, pointA.y), new Vector2(pointB.x, pointA.y));
        Gizmos.DrawLine(new Vector2(pointA.x, pointA.y), new Vector2(pointA.x, pointB.y));
        Gizmos.DrawLine(new Vector2(pointA.x, pointB.y), new Vector2(pointB.x, pointB.y));
        Gizmos.DrawLine(new Vector2(pointB.x, pointA.y), new Vector2(pointB.x, pointB.y));
        Gizmos.DrawLine(new Vector2(pointA.x, pointA.y), new Vector2(pointB.x, pointB.y));
    }

    public void DrawGroundedDebug()
    {
        Vector2 origin = new Vector2(_coll.bounds.center.x, _coll.bounds.min.y);
        Vector2 size = new Vector2(_coll.bounds.size.x -stats.ColReachDif,
            stats.GroundedColReach /2);
        Vector2 offsetDir = Vector2.down;
        float offsetDist = stats.GroundedColReach /4;

        Gizmos.color = isGrounded ? Color.red : Color.yellow;
        DrawBoxCast(origin, size, offsetDir, offsetDist);
        Gizmos.color = Color.black;
    }

    public void DrawSideDebug(bool isRightSide)
    {
        bool _check = isRightSide ? rightColl : leftColl;

        Vector2 origin = new Vector2(
            isRightSide? _coll.bounds.max.x : _coll.bounds.min.x,
            _coll.bounds.center.y);
        Vector2 size = new Vector2(
            stats.SideColReach /2,
            _coll.bounds.size.y -stats.ColReachDif);
        Vector2 offsetDir = isRightSide? Vector2.right : Vector2.left;
        float offsetDist = stats.SideColReach /4;
        
        Gizmos.color = _check ? Color.red : Color.yellow;
        DrawBoxCast(origin, size, offsetDir, offsetDist);
        Gizmos.color = Color.black;
    }

    public void DrawCeilingDebug()
    {
        Vector2 origin = new(_coll.bounds.center.x, _coll.bounds.max.y);
        Vector2 size = new Vector2(_coll.bounds.size.x 
            -stats.HeadWidthDif -stats.ColReachDif,
            stats.GroundedColReach /2);
        Vector2 offsetDir = Vector2.up;
        float offsetDist = stats.GroundedColReach /4;

        Gizmos.color = bumpHead ? Color.red : Color.yellow;
        DrawBoxCast(origin, size, offsetDir, offsetDist);
        Gizmos.color = Color.black;
    }

    public void DrawBodyDebug()
    {
        Vector2 origin = new Vector2(
            _coll.bounds.center.x,
            _coll.bounds.center.y);
        Vector2 size = new Vector2(
            _coll.bounds.size.x -stats.ClippingColReach,
            _coll.bounds.size.y -stats.ClippingColReach);
        Vector2 offsetDir = Vector2.zero;
        float offsetDist = 0f;

        Gizmos.color = !isWallSideClipping? (isGroundClipping ? Color.darkRed : Color.darkGreen) : Color.darkMagenta;
        DrawBoxCast(origin, size, offsetDir, offsetDist);
        Gizmos.color = Color.black;
    }
    
    #endregion ############################################################

}
