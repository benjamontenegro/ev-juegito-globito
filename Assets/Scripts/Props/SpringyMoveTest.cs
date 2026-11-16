using System;
using UnityEngine;
using UnityEngine.Timeline;

public class SpringyMoveTest : MonoBehaviour
{
    [SerializeField] GameEntity Entity;
    float Xvel { get => Entity.moveVel.x; set => Entity.moveVel.x = value; }
    float Yvel { get => Entity.moveVel.y; set => Entity.moveVel.y = value; }
    // PlatformerStats Stats;
    // AudioManager _sfx;

    [SerializeField] float MoveSpringFactor = 0.5f;
    [SerializeField] float MoveSpringDampening = 0.05f;
    [SerializeField] Transform posStart;

    void FixedUpdate()
    {
        Entity.PreCollisions();
        MovimientoResorte();
        Entity.MoveApplyVelocity();
        Entity.GetCollisionChecks(); // ACTUALIZAR CHECKEO COLISIONES
        Entity.PostCollisions(); // STOP SPEEDS BASED ON DETECTED COLLISIONS
    }

    void MovimientoResorte()
    {
        Vector3 MovePosStart = posStart.position;
        Vector2 _diff = transform.position - MovePosStart;
        Vector2 _moveDir = new( -Mathf.Sign(_diff.x), -Mathf.Sign(_diff.y));

        float _Xacc = (-MoveSpringFactor * _diff.x) -(MoveSpringDampening * Xvel);
        float _Yacc = (-MoveSpringFactor * _diff.y) -(MoveSpringDampening * Yvel);

        Xvel += _Xacc;
        Yvel += _Yacc;
    }

    void OnDrawGizmos()
    {
        Vector3 MovePosStart = posStart.position;
        Vector2 _diff = transform.position - MovePosStart;
        Vector2 _moveDir = new( -Mathf.Sign(_diff.x), -Mathf.Sign(_diff.y));

        Gizmos.DrawSphere(new Vector3(MovePosStart.x, MovePosStart.y, 0), 0.1f);

        DrawArrowRay(transform.position, new Vector3(Xvel /4, Yvel /4));

        Gizmos.color = Color.blue;
        DrawArrowRay(transform.position +new Vector3(-1, 0), new Vector3(0, _moveDir.y));
        DrawArrowRay(transform.position +new Vector3(0, 1), new Vector3(_moveDir.x, 0));
    }

    void DrawArrowRay(Vector3 origin, Vector3 direction)
    {
        float _rayaim = Mathf.Atan2(direction.y, direction.x);
        float _aim1 = _rayaim +((135f +20f) *Mathf.Deg2Rad);
        float _aim2 = _rayaim +((225f -20f)*Mathf.Deg2Rad);
        float _s = 0.25f;

        Ray _r = new Ray(origin, direction);
        
        Gizmos.DrawRay(origin, direction);
        Gizmos.DrawRay(origin +direction, new Vector3(Mathf.Cos(_aim1) *_s, Mathf.Sin(_aim1) *_s));
        Gizmos.DrawRay(origin +direction, new Vector3(Mathf.Cos(_aim2) *_s, Mathf.Sin(_aim2) *_s));
    }
}
