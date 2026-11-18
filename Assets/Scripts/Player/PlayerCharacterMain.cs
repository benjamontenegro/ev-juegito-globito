using UnityEngine;
using Unity.Mathematics;

// using LombaxGuy.STG;

// using UnityEngine.Rendering.Universal;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(GameEntity))]
public class PlayerCharacterMain : MonoBehaviour
{
    #region refs ############################################################

        [Header("Referencias")]

        [SerializeField] GameEntity Entity;
        [SerializeField] PlatformerStats stats;
        AudioManager _sfx;
        SpriteRenderer _sprRender;
        Animator _anim;
        [SerializeField] Transform debugDrawPos;

        /// <summary> (Entity.moveVel.x) </summary>
        float XVel { get => Entity.moveVel.x; set => Entity.moveVel.x = value; }
        /// <summary> (Entity.moveVel.y) </summary>
        float YVel { get => Entity.moveVel.y; set => Entity.moveVel.y = value; }

        [Header("Movimiento (Regular)")]
        float MoveXvel = 7f;
        float MoveAcc = 2.6f;
        float MoveFri = 4f;
        float MoveAirAcc = 1.3f;
        float MoveAirFri = 0.2f;
        [SerializeField] float JumpSpd = 13.5f; // CALCULATE THIS
        bool isJumpHolding;
        float JumpHoldMinTime = 0.025f;
        float JumpHoldMult = 0.75f;
        float JumpEnemyStoolSpd = 10f;

        // MOVEMENT VARIABLES ############################################################
        [Header("Movimiento (Inflado)")]
        [SerializeField] float InflatedGravSpd = 0.23f;
        [SerializeField] float InflatedGravFallSpd = 0.04f;
        [SerializeField] float InflatedFallYvel = 2f;
        [SerializeField] float InflatedFallFastYvel = 7f;
        [SerializeField] float InflatedJumpSpd = 7.7f;
        float InflatedMoveXvel = 7f;
        [SerializeField] float InflatedMoveAirXvel = 4f;
        [SerializeField] float InflatedMoveAcc = 0.4f;
        [SerializeField] float InflatedMoveFri = 0.02f;
        float InflatedHeadBumpSpd = -5f;
        int MaxNumberInflations = 1;

        bool isInflatedState = false;
        int inflationsLeft;
        float timerInflated;
        float bumpHeadBounceBuffer = -4f;
        [SerializeField] float FlapSpd = 9f;
        [SerializeField] float FlapSlowXvel = 4f;

        GameObject CurrentBalloonObj;
        [SerializeField] GameObject prefBalloon;

        float timerJumpInputBuffer;
        float timerJumpCoyote;
        float timerStunMoveAxisX;
        bool isGroundedSave; 
        // CHECKEO DEL PISO PERSONAL, SE PASA AL ENT POR LAS WEAS
        Vector2 moveAxis;
        bool _isHoldingDown;
            
    #endregion####################################
    
    #region EVENTOS ####################################

    void Awake()
    {
        if (Entity == null) 
            Entity = GetComponent<GameEntity>();
        stats = Entity.stats;

        _sprRender = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        _sfx = GameObject.FindWithTag("GameHandler").GetComponentInChildren<AudioManager>();

        if (debugDrawPos == null)
            debugDrawPos = transform;
    }

    void FixedUpdate()
    {
        Entity.PreCollisions(); // VELOCIDADES OLD
        PlayerMove(); // KI
        Entity.MoveApplyVelocity(); // VELOCITIES
        Entity.GetCollisionChecks(); // ACTUALIZAR CHECKEO COLISIONES
        Entity.PostCollisions(); // STOP SPEEDS BASED ON DETECTED COLLISIONS
    }
    void OnCollisionEnter2D(Collision2D collide) => PlayerCollideCall(collide.gameObject, false);
    void OnTriggerEnter2D(Collider2D collide) => PlayerCollideCall(collide.gameObject, true);

    void Update()
    {
        DelayCounters(); // CHECK FOR COUNTERS
        SetInputs();
        Inflate();
        HandleSprites();
    }

    // void OnDrawGizmos()
    // {
    //     if (Entity.stats.debugTextEnable)
    //     {
    //         string msg = "";

    //         switch(debugDrawPos.position.z)
    //         {
    //             default: case -5:
    //             msg = "LMAO";
    //             break;
                
    //             case 0:
    //             msg = "ANIMS" 
    //             + $"\n Current Sprite Int: {_currentSprite}"
    //             + $"\n AnimStateTemp {AnimStateTemp}";
    //             break;

    //             case 1:
    //             msg = "BALLOON SHIT"
    //             + $"\n Inflated? {isInflatedState}"
    //             + $"\n inflations left {inflationsLeft}/{MaxNumberInflations}"
    //             + $"\n inflation state coold {timerInflated *60}";
    //             // + $"\n";
    //             break;

    //             case 2:
    //             msg = "SPEED SHIT"
    //             + $"\njump inital spd {JumpSpd}"
    //             + $"\njump ballon spd {InflatedJumpSpd}"
    //             + $"\nflap spd {FlapSpd}";
    //             break;
    //         }

    //         Handles.Label(debugDrawPos.position, msg);
    //     }

    // }

    #endregion ####################################

    #region METODOS ############################################

    /// <summary> main pleyer </summary>
    void PlayerMove()
    {
        bool _groundwas = Entity.isGrounded;
        bool _ground = Entity.GetOnGround();

        // LANDING
        if (_ground && !_groundwas)
        {
            isJumpHolding = false;
            _sfx.PlayAudioClip("land");
            if (!isInflatedState)
                RefillInflations(MaxNumberInflations);
            timerJumpCoyote = (0.017f *8);

            SetSpriteAnimState(sprIdleLand);
        }
        _groundwas = _ground;

        // SE MUEVE EN X PAPA 
        Vector2 _axis = timerStunMoveAxisX > 0 ? new Vector2(0, moveAxis.y) : moveAxis;
        float _maxspd = isInflatedState ? 
            (isGroundedSave ? InflatedMoveXvel : InflatedMoveAirXvel) : MoveXvel;

        // si el jugador presiona el mismo sentido de su velocidad,
        // se pasa de la velocidad maxima y esta en el aire: _l da true XD
        bool _axisspeed = moveAxis.x == Mathf.Sign(XVel);
        bool _l = _axisspeed && (Mathf.Abs(XVel) > _maxspd) && !isGroundedSave;
        
        float _acc    = _ground ? MoveAcc : (_l ? 0 : (isInflatedState ? InflatedMoveAcc : MoveAirAcc));
        float _fri    = _ground ? MoveFri : (isInflatedState ? InflatedMoveFri : MoveAirFri);
        
        if (_axis.x != 0)
        {
            XVel = Mathf.MoveTowards(XVel, _maxspd *_axis.x, _acc *60 *Time.deltaTime);
            if (isGroundedSave)
                TurnDirection(_axis.x);
        }
        else
        {
            XVel = Mathf.MoveTowards(XVel, 0, _fri *60 *Time.deltaTime);
        }

        // GRAVEDAD
        var _finalgrav = isInflatedState ? 
            (YVel > 0 || _isHoldingDown ? InflatedGravSpd : InflatedGravFallSpd) : stats.GravSpd;
        var _finalmaxfall = isInflatedState ? 
            (_isHoldingDown ? InflatedFallFastYvel : InflatedFallYvel) : stats.MaxFallSpd;
        
        if (isInflatedState && YVel < -InflatedFallYvel && !_isHoldingDown)
            _finalgrav = stats.GravSpd;

        YVel = !_ground ? 
            Mathf.MoveTowards(YVel, -_finalmaxfall, _finalgrav *60 *Time.fixedDeltaTime) : YVel;

        // SALTO
        if (!(isInflatedState && timerJumpCoyote <= 0))
            Jump(_groundwas);
        else
            FlapJump();

        // SOLTAR EL SALTO
        if (isJumpHolding && !InputHandler.JumpIsHeld && (timerJumpCoyote <= -JumpHoldMinTime))
        {
            if (YVel > 0f && !isInflatedState)
                YVel = Mathf.Lerp(YVel, 0, JumpHoldMult);
            isJumpHolding = false;
        }

        isGroundedSave = _groundwas; // PASARLE EL GROUNDSAVE QUE IGNORA EL ENT.GROUNDSAVE
        // ESTO NO MERECE SEMEJANTE WEBEO
        // ASJDHSKADHBSKD UNITY DE MIERDA
    }

    /// <summary> aplicar varialbes del inputs </summary>
    void SetInputs()
    {
        moveAxis = InputHandler.Movement;

        _isHoldingDown = moveAxis.y == -1;

        if (InputHandler.JumpWasPressed) // JUMP INPUTED
            timerJumpInputBuffer = 0.017f *8;
    }

    /// <summary> salto(En el piso?, eje) </summary>
    void Jump(bool groundWas)
    {
        if (timerJumpInputBuffer > 0f && timerJumpCoyote > 0f)
        {
            // float _jumpxaxis = Mathf.Abs(XVel) / MoveXvel *1.2f;

            YVel = isInflatedState ? InflatedJumpSpd : JumpSpd;
            // XVel = _isHoldingDown ? JumpMoveSpdBoost *_jumpxaxis *Axis.x : XVel;
            
            isJumpHolding = true;

            timerJumpInputBuffer = 0.0f;
            timerJumpCoyote = 0.0f;
            groundWas = false; // FORZAR EL ISGROUNDSAVE A FALSO

            ForceSpriteAnimStateEnd();
            _sfx.PlayAudioClip("jump");
        }
    }

    /// <summary> cambiar direccion y sprite segun sign(float)  </summary>
    void TurnDirection(float turnDir)
    {
        float _dir = Mathf.Sign(turnDir);
        if (_dir == 0)
            return;
        
        _sprRender.flipX = _dir != 1;
        Entity.Turn(_dir);
    }

    #region DELAYS Y TIMERS!!!!! ############################################################
    #endregion ############################################################

    /// <summary> contadores </summary>
    void DelayCounters()
    {
        // timerJumpInputBuffer = (timerJumpInputBuffer > 0f) ? 
        timerJumpInputBuffer -= Time.deltaTime; //: timerJumpInputBuffer;
        
        timerJumpCoyote = isGroundedSave ? timerJumpCoyote : timerJumpCoyote -Time.deltaTime;
        
        timerInflated = isInflatedState ? 
            timerInflated +Time.deltaTime : timerInflated;

        bumpHeadBounceBuffer -= Time.deltaTime;
        timerStunMoveAxisX -= Time.deltaTime;
    }

    #region MORIR!!!!!!!!!!!!
    #endregion
    /// <summary> al morirse (llama a scenehandler) </summary>
    void PlayerDie()
    {
        _sfx.PlayAudioClip("hurt");
        Destroy(gameObject);

        GameObject.Find("SceneHandler").GetComponent<SceneHandler>().TriggerPlayerRespawn();
    }

    #region INFLARSE!!!!! ############################################################
    #endregion ############################################################

    public bool IsInflated()
    {
        return isInflatedState;
    }

    /// <summary> comportamiento sobre inflacion... </summary>
    void Inflate()
    {
        // DEJAR IR EL AIRE
        if (isInflatedState)
        {
            Deflate();
        }
        else
        {
            InflateBegin();
        }

        // AL INFLARSE
        
        // CHOCARSE CON EL TECHO
        if (isInflatedState && Entity.bumpHead && bumpHeadBounceBuffer < 0f)
        {
            bumpHeadBounceBuffer = 0.017f *4;
            // XVel *= 0.3f;
            YVel = InflatedHeadBumpSpd;

            _sfx.PlayAudioClip("bounce");
        }

        // ALETEAR
        // if (timerInflated >= 1.5f)
        //     FlapJump();
    }

    /// <summary> cuando el jugador presiona el boton de inflarse </summary>
    void InflateBegin()
    {
        if (InputHandler.AbilityWasPressed && inflationsLeft > 0)
        {
            isInflatedState = true;
            if (moveAxis.x != 0)
                TurnDirection(moveAxis.x);

            CurrentBalloonObj = Instantiate(prefBalloon, 
                transform.position + new Vector3(0.4f *Entity.GetLookDir(), 0, 3), 
                quaternion.identity);
            CurrentBalloonObj.GetComponent<BalloonMain>().SetPosTarget(transform);
            CurrentBalloonObj.GetComponent<BalloonMain>().MoveForce(
                Vector3.right *Entity.GetLookDir() *4f);

            // Entity.isGrounded = false;
            
            if (!isGroundedSave)
            {
                // XVel *= 0.3f;
                XVel = Mathf.Clamp(XVel, -InflatedMoveXvel, InflatedMoveXvel);
                YVel = Mathf.Clamp(YVel, -InflatedFallYvel, 3.7f);
            }
            
            isJumpHolding = false;
            inflationsLeft --;

            _sfx.PlayAudioClip("inflate");
            SetSpriteAnimState(sprBalloonSpawn);
        }
    }

    void Deflate()
    {
        if (InputHandler.AbilityWasPressed)
        {

            if (CurrentBalloonObj != null)
                CurrentBalloonObj.GetComponent<BalloonMain>().Die();

            isInflatedState = false;
            timerInflated = 0f;

            // _sfx.PlayAudioClip("pop");
            
            if (moveAxis.x != 0)
                TurnDirection(moveAxis.x);

            if (isGroundedSave)
                RefillInflations(MaxNumberInflations);
        }
    }

    /// <summary> "salto" aleteo cuando esta inflado  </summary>
    void FlapJump()
    {
        if (timerJumpInputBuffer > 0f)
        {
            YVel = (YVel <= 1f) ? FlapSpd *0.4f : FlapSpd;
            XVel = Mathf.Clamp(XVel, -FlapSlowXvel, FlapSlowXvel);
                        
            timerJumpInputBuffer = -0.017f;
            if (moveAxis.x != 0)
                TurnDirection(moveAxis.x);

            SetSpriteAnimState(sprFlap);
            _sfx.PlayAudioClip("jump");
        }
    }

    public void RefillInflations(int amount = 1)
    {
        inflationsLeft = Mathf.Max(MaxNumberInflations, amount, inflationsLeft);
        // if (amount > MaxNumberInflations)
    }

    #region COLISION ENTIDADES!!!!! ############################################################
    #endregion
    /// <summary> Al chocar con un enemigo </summary>
    void PlayerCollideCall(GameObject collide, bool IsTrigger)
    {
        if (IsTrigger && collide.CompareTag("WallHurt"))
        {
            PlayerDie();
            return;
        }

        if (IsTrigger)
            collide = collide.transform.parent.gameObject;

        // ESTO PARA ACCEDER AL OBJETO PADRE
        GameObject _object = collide;

        if (collide.CompareTag("EntityProp"))
        {
            switch (_object.GetComponent<GameEntity>().entityName)
            {
                default: 
                    Debug.Log("???");
                break;
                case "spring":
                    // Debug.Log("SPRING");
                    int _springdir = Mathf.RoundToInt(_object.transform.eulerAngles.z / 90);
                    bool _springLookRight = _springdir == 3;

                    RefillInflations();
                    
                    switch (_springdir)
                    {
                        default: break; /// 0 ARRIBA, 1 IZQUIERDA, 3 DERECHA
                        // NO DEBERIA SER ASI!!!!!!!!!!!!!!!!
                        // CAMBIAR A SIN(DIR) & COS(DIR) FLOJO!!!
                        case 0:
                            Entity.MoveToPos(new Vector3(
                                Mathf.Lerp(transform.position.x, _object.transform.position.x, 0.7f), 
                                _object.transform.position.y +0.8f));
                            YVel = JumpSpd * 1.65f;
                            XVel = 0f;
                            timerStunMoveAxisX = (0.017f *3);
                        break;

                        case 1: case 3:
                            Entity.MoveToPos(
                                new Vector3(_object.transform.position.x +(_springLookRight ? 0.5f : -0.5f), 
                                Mathf.Lerp(transform.position.y, _object.transform.position.y +0.5f, 0.5f)));
                            XVel = (MoveXvel + 3.6f) * (_springLookRight ? 1 : -1) * 1.7f;
                            YVel = JumpSpd;
                            timerStunMoveAxisX = (0.017f *8);
                        break;
                    }

                    _sfx.PlayAudioClip("springbounce");
                    
                    _object.GetComponent<Animator>().CrossFade(Animator.StringToHash("Bounce"), 0.7f, 0);
                return;
            }
        }

        if (collide.CompareTag("EntityItem"))
        {
            if (_object.TryGetComponent<IEntityItem>(out var item))
            {
                item.Collect(Entity);
                switch(item.GetEntityName())
                {
                    default: break;
                    case "airtank":
                        RefillInflations();
                    break;
                }
            }
            return;
        }

        if (collide.CompareTag("EntityEnemy"))
        {
            string enemy_name = _object.GetComponent<GameEntity>().entityName;
            switch (enemy_name)
            {
                default:
                    if (collide.transform.position.y < transform.position.y && !isGroundedSave)
                    {
                        bool _holdpress = InputHandler.JumpIsHeld;

                        _sfx.PlayAudioClip("bump");

                        YVel = JumpEnemyStoolSpd * (_holdpress ? 1.4f : 1f);
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

    #region SPRITES!! ############################################################
        // SPRITES!!!!!!!!!!!    (ANIMATION STATES)

        private static readonly int sprBalloonSpawn = Animator.StringToHash("ANIM_PJ_BALLOON_SPAWN");
        
        private static readonly int sprDie = Animator.StringToHash("ANIM_PJ_DIE");

        private static readonly int sprDuck         = Animator.StringToHash("ANIM_PJ_DUCK");
        private static readonly int sprDuckBegin    = Animator.StringToHash("ANIM_PJ_DUCK_BEGIN");
        private static readonly int sprDuckCrawl    = Animator.StringToHash("ANIM_PJ_DUCK_CRAWL");
        private static readonly int sprDuckEnd      = Animator.StringToHash("ANIM_PJ_DUCK_END");
        private static readonly int sprDuckFall     = Animator.StringToHash("ANIM_PJ_DUCK_FALL");

        private static readonly int sprFall         = Animator.StringToHash("ANIM_PJ_FALL");
        private static readonly int sprFlap         = Animator.StringToHash("ANIM_PJ_FLAP");

        private static readonly int sprIdle         = Animator.StringToHash("ANIM_PJ_IDLE");
        private static readonly int sprIdleLand     = Animator.StringToHash("ANIM_PJ_IDLE_LAND");
        private static readonly int sprIdleWin      = Animator.StringToHash("ANIM_PJ_IDLE_WIN");

        private static readonly int sprLongJump     = Animator.StringToHash("ANIM_PJ_LONGJUMP");
        private static readonly int sprLongJumpAntic = Animator.StringToHash("ANIM_PJ_LONGJUMP_ANTIC");
        private static readonly int sprLongJumpTemp = Animator.StringToHash("ANIM_PJ_LONGJUMP_TEMP");

        private static readonly int sprStun         = Animator.StringToHash("ANIM_PJ_STUN");
        private static readonly int sprWalk         = Animator.StringToHash("ANIM_PJ_WALK");
        private static readonly int sprWalkBegin    = Animator.StringToHash("ANIM_PJ_WALK_BEGIN");

        int _currentSprite;
        int AnimStateTemp;
        // float _lockedUntil;
        bool _AnimStateTempBypassReset;
        
        // AnimStateTemp PUEDE SER UN HASH DE ANIMACION, O 0.
        // SI ES 0., NO HAY NINGUNA ANIMACION TEMPORAL REPRODUCIENDOSE

        /// <summary> CAmbiar estados del anim <br/> iiiiiiiiiiii mira como spriteo </summary>
        void HandleSprites()
        {
            // // SI ANIMACION TEMPORAL TERMINO, REINICIA FRAME
            if (AnimStateTemp != 0 && IsAnimationEnding() && !_AnimStateTempBypassReset)
                AnimStateTemp = 0;

            // ACTUALIZAR ANIMACION SEGUN SPRANIM O NO
            int sprite = GetSpriteState(_currentSprite);
            
            // REPRODUCIR ANIMACION SI NO SE ESTA REPRODUCIOENDO
            if (sprite != _currentSprite)
            {
                _anim.CrossFade(sprite, 0.0f, -1);
                _currentSprite = sprite;
            }

            _AnimStateTempBypassReset = false;
        }

        /// <summary> Revisar si la animacion esta terminando </summary>
        bool IsAnimationEnding()
        {
            AnimatorStateInfo _sprInfo = _anim.GetCurrentAnimatorStateInfo(0);
            return _sprInfo.normalizedTime > 1.0f;
        }

        /// <summary> estados de animacion segun sus ids </summary>
        int GetSpriteState(int CurrentSprite)
        {
            bool _walk = moveAxis.x != 0;

            if (AnimStateTemp == 0)
            {
                if (isGroundedSave)
                {
                    if (_walk) 
                        return sprWalk;
                    return sprIdle;
                }
                return sprFall;   
            }
            else
                return AnimStateTemp;
        }

        /// <summary> Activar animacion temporal con X animacion </summary>
        void SetSpriteAnimState(int stateName)
        {
            AnimStateTemp = stateName;
            _AnimStateTempBypassReset = true;

            // _anim.CrossFade(AnimStateTemp, 0.0f, -1);
            _anim.Play(AnimStateTemp, -1, 0.0f);
            _currentSprite = AnimStateTemp;
            _anim.Update(0.0f);
        }

        /// <summary> Desactivar animacion actual si hay </summary>
        void ForceSpriteAnimStateEnd()
        {
            AnimStateTemp = 0;
            _AnimStateTempBypassReset = false;
        }

    #endregion 

    #endregion ####################################
}
