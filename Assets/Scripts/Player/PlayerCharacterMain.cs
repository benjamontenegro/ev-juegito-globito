using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(EntityMovement))]
public class PlayerCharacterMain : MonoBehaviour
{
    #region refs ####################################

    [Header("Referencias")]

    EntityMovement Ent;
    PlatformerStats stats;
    AudioManager _sfx;

    SpriteRenderer _spriteRenderer;
    Animator _anim;

    float timerJumpBuffer;
    float timerJumpCoyote;
    bool isGroundedSave; // CHECKEO DEL PISO PERSONAL, SE PASA AL ENT POR LAS WEAS
    // LIT SOLO HECHO PARA EL TIMERJUMPCOYOTE DE MIERDA QUE NO ME AGARRABA
    Vector2 moveAxis;
    
    [Header("Movimiento")]
    [SerializeField] float MoveMaxSpd = 12f;
    [SerializeField] float MoveAcc = 3.5f;
    [SerializeField] float MoveFri = 6f;
    [SerializeField] float MoveAirAcc = 1f;
    [SerializeField] float MoveAirFri = 0.3f;

    [SerializeField] float JumpSpd = 17f;
    bool isJumpHolding;
    [SerializeField] float JumpHoldMult = 0.4f;
    
    #endregion####################################
    
    #region llamadas o mensajes????? ####################################

    void Awake()
    {
        Ent = GetComponent<EntityMovement>();
        stats = Ent.stats;

        _spriteRenderer = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        
        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();
    }

    void FixedUpdate()
    {
        Ent.PreCollisions(); // VELOCIDADES OLD
        PlayerMove(); // KI
        Ent.MoveApplyVelocity(); // VELOCITIES
        Ent.GetCollisionChecks(); // ACTUALIZAR CHECKEO COLISIONES
        Ent.PostCollisions(); // STOP SPEEDS BASED ON DETECTED COLLISIONS
    }

    void Update()
    {
        DelayCounters(); // CHECK FOR COUNTERS

        if (InputHandler.JumpWasPressed) // JUMP INPUTED
            timerJumpBuffer = 0.017f *7;
        
        moveAxis = InputHandler.Movement;
    }

    void OnDrawGizmos()
    {
        // if (PlatformerStats.debugTextEnable)
        // {
        GUIStyle _style = GUIStyle.none;
        _style.fontSize = 7;
        _style.normal.textColor = Color.white;
        
        string msg = $"\nbuffer {Mathf.Round(timerJumpBuffer *60)}"
        + $"\n (temp grounded) {isGroundedSave}";

        Handles.Label(transform.position -new Vector3(0, -3.5f), msg);
        // }
    }
    
    #endregion ####################################

    #region METODOS ############################################

    /// <summary> main pleyer </summary>
    void PlayerMove()
    {
        bool _groundwas = Ent.isGrounded;
        bool _ground = Ent.GetOnGround();

        // LANDING
        if (_ground && !_groundwas)
        {
            isJumpHolding = false;
            _sfx.PlayAudioClip("land");
        }
        _groundwas = _ground;

        /// SE MUEVE CON ACELERACION UWU
        float _acc = _ground ? MoveAcc : MoveAirAcc;;
        float _fri = _ground ? MoveFri : MoveAirFri;

        if (moveAxis.x != 0)
        {
            Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, MoveMaxSpd *moveAxis.x, 
                _acc *60 *Time.deltaTime);
            
            TurnDirection(-moveAxis.x);
        }
        else
        {
            Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, 0, 
                _fri *60 *Time.deltaTime);
        }

        // GRAVEDAD
        Ent.moveVel.y = !_ground ? 
            Mathf.MoveTowards(Ent.moveVel.y, -stats.MaxFallSpd,
            stats.GravSpd *60 *Time.fixedDeltaTime) : 0;

        // SALTO
        if (timerJumpBuffer > 0f && timerJumpCoyote > 0f)
        {
            Ent.moveVel.y = JumpSpd;
            isJumpHolding = true;

            timerJumpBuffer = -0.017f;
            timerJumpCoyote = -0.017f;
            _groundwas = false; // FORZAR EL ISGROUNDSAVE A FALSO

            _sfx.PlayAudioClip("jump");
        }

        // SOLTAR EL SALTO
        if (isJumpHolding && !InputHandler.JumpIsHeld)
        {
            if (Ent.moveVel.y > 0f)
            {
                Ent.moveVel.y = Mathf.Lerp(Ent.moveVel.y, 0, JumpHoldMult);
            }
            isJumpHolding = false;
        }

        isGroundedSave = _groundwas; // PASARLE EL GROUNDSAVE QUE IGNORA EL ENT.GROUNDSAVE
        // ESTO NO MERECE SEMEJANTE WEBEO
        // ASJDHSKADHBSKD UNITY DE MIERDA

        HandleSprites();
    }

    /// <summary> cambiar direccion y sprite segun sign(float)  </summary>
    void TurnDirection(float turnDir)
    {
        float _dir = Mathf.Sign(turnDir);
        if (_dir == 0)
            return;
        
        _spriteRenderer.flipX = _dir == 1;
        Ent.Turn(_dir);
    }

    /// <summary> coso de sprites </summary>
    void HandleSprites()
    {
        bool _walk = moveAxis.x == 0;
        

        _anim.SetBool("isGrounded", isGroundedSave);
        _anim.SetBool("isWalking", _walk);
        // Que recuerdos bro...
        
        // if (isGroundedSave)
        // {
            
        // }
        // else
        // {
            
        // }
    }

    /// <summary> contadores </summary>
    void DelayCounters()
    {
        // timerJumpBuffer = (timerJumpBuffer > 0f) ? 
        timerJumpBuffer -= Time.deltaTime; //: timerJumpBuffer;
        
        timerJumpCoyote = isGroundedSave ? 
            (0.017f *6) : timerJumpCoyote -Time.deltaTime;
    }

    #endregion ####################################
}
