using UnityEngine;
using UnityEngine.InputSystem;
// using TMPro;
// using UnityEditor;

public class PlayerMain : MonoBehaviour
{
    #region REFERENCIAS + VARIABLES
    // ################################################################
    // REFERENCIA UFFFF

    [Header("Referencias uwu")]
    public PlayerMoveStats MoveStats;
    [SerializeField] private Collider2D _feetColl;
    [SerializeField] private Collider2D _bodyColl;

    private Rigidbody2D _rb;

    private Vector2 _moveVel;
    private bool _faceDir; 
    // BOOLEANO PQ -> 1 / 0 -<
    // DERECHA == TRUE, IZQUIERDA == FALSE

    private RaycastHit2D _groundHit;
    private RaycastHit2D _headHit;
    private bool _grounded;
    private bool _bumpHead;

    // COSAS SALTO
    public float VerticalVelocity { get; private set; }
    private bool _isJumping;
    private bool _isFalling;
    private bool _isFastFalling;
    private float _fastFallTime;
    private float _fastFallReleaseSpeed;
    private int _jumpsUsed;

    private float _apexPoint;
    private float _timePastApexThreshold;
    private bool _isPastApexThreshold;

    private float _jumpBufferTimer;
    private float _jumpReleasedOnBuffer;
    private float _jumpCoyoteTimer;

    #endregion

    #region BEHAVIOR
    // ################################################################

    private void Awake()
    {
        _faceDir = true;
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        
    }

    void FixedUpdate()
    {
        CollisionChecks();

        if (_grounded)
        {
            Move(MoveStats.MoveAcc, MoveStats.MoveFri, InputHandler.Movement);
        }
        else
        {
            Move(MoveStats.MoveAccAir, MoveStats.MoveFriAir, InputHandler.Movement);
        }
    }
    #endregion

    #region MOVIMIENTO 
    // ################################################################

    private void Move(float acc, float fri, Vector2 moveInput)
    {
        // check dir uwu
        TurnCheck(moveInput);

        if (moveInput != Vector2.zero)
        {
            Vector2 finalMoveVel = Vector2.zero;
            finalMoveVel = new Vector2(moveInput.x, 0f) * MoveStats.MaxMoveSpd;

            _moveVel = Vector2.Lerp(_moveVel, finalMoveVel, acc *Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVel.x, _rb.linearVelocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            _moveVel = Vector2.Lerp(_moveVel, Vector2.zero, fri *Time.fixedDeltaTime);
            _rb.linearVelocity = new Vector2(_moveVel.x, _rb.linearVelocity.y);
        }
    }

    private void TurnCheck(Vector2 moveInput)
    {
        if (_faceDir && moveInput.x < 0)
        {
            Turn(false);
        }
        else if (!_faceDir && moveInput.x > 0)
        {
            Turn(true);
        }
    }

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

    #endregion

    #region COLISIONES
    // ################################################################

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MoveStats.GroundDetectRayDist);
        
        _groundHit = Physics2D.BoxCast(
            boxCastOrigin, boxCastSize,0f, Vector2.down, MoveStats.GroundDetectRayDist, MoveStats.GroundLy);
        
        if (_groundHit.collider != null)
        {
            _grounded = true;
        }
        else { _grounded = false; }

        if (MoveStats.DebugShowGrounded)
        {
            Color rayCol = Color.green;

            Debug.DrawRay(new Vector2(boxCastOrigin.x -boxCastSize.x / 2, boxCastOrigin.y),
                Vector2.down *MoveStats.GroundDetectRayDist, rayCol);
            Debug.DrawRay(new Vector2(boxCastOrigin.x +boxCastSize.x / 2, boxCastOrigin.y),
                Vector2.down *MoveStats.GroundDetectRayDist, rayCol);
            Debug.DrawRay(new Vector2(boxCastOrigin.x -boxCastSize.x / 2, 
                boxCastOrigin.y -MoveStats.GroundDetectRayDist), Vector2.right *boxCastSize.x, rayCol);
        }

    }

    private void CollisionChecks()
    {
        IsGrounded();
    }

    #endregion

}