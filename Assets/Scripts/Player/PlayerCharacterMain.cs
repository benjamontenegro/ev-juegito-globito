using UnityEngine;
using UnityEditor;
using TMPro;
using System;
using Unity.Mathematics;
using System.Threading;

[RequireComponent(typeof(GameEntity))]
public class PlayerCharacterMain : MonoBehaviour
{
    #region refs ####################################

    [Header("Referencias")]

    GameEntity Ent;
    PlatformerStats stats;
    AudioManager _sfx;
    SpriteRenderer _spriteRenderer;
    Animator _anim;

    float timerJumpBuffer;
    float timerJumpCoyote;
    bool isGroundedSave; // CHECKEO DEL PISO PERSONAL, SE PASA AL ENT POR LAS WEAS
    // LIT SOLO HECHO PARA EL TIMERJUMPCOYOTE DE MIERDA QUE NO ME AGARRABA
    Vector2 moveAxis;
    
    bool isInflatedState = false;
    float timerInflated;
    float bumpHeadBounceBuffer = -4f;

    [SerializeField] float GravInflatedSpd = 0.3f;
    [SerializeField] float MaxInflatedSpd = -3.5f;
    [SerializeField] float MoveInflatedMaxSpd = 4f;
    [SerializeField] float MoveInflatedAcc = 0.35f;
    [SerializeField] float MoveInflatedFri = 0.03f;
    [SerializeField] float InflatedHeadBumpSpd = -7f;
    [SerializeField] float InflateStateSwapTimer = 0.4f;
    
    // [Header("Movimiento")]

    float MoveMaxSpd = 8f;
    float MoveAcc = 2.5f;
    float MoveFri = 4f;
    float MoveAirAcc = 1.2f;
    float MoveAirFri = 0.3f;

    float JumpSpd = 13.6f;
    
    bool isJumpHolding;
    float JumpHoldMult = 0.75f;
    float JumpEnemyStoolSpd = 10f;
    
    
    #endregion####################################
    
    #region llamadas o mensajes????? ####################################

    void Awake()
    {
        Ent = GetComponent<GameEntity>();
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
        
        Inflate();

        HandleSprites();
    }

    void OnCollisionEnter2D(Collision2D collide) => PlayerCollideCall(collide.gameObject, false);

    void OnTriggerEnter2D(Collider2D collide) => PlayerCollideCall(collide.gameObject, true);

    void OnDrawGizmos()
    {
        // if (PlatformerStats.debugTextEnable)
        // {
        GUIStyle _style = GUIStyle.none;
        _style.fontSize = 7;
        _style.normal.textColor = Color.white;
        
        string msg = $"\nballoon bump t {Mathf.Round(bumpHeadBounceBuffer *60)}"
        + $"\n (temp grounded) {isGroundedSave}";

        Handles.Label(transform.position -new Vector3(0, -1.5f), msg);
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
        var _maxspd = isInflatedState ? MoveInflatedMaxSpd : MoveMaxSpd;
        var _acc    = _ground ? MoveAcc : (isInflatedState ? MoveInflatedAcc : MoveAirAcc);
        var _fri    = _ground ? MoveFri : (isInflatedState ? MoveInflatedFri : MoveAirFri);

        if (moveAxis.x != 0)
        {
            Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, _maxspd *moveAxis.x, 
                _acc *60 *Time.deltaTime);
            
            if (isGroundedSave)
                TurnDirection(-moveAxis.x);
        }
        else
        {
            Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, 0, 
                _fri *60 *Time.deltaTime);
        }

        // GRAVEDAD

        var _finalgrav = isInflatedState ? GravInflatedSpd : stats.GravSpd;
        var _finalmaxfall = isInflatedState ? MaxInflatedSpd : stats.MaxFallSpd;

        Ent.moveVel.y = !_ground ? 
            Mathf.MoveTowards(Ent.moveVel.y, -_finalmaxfall, _finalgrav *60 *Time.fixedDeltaTime) : Ent.moveVel.y;

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
                Ent.moveVel.y = Mathf.Lerp(Ent.moveVel.y, 0, JumpHoldMult);
            isJumpHolding = false;
        }

        isGroundedSave = _groundwas; // PASARLE EL GROUNDSAVE QUE IGNORA EL ENT.GROUNDSAVE
        // ESTO NO MERECE SEMEJANTE WEBEO
        // ASJDHSKADHBSKD UNITY DE MIERDA
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

    /// <summary> comportamiento sobre inflacion... </summary>
    void Inflate()
    {
        // AL INFLARSE
        if (InputHandler.AbilityIsHeld && !isInflatedState)
        {
            isInflatedState = true;
            
            _sfx.PlayAudioClip("inflate");
            if (!isGroundedSave)
            {
                // Ent.moveVel.x *= 0.3f;
                Ent.moveVel.y = Mathf.Min(Ent.moveVel.y, JumpSpd -5f);
                
                isGroundedSave = false;
                Ent.isGrounded = false;
            }

            if (moveAxis.x != 0)
                TurnDirection(-moveAxis.x);
            
            isJumpHolding = false;
        }
    
        // CHOCARSE CON EL TECHO
        if (isInflatedState && Ent.bumpHead && bumpHeadBounceBuffer < 0f)
        {
            bumpHeadBounceBuffer = 0.017f *4;

            Ent.moveVel.x *= 0.3f;
            Ent.moveVel.y = InflatedHeadBumpSpd;

            _sfx.PlayAudioClip("bounce");
        }

        // DEJAR IR EL AIRE
        if (!InputHandler.AbilityIsHeld && isInflatedState && (timerInflated > InflateStateSwapTimer))
        {
            isInflatedState = false;
            timerInflated = 0f;

            _sfx.PlayAudioClip("pop");
            
            if (moveAxis.x != 0)
            {
                // Ent.moveVel.x = moveAxis.x *(MoveMaxSpd *1.3f);
                TurnDirection(-moveAxis.x);
            }
        }
    }

    /// <summary> Al chocar con un enemigo </summary>
    void PlayerCollideCall(GameObject collide, bool IsTrigger)
    {
        if (IsTrigger && collide.CompareTag("WallHurt"))
        {
            PlayerDie();
            return;
        }

        if (collide.CompareTag("EntitySpring"))
        {
            GameObject _spring  = collide.transform.parent.gameObject;
            int _springdir      = Mathf.RoundToInt(_spring.transform.eulerAngles.z / 90);

            bool _holdpress = InputHandler.JumpIsHeld;

            switch (_springdir)
            {
                default: break;
                case 0:
                    Ent.moveVel.y = JumpSpd * (_holdpress ? 1.6f : 0.8f);
                    Ent.moveVel.x *= 0.4f;
                break;

                case 1: case 3:
                    Ent.moveVel.x = (MoveMaxSpd +3.6f) * (_springdir == 3 ? 1 : -1) *(_holdpress ? 1.6f : 1f);
                    Ent.moveVel.y = JumpSpd *0.4f *(_holdpress ? 1.7f : 1f);

                break;
            }

            if (_holdpress)
                isJumpHolding = true;
            
            _sfx.PlayAudioClip("springbounce");

            _spring.GetComponent<Animator>().CrossFade(Animator.StringToHash("Bounce"), 0.7f, 0);
            return;
        }

        if (collide.CompareTag("EntityItem"))
        {
            GameObject _coin = collide.transform.parent.gameObject;
            // Debug.Log($"colision con {_coin.GetComponent<GameEntity>().entityName}");

            _sfx.PlayAudioClip("coin");
            Destroy(_coin);
            return;
        }

        if (collide.CompareTag("EntityEnemy"))
        {
            string enemy_name = collide.GetComponent<GameEntity>().entityName;

            switch (enemy_name)
            {
                default:
                    if (collide.transform.position.y < transform.position.y && !isGroundedSave)
                    {
                        bool _holdpress = InputHandler.JumpIsHeld;

                        _sfx.PlayAudioClip("bump");

                        Ent.moveVel.y = JumpEnemyStoolSpd * (_holdpress ? 1.4f : 1f);
                        if (_holdpress)
                        {
                            isJumpHolding = true;
                            // dsps tmbn aplicarle algo al enemigosinono
                        }
                    }
                    else
                    {
                        PlayerDie();
                    }
                break;

                case "spikeball":

                    PlayerDie();
                break;
            }
        }
    }

    // SPRITES!!!!!!!!!!!    
    // esto lo saque de un tutorial ☉‿⚆
    // una y otras mil cosas mas askjasdh
    // me da wiwi si poner los sprites asi pero seria muy caradura decirlo
    private static readonly int sprIdle = Animator.StringToHash("Idle");
    private static readonly int sprWalk = Animator.StringToHash("Walk");
    private static readonly int sprFall = Animator.StringToHash("Fall");
    private static readonly int sprBalloonIdle = Animator.StringToHash("BalloonIdle");

    int _currentSprite;
    float _lockedUntil;

    /// <summary> CAmbiar estados del anim <br/> iiiiiiiiiiii mira como spriteo </summary>
    void HandleSprites()
    {
        int sprite = GetSpriteState();
        
        if (sprite != _currentSprite)
        {
            _anim.CrossFade(sprite, 0f, 0);
            _currentSprite = sprite;
        }
    }

    /// <summary> estados de animacion segun sus ids </summary>
    int GetSpriteState()
    {
        if (Time.time < _lockedUntil)
            return _currentSprite; // bloquea los q usen el lockstate

        bool _walk = moveAxis.x != 0;
        bool _keyj = InputHandler.PlInp.Player.Attack1.WasPressedThisFrame();

        if (isInflatedState)
            return sprBalloonIdle;
        if (isGroundedSave)
        {
            if (_walk) 
                return sprWalk;
            return sprIdle;
        }
        return sprFall;

        // int Lockstate(int s, float t)
        // {
        //     // Es lo mismoque un crossfade pero
        //     // lo bloquea por un tiempo
        //     _lockedUntil = Time.time +t;
        //     // PODRIA SER UN TIMER MEJOR HMMMM
        //     // O DETECTAR CUANDO SE ACABE EL SPRITE
        //     // ESTO ES TEMPORAL MANITO
        //     return s;
        // }
    }

    /// <summary> contadores </summary>
    void DelayCounters()
    {
        // timerJumpBuffer = (timerJumpBuffer > 0f) ? 
        timerJumpBuffer -= Time.deltaTime; //: timerJumpBuffer;
        
        timerJumpCoyote = isGroundedSave ? 
            (0.017f *6) : timerJumpCoyote -Time.deltaTime;
        
        timerInflated = isInflatedState ? 
            timerInflated +Time.deltaTime : timerInflated;

        bumpHeadBounceBuffer -= Time.deltaTime;
    }

    void PlayerDie()
    {
        _sfx.PlayAudioClip("hurt");
        Destroy(gameObject);

        GameObject.Find("SceneHandler").GetComponent<SceneHandler>().TriggerPlayerRespawn();
    }

    #endregion ####################################
}
