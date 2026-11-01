using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(EntityMovement))]
public class PlayerCharacterMain : MonoBehaviour
{
    [Header("Referencias")]

    private EntityMovement Ent;
    private PlatformerStats Stats;
    [SerializeField] private AudioManager _sfx; // PUEDE SER MANUAL??
    
    [Header("Movimiento")]
    [SerializeField] float MoveSpd = 12f;
    [SerializeField] float MoveAcc = 3.5f;
    [SerializeField] float MoveFri = 6f;
    [SerializeField] float MoveAirAcc = 1f;
    [SerializeField] float MoveAirFri = 0.3f;

    [SerializeField] float JumpSpd = 17f;
    private bool isJumpHolding;
    [SerializeField] float JumpHoldMult = 0.5f;
    
    private float timerJumpBuffer;
    private float timerJumpCoyote;
    private bool groundWas; 

    private Vector2 MoveAxis;
    
    void Awake()
    {
        Ent = GetComponent<EntityMovement>();
        Stats = Ent.stats;
    }

    void Update()
    {
        DelayCounters();

        if (InputHandler.JumpWasPressed) // JUMP INPUTED
            timerJumpBuffer = (0.017f *7);
        
        MoveAxis = InputHandler.Movement;
    }

    void FixedUpdate()
    {
        Ent.PreCollisions();

        PlayerMove();
        
        Ent.MoveApplyVelocity(); // APPLY VELOCITIES
        Ent.GetCollisionChecks(); // GET CHECKS
        Ent.PostCollisions(); // STOP SPEEDS ON DETECTED COLLISIONS
    }

    void OnDrawGizmos()
    {
        // if (PlatformerStats.debugTextEnable)
        // {
        GUIStyle _style = GUIStyle.none;
        _style.fontSize = 12;
        _style.normal.textColor = Color.white;
        
        Handles.Label(transform.position -new Vector3(0, -3f),
            $"coyote {timerJumpCoyote} \nbuffer {timerJumpBuffer}");
        // }
    }


    public void PlayerMove()
    {
        bool _ground = Ent.GetOnGround();

        if (_ground && !groundWas)
        {
            isJumpHolding = false;
            _sfx.PlayAudioClip("land");
        }
        groundWas = _ground;

        /// SE MUEVE CON ACELERACION UWU
        float _acc = _ground ? MoveAcc : MoveAirAcc;;
        float _fri = _ground ? MoveFri : MoveAirFri;

        if (MoveAxis.x != 0)
        {
            Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, MoveSpd *MoveAxis.x, 
                _acc *60 *Time.deltaTime);
            
            if (MoveAxis.x == 1)
                Ent.Turn(true);
            else
                Ent.Turn(false);
        }
        else
        {
            Ent.moveVel.x = Mathf.MoveTowards(Ent.moveVel.x, 0, 
                _fri *60 *Time.deltaTime);
        }

        Ent.moveVel.y = !_ground ? 
            Mathf.MoveTowards(Ent.moveVel.y, -Stats.MaxFallSpd,
            Stats.GravSpd *60 *Time.fixedDeltaTime) : 0;


        if ((timerJumpBuffer > 0f) && (timerJumpCoyote > 0f))
        {
            timerJumpBuffer = -1f;
            timerJumpCoyote = -1f;
            
            Ent.moveVel.y = JumpSpd;
            Ent.isGrounded = false; // DURO

            isJumpHolding = true;
            
             _sfx.PlayAudioClip("jump");
        }

        if (isJumpHolding && !InputHandler.JumpIsHeld)
        {
            if (Ent.moveVel.y > 0f)
            {
                Ent.moveVel.y = Mathf.Lerp(Ent.moveVel.y, 0, JumpHoldMult);
            }
            isJumpHolding = false;
        }
    }

    public void DelayCounters()
    {
        // timerJumpBuffer = (timerJumpBuffer > 0f) ? 
        timerJumpBuffer -= Time.deltaTime; //: timerJumpBuffer;
        timerJumpCoyote = Ent.isGrounded ? 
            0.1f : timerJumpCoyote -Time.deltaTime;
    }
}
