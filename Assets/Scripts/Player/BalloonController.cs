using UnityEngine;

public class BalloonController : MonoBehaviour
{
    #region REFS ############################################################
    #endregion

    [SerializeField] GameEntity Entity;
    float MoveX { get => Entity.moveVel.x; set => Entity.moveVel.x = value; }
    float MoveY { get => Entity.moveVel.y; set => Entity.moveVel.y = value; }
    [SerializeField] PlatformerStats Stats;

    public Transform BalloonTarget;

    #region KASHDKAHSD hrmno no se como llamar esto ############################################################
    #endregion

    void Awake()
    {
        if (BalloonTarget == null)
            BalloonTarget = GameObject.FindWithTag("Player").transform;
    }

    void FixedUpdate()
    {
        MoveToBalloonTarget();
    }

    #region METODOS ############################################################
    #endregion

    void MoveToBalloonTarget()
    {
        Vector3 moveToPosLerp = Vector3.Lerp(transform.position, BalloonTarget.position, 0.1f);
        
        Entity.MoveToPos(moveToPosLerp);
        
    }
    
}
