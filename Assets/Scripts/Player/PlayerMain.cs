using System.Threading;
using NUnit.Framework;
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
    public AudioManager Sound;
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
    private bool _jumpReleasedOnBuffer;
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
        // SOLO REVISA LOS INPUTS DEL SALTO
        JumpChecks();
        CountTimers();
    }

    void FixedUpdate()
    {
        CollisionChecks();
        Jump();

        float FinalAcc = _grounded ? MoveStats.MoveAcc : MoveStats.MoveAccAir;
        float FinalFri = _grounded ? MoveStats.MoveFri : MoveStats.MoveFriAir;

        Move(FinalAcc, FinalFri, InputHandler.Movement);
    }
    #endregion

    #region MOVIMIENTO X
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

    #region MOVIMIENTO Y
    // ################################################################

    private void JumpChecks()
    { 
        // PRESS JUMP 
        if (InputHandler.JumpWasPressed)
        {
            _jumpBufferTimer = MoveStats.JumpBuffer;
            _jumpReleasedOnBuffer = false;
        }

        // RELEASE JUMP
        if (InputHandler.JumpWasReleased)
        {
            // SE SOLTO MIENTRAS HABIA BUFFER
            if (_jumpBufferTimer > 0f)
                _jumpReleasedOnBuffer = true;
            
            // EN PLENO SALTO + YENDO HACIA ARRIBA
            if (_isJumping && VerticalVelocity > 0 )
            {
                if (_isPastApexThreshold) // ACTIVAR AL INICIAR LA CAIDA
                {
                    _isPastApexThreshold = false;
                    _isFastFalling = true;
                    _fastFallTime = MoveStats.timeForUpcancel;
                    VerticalVelocity = 0f; // DETENER TOTAL
                }
                else
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }
        
        // DETECT JUMP BUFFER
        if (_jumpBufferTimer > 0f)
        {
            if (!_isJumping && _grounded || _jumpCoyoteTimer > 0f) // START JUMP + BUFFER + COYOTE 
            {
                InitiateJump(1);
                if (_jumpReleasedOnBuffer)
                {
                    _isFastFalling = true;
                    _fastFallReleaseSpeed = VerticalVelocity;
                }
            }
            else if (_isJumping && _jumpsUsed < MoveStats.JumpsCanAmount) // DOUBLE/AIR JUMP (LEGAL)
            {
                InitiateJump(1);
                _isFastFalling = false;
            }
            else if (_isFalling && _jumpsUsed < MoveStats.JumpsCanAmount -1) // AIR JUMP AFTER COYOTE
            {
                 InitiateJump(2); // SPEND 2 BECAUSE YOU ALREADY MISSED YOUR GROUND JUMP
                _isFastFalling = false;
            }
        }
        
        // LANDED
        if ((_isJumping || _isFalling) && _grounded && VerticalVelocity <= 0f)
        {
            _isFalling = false;
            _isJumping = false;
            _isFastFalling = false;
            _fastFallTime = 0f;
            _isPastApexThreshold = false;
            VerticalVelocity = Physics2D.gravity.y;
            _jumpsUsed = 0;

            Sound.PlayAudioClip(Sound.LandSound);
        }
    }

    private void InitiateJump(int AmountJumpsUsed)
    {
        if (!_isJumping)
        {
            _isJumping = true;
        }
        
        _jumpBufferTimer = 0f;
        _jumpsUsed += AmountJumpsUsed;
        VerticalVelocity = MoveStats.InicialJumpVel;

        Sound.PlayAudioClip(Sound.JumpSound);
    }
 
    private void Jump()
    {
        // APPLY GRAV
        if (_isJumping)
        {
             // CHECK FOR BUMPING HEADS
            // if (_bumpHead) { _isFastFalling = true; }

            // GRAVITY ASCENT
            if (VerticalVelocity >= 0f)
            {
                // APEX JUMP
                _apexPoint = Mathf.InverseLerp(MoveStats.InicialJumpVel, 0f, VerticalVelocity);
                if (_apexPoint > MoveStats.ApexThreshold)
                {
                    if (!_isPastApexThreshold)
                    {
                        _isPastApexThreshold = true;
                        _timePastApexThreshold = 0f;
                    }
                    else
                    {
                        _timePastApexThreshold += Time.deltaTime;
                        if (_timePastApexThreshold < MoveStats.ApexHangTime)
                        {
                            VerticalVelocity = 0f;
                        }
                        else
                        {
                            VerticalVelocity = -0.01f;
                        }
                    }
                }
                else // GRAVITY DESCENT NOT PAST APEX
                {
                    VerticalVelocity += MoveStats.Gravity *Time.fixedDeltaTime;
                    if (_isPastApexThreshold)
                    {
                        _isPastApexThreshold = false;
                    }
                }
            }            
            else if (!_isFastFalling) // GRAVITY DESCENT AFTER APEX
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMult *Time.fixedDeltaTime;
            }

            else if (VerticalVelocity < 0f) // REGULAR FALLING
            {
                if (!_isFalling)
                {
                    _isFalling = true;
                }
            }
        }
        
        // JUMPCUT
        if (_isFastFalling)
        {
            if (_fastFallTime >= MoveStats.timeForUpcancel)
            {
                VerticalVelocity += MoveStats.Gravity * MoveStats.GravityOnReleaseMult *Time.fixedDeltaTime; 
            }
            else if (_fastFallTime < MoveStats.timeForUpcancel)
            {
                VerticalVelocity = Mathf.Lerp(_fastFallReleaseSpeed, 0f, 
                    (_fastFallTime / MoveStats.timeForUpcancel));
            }

            _fastFallTime += Time.fixedDeltaTime;
        }

        // NO JUMP GRAVITY DESCENT
        if (!_grounded && !_isJumping)
        {
            if (!_isFalling)
                _isFalling = true;
            
            VerticalVelocity += MoveStats.Gravity *Time.fixedDeltaTime;
        }

        // CLAMP FALLSPD
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -MoveStats.MaxFallSpd, 50f);

        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, VerticalVelocity);

    }

    #endregion

    #region COLISIONES
    // ################################################################

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(_feetColl.bounds.center.x, _feetColl.bounds.min.y);
        Vector2 boxCastSize = new Vector2(_feetColl.bounds.size.x, MoveStats.GroundDetectRayDist);
        
        _groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize,0f, Vector2.down,
            MoveStats.GroundDetectRayDist, MoveStats.GroundLy);
        
        if (_groundHit.collider != null)
        {
            _grounded = true;
        }
        else { _grounded = false; }

        #region DEBUG SHIT
        if (MoveStats.DebugShowGrounded)
        {
            Color rayCol = _grounded ? Color.red : Color.green;

            Debug.DrawRay(new Vector2(boxCastOrigin.x -boxCastSize.x / 2, boxCastOrigin.y),
                Vector2.down *MoveStats.GroundDetectRayDist, rayCol);
            Debug.DrawRay(new Vector2(boxCastOrigin.x +boxCastSize.x / 2, boxCastOrigin.y),
                Vector2.down *MoveStats.GroundDetectRayDist, rayCol);
            Debug.DrawRay(new Vector2(boxCastOrigin.x -boxCastSize.x / 2, 
                boxCastOrigin.y -MoveStats.GroundDetectRayDist), Vector2.right *boxCastSize.x, rayCol);
        }
        #endregion
    }

    private void BumpedHead()
    {
        Vector2 BoxCastOrigin = new Vector2(_feetColl.bounds.center.x, _bodyColl.bounds.max.y); 
        Vector2 BoxCastSize = new Vector2(_feetColl.bounds.size.x *MoveStats.HeadWidth,
            MoveStats.HeadDetectRayDist);

        _headHit = Physics2D.BoxCast(BoxCastOrigin, BoxCastSize, 0f, Vector2.up, 
            MoveStats.HeadDetectRayDist, MoveStats.GroundLy);
        
        if (_headHit.collider != null)
        {
            _bumpHead = true;
        }
        else { _bumpHead = false; }

        #region DEBUG SHIT
        if (MoveStats.DebugShowGrounded)
        {
            float HeadW = MoveStats.HeadWidth;
            Color rayCol = _bumpHead ? Color.red : Color.green;

            Debug.DrawRay(new Vector2(BoxCastOrigin.x -BoxCastSize.x / 2 *HeadW, BoxCastOrigin.y),
                Vector2.up *MoveStats.HeadDetectRayDist, rayCol);
            Debug.DrawRay(new Vector2(BoxCastOrigin.x +(BoxCastSize.x / 2) *HeadW, BoxCastOrigin.y),
                Vector2.up *MoveStats.HeadDetectRayDist, rayCol);
            Debug.DrawRay(new Vector2(BoxCastOrigin.x -BoxCastSize.x / 2, 
                BoxCastOrigin.y -MoveStats.HeadDetectRayDist), Vector2.right *BoxCastSize.x *HeadW, rayCol);
        }
        #endregion
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }

    #endregion

    #region TIMERS
    // ################################################################

    private void CountTimers()
    {
        _jumpBufferTimer -= Time.deltaTime;

        _jumpCoyoteTimer = (!_grounded) ? 
            _jumpCoyoteTimer -= Time.deltaTime : MoveStats.JumpCoyote;

        // if (_grounded)
        // {    
        //     _jumpCoyoteTimer -= Time.deltaTime;
        // }
        // else
        // { _jumpCoyoteTimer = MoveStats.JumpCoyote; }
    }

    #endregion

}