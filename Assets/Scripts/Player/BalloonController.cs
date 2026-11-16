using UnityEditor;
using UnityEngine;

public class BalloonController : MonoBehaviour
{
    #region REFS ############################################################
    #endregion

    [SerializeField] GameEntity Entity;
    float XVel { get => Entity.moveVel.x; set => Entity.moveVel.x = value; }
    float YVel { get => Entity.moveVel.y; set => Entity.moveVel.y = value; }
    [SerializeField] PlatformerStats Stats;

    [SerializeField] float MoveAcc = 0.1f;
    [SerializeField] float MoveMaxVel = 1.2f;
    public Transform BalloonTarget;

    SpriteRenderer _sprite;

    #region EVENTOS ############################################################
    #endregion

    void Awake()
    {
        if (BalloonTarget == null)
            BalloonTarget = GameObject.FindWithTag("Player").transform;
        
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        TempBalloonBehaviour();
    }

    // void FixedUpdate()
    // {
    //     MoveToBalloonTarget();
    //     Entity.MoveApplyVelocity();
    // }

    // void OnDrawGizmos()
    // {
    //     Vector3 MoveDiff = BalloonTarget.position -transform.position;
    //     Vector3 _moveDir = MoveDiff -(MoveDiff *0.4f);

    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawRay(transform.position, new(XVel, YVel)); // VEL RAY
    //     Gizmos.DrawRay(transform.position, _moveDir *2f); // MOVE RAY

    //     Gizmos.color = Color.white;
    //     Gizmos.DrawRay(transform.position, MoveDiff);
    //     Handles.Label(transform.position +new Vector3(0f, 1f), 
    //         $"SPEED {_moveDir.magnitude}");
    // }

    #region METODOS ############################################################
    #endregion

    void TempBalloonBehaviour()
    {
        if (BalloonTarget == null)
        {
            Destroy(gameObject);
            return;
        }

        _sprite.enabled = BalloonTarget.GetComponent<PlayerCharacterMain>().IsInflated();
    }

    /// <summary> moverse al target con aceleracion maxima y la wea </summary>
    // void MoveToBalloonTarget()
    // {
    //     Vector3 MoveDiff = BalloonTarget.position -transform.position;
    //     Vector3 _moveDir = MoveDiff -(MoveDiff *0.7f);
        

    //     XVel = Mathf.MoveTowards(XVel, _moveDir.x *MoveMaxVel, MoveAcc);
    //     YVel = Mathf.MoveTowards(YVel, _moveDir.y *MoveMaxVel, MoveAcc);
    // }
    
}
